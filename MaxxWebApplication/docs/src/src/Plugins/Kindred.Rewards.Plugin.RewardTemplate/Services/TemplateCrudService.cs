using AutoMapper;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Infrastructure.Hosting.WebApi.Extensions;
using Kindred.Infrastructure.Kafka;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Helpers;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Messages.RewardTemplate;
using Kindred.Rewards.Core.Models.Promotions;
using Kindred.Rewards.Core.Models.Rewards;

using LinqKit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using DataModel = Kindred.Rewards.Core.Infrastructure.Data.DataModels;

namespace Kindred.Rewards.Plugin.Template.Services;

public interface ITemplateCrudService
{
    Task<PagedResponse<RewardTemplateDomainModel>> GetAllAsync(PromotionTemplateFilterDomainModel filter, PagedRequest pagination);

    Task<RewardTemplateDomainModel> GetAsync(string key, bool? isActive);

    Task<RewardTemplateDomainModel> CreateAsync(RewardTemplateDomainModel template);

    Task DeleteAsync(string key);

    Task UpdateMappingAsync(string key, List<string> rewardRns, string? user);
}

public class TemplateCrudService : ITemplateCrudService
{
    private readonly RewardsDbContext _context;
    private readonly ITimeService _timeService;
    private readonly IMapper _mapper;
    private readonly IKafkaProducerManager _producer;
    private readonly ILogger<TemplateCrudService> _logger;
    private readonly BlueprintsConfigurationModel _blueprintsConfiguration;

    public TemplateCrudService(
        ILogger<TemplateCrudService> logger,
        RewardsDbContext context,
        IOptions<BlueprintsConfigurationModel> blueprintOptions,
        ITimeService timeService,
        IMapper mapper,
        IKafkaProducerManager producer)
    {
        _blueprintsConfiguration = blueprintOptions.Value;
        _context = context;
        _timeService = timeService;
        _logger = logger;
        _mapper = mapper;
        _producer = producer;
    }


    public async Task<RewardTemplateDomainModel> CreateAsync(RewardTemplateDomainModel template)
    {
        var exist = await DoesExistAsync(template.TemplateKey);
        if (exist)
        {
            var errorMsg = $"Promotion template exist with template key {template.TemplateKey}";
            _logger.LogWarning(errorMsg);
            throw new RewardsValidationException(errorMsg);
        }

        _logger.LogInformation("Creating new promotion template {@template}", template);

        var addResult = await AddAsync(template);

        var evnt = _mapper.Map<RewardTemplateCreated>(addResult);

        await PublishRewardTemplateCreated(evnt);

        return await Task.FromResult(addResult);
    }

    public async Task DeleteAsync(string key)
    {
        var keyLower = key.ToLower();
        if (_blueprintsConfiguration.LockedTemplates.Any(tKey => tKey.ToLower() == keyLower))
        {
            _logger.LogWarning($"Attempted to delete a locked template key {key}");
        }
        else
        {
            var rewardTemplate = _context.RewardTemplates.FirstOrDefault(t => t.Key.ToLower() == keyLower);

            if (rewardTemplate == null)
            {
                _logger.LogWarning($"Attempted to delete a non-existent template key {key}");
            }
            else
            {
                rewardTemplate.Enabled = false;
                await _context.SaveChangesAsync();

                var deleteResult = _mapper.Map<RewardTemplateDomainModel>(rewardTemplate);

                var evnt = _mapper.Map<RewardTemplateUpdated>(deleteResult);

                await PublishRewardTemplateUpdated(evnt);
            }
        }
    }

    public async Task<PagedResponse<RewardTemplateDomainModel>> GetAllAsync(PromotionTemplateFilterDomainModel filter, PagedRequest pagination)
    {
        var templates = await FindAllAsync(filter, pagination);

        return templates;
    }

    public async Task<RewardTemplateDomainModel> GetAsync(string key, bool? isActive)
    {
        var template = await GetTemplateWithPromotionsAsync(key, isActive);

        if (template == null)
        {
            _logger.LogWarning("Template key {key} not found", key);
            throw new PromotionTemplateNotFoundException(key);
        }

        return template;
    }

