using AutoMapper;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Infrastructure.Kafka;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Helpers;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Messages.RewardTemplate;
using Kindred.Rewards.Core.Models.Rewards;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.Reward.Services;


public interface ITemplateCancellationService
{
    Task RemoveCancelledPromotionAsync(string cancelledRewardRn);
}

public class TemplateCancellationService : ITemplateCancellationService
{
    private readonly IMapper _mapper;
    private readonly ILogger<RewardService> _logger;
    private readonly RewardsDbContext _context;
    private readonly ITimeService _timeService;
    private readonly IKafkaProducerManager _producer;

    public TemplateCancellationService(IMapper mapper,
        ILogger<RewardService> logger,
        RewardsDbContext context,
        ITimeService timeService,
        IKafkaProducerManager producer)
    {
        _mapper = mapper;
        _logger = logger;
        _context = context;
        _timeService = timeService;
        _producer = producer;
    }

    public async Task RemoveCancelledPromotionAsync(string cancelledRewardRn)
    {
        var templates = await GetTemplatesByRewardRn(cancelledRewardRn);

        foreach (var template in templates)
        {
            var activeRewardRns = template.Rewards
                .Where(p => p.RewardId != cancelledRewardRn)
                .Select(p => p.RewardId)
                .ToList();

            await UpdateMappingAsync(template.TemplateKey, activeRewardRns);
        }
    }
    private async Task UpdateMappingAsync(string key, List<string> rewardRns)
    {
        var exists = await DoesExistAsync(key);

        if (!exists)
        {
            _logger.LogWarning("Template key {key} not found", key);
            throw new PromotionTemplateNotFoundException(key);
        }

        var promotions = await GetAsync(rewardRns);

        var missing = rewardRns.Where(k => !promotions.Select(p => p.RewardId).Contains(k)).ToList();

        if (missing.Any())
        {
            _logger.LogWarning("Promotion with reward key {rewardRns} not found", missing.ToCsv());
            throw new PromotionsNotFoundException(missing);
        }

        _logger.LogInformation("Binding reward keys {@rewardRns} to template key {@key}", rewardRns, key);

        var updateMappingResult = await SetPromotionToPromotionTemplateAsync(key, rewardRns);

        var evnt = _mapper.Map<RewardTemplateUpdated>(updateMappingResult);

        await Publish(evnt);
    }

    private async Task Publish(RewardTemplateUpdated rewardTemplateMessage)
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

    private async Task<RewardTemplateDomainModel> SetPromotionToPromotionTemplateAsync(string promotionTemplateKey, IEnumerable<string> rewardRns)
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
                        p => new RewardTemplateReward
                        {
                            RewardTemplate = template,
                            Reward = p
                        }));

                template.UpdatedOn = _timeService.UtcNow;

                await _context.SaveChangesAsync();

                return _mapper.Map<RewardTemplateDomainModel>(template);
            }
        }

        throw new InvalidOperationException($"Template with key={promotionTemplateKey} not found");
    }

    private async Task<List<RewardDomainModel>> GetAsync(List<string> rewardRns)
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

    private async Task<bool> DoesExistAsync(string promotionTemplateKey)
    {
        var exist = await _context.RewardTemplates.AnyAsync(t => t.Key == promotionTemplateKey);
        return exist;
    }

    private async Task<IReadOnlyCollection<RewardTemplateDomainModel>> GetTemplatesByRewardRn(string rewardRn)
    {
        var data = await _context.RewardTemplates
            .AsNoTracking()
            .Include(p => p.RewardTemplateReward)
            .ThenInclude(ptp => ptp.Reward)
            .Where(t => t.RewardTemplateReward.Any(p => p.RewardRn == rewardRn))
            .ToListAsync();

        return _mapper.Map<IReadOnlyCollection<RewardTemplateDomainModel>>(data);
    }
}