    public async Task UpdateMappingAsync(string key, List<string> rewardRns, string? user)
    {
        var exists = await DoesExistAsync(key);

        if (!exists)
        {
            _logger.LogWarning("Template key {key} not found", key);
            throw new PromotionTemplateNotFoundException(key);
        }

        var promotions = await GetInternalAsync(rewardRns);

        var missing = rewardRns.Where(k => !promotions.Select(p => p.RewardId).Contains(k)).ToList();

        if (missing.Any())
        {
            _logger.LogWarning("Promotion with reward key {rewardRns} not found", missing.ToCsv());
            throw new PromotionsNotFoundException(missing);
        }

        _logger.LogInformation("Binding reward keys {@rewardRns} to template key {@key}", rewardRns, key);

        var updateMappingResult = await SetPromotionToPromotionTemplateAsync(key, rewardRns, user);

        var evnt = _mapper.Map<RewardTemplateUpdated>(updateMappingResult);

        await PublishRewardTemplateUpdated(evnt);
    }

    private async Task<List<RewardDomainModel>> GetInternalAsync(List<string> rewardRns)
    {
        var rewards = await _context.Rewards
            .Include(b => b.RewardTags).ThenInclude(bt => bt.Tag)
            .Include(p => p.RewardTemplateReward).ThenInclude(ptp => ptp.RewardTemplate)
            .AsNoTracking()
            .Where(r => rewardRns.Contains(r.Id))
            .ToListAsync();

        var rewardTemplates = rewards.SelectMany(reward => reward.RewardTemplateReward);

        if (rewards.Any() && (rewardTemplates?.Any() ?? false))
        {
            // Make sure there are no circular dependencies otherwise auto-mapper does recursive mapping
            foreach (var rewardTemplate in rewards.SelectMany(promotion => promotion.RewardTemplateReward.Select(ptp => ptp.RewardTemplate)))
            {
                rewardTemplate.RewardTemplateReward = null;
            }
        }

        var domain = _mapper.Map<List<RewardDomainModel>>(rewards);

        return domain;
    }

    public async Task<PagedResponse<RewardTemplateDomainModel>> FindAllAsync(PromotionTemplateFilterDomainModel filter, PagedRequest pagination)
    {
        var predicate = PredicateBuilder.New<DataModel.RewardTemplate>(true);

        if (!filter.IncludeDisabled)
        {
            predicate = predicate.And(p => p.Enabled);
        }

        if (!string.IsNullOrWhiteSpace(filter.TemplateKey))
        {
            predicate = predicate.And(p => p.Key.ToLower().Contains(filter.TemplateKey.ToLower()));
        }

        var query = await _context.RewardTemplates
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();

        var totalRecords = query.Count;

        var data = query
            .AsQueryable()
            .SkipWithPaging(pagination, filter).Queryable
            .AsNoTracking()
            .ToList();

        var domain = _mapper.Map<IReadOnlyCollection<RewardTemplateDomainModel>>(data).ToList();
        return new()
        {
            Offset = pagination.Offset,
            Limit = pagination.Limit,
            Items = domain,
            ItemCount = totalRecords
        };
    }

    private async Task<RewardTemplateDomainModel> GetTemplateWithPromotionsAsync(string promotionTemplateKey, bool? isActive)
    {

        var now = _timeService.UtcNow;

        var promoTemplateAndPromos =
            !isActive.HasValue ?
                (
                    //include all promos
                    await _context.RewardTemplates
                        .Include(pt => pt.RewardTemplateReward)
                        .ThenInclude(ptp => ptp.Reward)
                        .Where(pt => pt.Key == promotionTemplateKey)
                        .Select(
                            pt => new
                            {
                                PromoTemplate = pt,
                                PromoTemplPromos = pt.RewardTemplateReward
                            }).AsNoTracking().FirstOrDefaultAsync()
                )
                :
                (
                    isActive.Value ?
                        (
                            //include only active promos
                            await _context.RewardTemplates
                                .Include(pt => pt.RewardTemplateReward)
                                .ThenInclude(ptp => ptp.Reward)
                                .Where(pt => pt.Key == promotionTemplateKey)
                                .Select(
                                    pt => new
                                    {
                                        PromoTemplate = pt,
                                        PromoTemplPromos = pt.RewardTemplateReward.Where(
                                            ptp =>
                                                (ptp.Reward.StartDateTime < now
                                                 && ptp.Reward.ExpiryDateTime > now
                                                 && !ptp.Reward.IsCancelled)).ToList()
                                    }).AsNoTracking().FirstOrDefaultAsync()
                        )
                        :
                        (
                            //include only inactive promos
                            await _context.RewardTemplates
                                .Include(pt => pt.RewardTemplateReward)
                                .ThenInclude(ptp => ptp.Reward)
                                .Where(pt => pt.Key == promotionTemplateKey)
                                .Select(
                                    pt => new
                                    {
                                        PromoTemplate = pt,
                                        PromoTemplPromos = pt.RewardTemplateReward.Where(
                                            ptp =>
                                                !(ptp.Reward.StartDateTime < now
                                                  && ptp.Reward.ExpiryDateTime > now
                                                  && !ptp.Reward.IsCancelled)).ToList()
                                    }).AsNoTracking().FirstOrDefaultAsync()
                        )
                );

        if (promoTemplateAndPromos?.PromoTemplate == null)
        {
            return null;
        }

        var promoTemplate = promoTemplateAndPromos.PromoTemplate;
        promoTemplate.RewardTemplateReward = promoTemplateAndPromos.PromoTemplPromos;

        return _mapper.Map<RewardTemplateDomainModel>(promoTemplate);
    }

    private async Task PublishRewardTemplateCreated(RewardTemplateCreated rewardTemplateMessage)
    {
        try
        {
            await _producer.SendAsync(DomainConstants.TopicProfileTemplateUpdates, rewardTemplateMessage, rewardTemplateMessage.TemplateKey, Guid.NewGuid().ToString());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ERROR PUBLISHING {@rewardTemplateMessage} ", rewardTemplateMessage);
            throw;
        }
    }

    private async Task PublishRewardTemplateUpdated(RewardTemplateUpdated rewardTemplateMessage)
    {
        try
        {
            await _producer.SendAsync(DomainConstants.TopicProfileTemplateUpdates, rewardTemplateMessage, rewardTemplateMessage.TemplateKey, Guid.NewGuid().ToString());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ERROR PUBLISHING {@rewardTemplateMessage} ", rewardTemplateMessage);
            throw;
        }
    }

    private async Task<bool> DoesExistAsync(string promotionTemplateKey)
    {
        var exist = await _context.RewardTemplates.AnyAsync(t => t.Key == promotionTemplateKey);
        return exist;
    }

    private async Task<RewardTemplateDomainModel> AddAsync(RewardTemplateDomainModel promotionTemplate)
    {
        var data = _mapper.Map<DataModel.RewardTemplate>(promotionTemplate);

        _context.RewardTemplates.Add(data);
        await _context.SaveChangesAsync();

        return _mapper.Map<RewardTemplateDomainModel>(data);
    }

    private async Task<RewardTemplateDomainModel> SetPromotionToPromotionTemplateAsync(
        string promotionTemplateKey,
        IEnumerable<string> rewardRns,
        string? user)
    {
        var template = await _context.RewardTemplates
            .Include(t => t.RewardTemplateReward)
            .FirstOrDefaultAsync(t => t.Key == promotionTemplateKey);

        if (template != null)
        {
            template.RewardTemplateReward.Clear();

            if (rewardRns != null)
            {
                var promotions = await _context.Rewards.Where(p => rewardRns.Contains(p.Id)).ToListAsync();

                template.RewardTemplateReward.AddRange(
                    promotions.Select(
                        p => new DataModel.RewardTemplateReward
                        {
                            RewardTemplate = template,
                            Reward = p
                        }));

                template.UpdatedOn = _timeService.UtcNow;

                if (user is not null)
                {
                    template.UpdatedBy = user;
                }

                await _context.SaveChangesAsync();

                return _mapper.Map<RewardTemplateDomainModel>(template);
            }
        }

        throw new InvalidOperationException($"Template with key={promotionTemplateKey} not found");
    }
}

