using System.Linq.Expressions;

using AutoMapper;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Infrastructure.Hosting.WebApi.Extensions;
using Kindred.Infrastructure.Hosting.WebApi.Sorting;
using Kindred.Infrastructure.Kafka;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels.Extensions;
using Kindred.Rewards.Core.Infrastructure.Data.Extensions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Messages.Reward;
using Kindred.Rewards.Core.Models.Rewards;

using LinqKit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.Reward.Services;


public interface IRewardService
{
    Task<RewardDomainModel> CreateAsync(RewardDomainModel rewardDomainModel);
    Task<RewardDomainModel> UpdateAsync(RewardDomainModel rewardDomainModel, bool byPassStartDateValidation = false);
    Task<PagedResponse<RewardDomainModel>> GetAllAsync(RewardFilterDomainModel filter, PagedRequest pagination);
    Task<RewardDomainModel> GetAsync(string id);
    Task CancelAsync(string id, string? reason);
    RewardInfoDomainModel GetDefaults(string rewardType);
    Task LockAsync(string id);
}


public class RewardService : IRewardService
{
    private readonly RewardsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<RewardService> _logger;
    private readonly IKafkaProducerManager _producer;
    private readonly IRewardCreationStrategyFactory _creationStrategyFactory;

    public RewardService(
        IMapper mapper,
        ILogger<RewardService> logger,
        RewardsDbContext context,
        IRewardCreationStrategyFactory creationStrategyFactory,
        IKafkaProducerManager producer)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _creationStrategyFactory = creationStrategyFactory;
        _producer = producer;
    }

    public async Task<RewardDomainModel> CreateAsync(RewardDomainModel rewardDomainModel)
    {
        ValidateAndInitialise(rewardDomainModel);

        _logger.LogInformation("Creating reward {@rewardDomainModel}", rewardDomainModel);

        rewardDomainModel.CreatedOn = DateTime.UtcNow;
        rewardDomainModel.UpdatedOn = DateTime.UtcNow;

        var foundTags = await ExtractTags(rewardDomainModel);

        var data = _mapper.Map<Core.Infrastructure.Data.DataModels.Reward>(rewardDomainModel);
        data.RewardTags = CreateRewardTags(foundTags, data);

        _context.Rewards.Add(data);
        OnDataAdded(data);
        await _context.SaveChangesAsync();

        var rewardCreatedEvent = _mapper.Map<RewardCreated>(rewardDomainModel);

        await PublishRewardCreated(rewardCreatedEvent);

        return rewardDomainModel;
    }

    private static List<RewardTag> CreateRewardTags(IEnumerable<Tag> tags, Core.Infrastructure.Data.DataModels.Reward data)
    {
        return tags.Select(t => new RewardTag { Reward = data, Tag = t, }).ToList();
    }

    private async Task<List<Tag>> ExtractTags(RewardDomainModel domain)
    {
        var foundTags = new List<Tag>();
        if (domain.Tags.IsNotNullAndNotEmpty())
        {
            var predicate = SearchPredicateBuilder.BuildSpecificTagsPredicate(domain.Tags);
            var repositoryTags = await _context.Tags.Where(predicate).ToListAsync();

            domain.Tags = domain.Tags.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
            foreach (var tagName in domain.Tags)
            {
                var tag = repositoryTags.Find(tag => tag.Name == tagName);

                if (tag == null)
                {
                    tag = CreateTag(tagName);
                    repositoryTags.Add(tag);
                }

                foundTags.Add(tag);
            }
        }
        return foundTags;
    }

    private async Task<RewardDomainModel> UpdateInternalAsync(RewardDomainModel domain)
    {

        var foundTags = await ExtractTags(domain);

        var repo = _context.Rewards
            .Include(b => b.RewardTags).ThenInclude(bt => bt.Tag);

        var data = repo.First(bonus => bonus.Id == domain.RewardId);

        var dataBeforeUpdate = _mapper.Map(data, new RewardDomainModel());
        _mapper.Map(domain, data);

        data.RewardTags = CreateRewardTags(foundTags, data);

        var dataAfterUpdate = data;
        OnDataUpdated(dataBeforeUpdate, dataAfterUpdate);
        await _context.SaveChangesAsync();

        var savedDomain = _mapper.Map<RewardDomainModel>(dataAfterUpdate);
        return savedDomain;
    }

    private void OnDataUpdated(RewardDomainModel existingData, Core.Infrastructure.Data.DataModels.Reward updatedData)
    {
        var activityName = !existingData.IsCancelled && updatedData.IsCancelled ? nameof(AuditActivity.BonusCancelled) : nameof(AuditActivity.BonusUpdated);
        var audit = new AuditReward
        {
            RewardId = updatedData.Id,
            Activity = activityName,
            CreatedBy = updatedData.CreatedBy
        };
        _context.AuditRewards.Add(audit);
    }

    public async Task<RewardDomainModel> UpdateAsync(RewardDomainModel reward, bool byPassStartDateValidation = false)
    {
        ValidateAndInitialise(reward);

        var existingReward = await GetAsync(reward.RewardId);
        ThrowIfRewardDoesNotExist(existingReward, reward.RewardId);
        ThrowIfTryingToUpdateCustomerId(existingReward, reward);
        ThrowIfLocked(existingReward);

        if (!byPassStartDateValidation)
        {
            ThrowIfStartDateHasElapsed(existingReward);
        }

        _logger.LogInformation("Updating reward {@reward}", reward);

        reward.IsSystemGenerated = existingReward.IsSystemGenerated;
        reward.IsLocked = existingReward.IsLocked;

        var updatedReward = await UpdateInternalAsync(reward);

        var rewardUpdatedEvent = _mapper.Map<RewardUpdated>(updatedReward);

        await PublishRewardUpdated(rewardUpdatedEvent);

        return updatedReward;
    }

    public async Task<RewardDomainModel> GetAsync(string id)
    {
        var data = await _context.Rewards
            .Include(b => b.RewardTags).ThenInclude(bt => bt.Tag)
            .Include(p => p.RewardTemplateReward).ThenInclude(pt => pt.RewardTemplate)
            .AsNoTracking()
            .FirstOrDefaultAsync(reward => reward.Id == id);

        // Make sure there are no circular dependencies otherwise auto-mapper does recursive mapping
        if (data is { RewardTemplateReward: { } })
        {
            foreach (var promotionTemplate in data.RewardTemplateReward.Select(pt => pt.RewardTemplate))
            {
                promotionTemplate.RewardTemplateReward.ForEach(ptp => ptp.Reward = null);
            }
        }

        var reward = _mapper.Map<RewardDomainModel>(data);

        ThrowIfRewardDoesNotExist(reward, id);

        return reward;
    }

    public async Task<PagedResponse<RewardDomainModel>> GetAllAsync(RewardFilterDomainModel filter, PagedRequest pagination)
    {
        _logger.LogDebug("Fetch rewards for filter criteria {@filter}", filter);

        var rewards = await GetRewardsByFilterAsync(filter, pagination);

        return rewards;
    }

    public async Task<PagedResponse<RewardDomainModel>> GetRewardsByFilterAsync(RewardFilterDomainModel filter, PagedRequest pagination)
    {
        var predicate = BuildRewardQuery(filter);

        var query = _context.Rewards
            .AsNoTracking()
            .Include(a => a.Audits)
            .Include(a => a.RewardTags).ThenInclude(t => t.Tag)
            .Include(r => r.RewardTemplateReward).ThenInclude(t => t.RewardTemplate)
            .Where(predicate)
            .Where(r => !r.IsSystemGenerated);

        return await ApplySortingToRewardsAsync(filter, pagination, query);
    }

    public async Task CancelAsync(string id, string? reason)
    {
        var reward = await GetAsync(id);

        reward.IsCancelled = true;
        reward.CancellationReason = reason;

        _logger.LogInformation("Cancelling reward {id} for reason {reason}", id, reason);

        var customerId = reward.CustomerId;

        var updatedReward = await UpdateInternalAsync(reward);

        var rewardUpdatedEvent = _mapper.Map<RewardUpdated>(updatedReward);

        await PublishRewardUpdated(rewardUpdatedEvent);
    }

    public async Task LockAsync(string id)
    {
        var reward = await GetAsync(id);

        if (!reward.IsSystemGenerated)
        {
            throw new RewardsValidationException("Can not lock a reward different than system generated");
        }
        reward.IsLocked = true;

        _logger.LogInformation("Locking reward {id}", id);

        var updatedReward = await UpdateInternalAsync(reward);
    }

    public RewardInfoDomainModel GetDefaults(string rewardType)
    {
        var type = rewardType.ToEnumSafe<RewardType>();
        if (type.HasValue)
        {
            var strategy = _creationStrategyFactory.GetRewardCreationStrategy(type.Value);

            return new()
            {
                OptionalParameterKeys = strategy.OptionalParameterKeys,
                RequiredParameterKeys = strategy.RequiredParameterKeys
            };
        }

        return null;
    }

    private static Expression<Func<Core.Infrastructure.Data.DataModels.Reward, bool>> BuildRewardQuery(RewardFilterDomainModel filter)
    {
        var whereClause = PredicateBuilder.New<Core.Infrastructure.Data.DataModels.Reward>(true);

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            whereClause = whereClause.And(p => p.Name.ToLower().Contains(filter.Name.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(filter.CustomerId))
        {
            whereClause = whereClause.And(r => r.CustomerId.Equals(filter.CustomerId));
        }

        if (!string.IsNullOrWhiteSpace(filter.RewardType.ToString()))
        {
            whereClause = whereClause.And(r => r.RewardType == filter.RewardType.ToString());
        }

        if (!string.IsNullOrWhiteSpace(filter.Jurisdiction))
        {
            whereClause = whereClause.And(r => r.Jurisdiction == filter.Jurisdiction);
        }

        if (!string.IsNullOrWhiteSpace(filter.Country))
        {
            whereClause = whereClause.And(r => r.CountryCode == filter.Country);
        }

        if (!string.IsNullOrWhiteSpace(filter.Brand))
        {
            whereClause = whereClause.And(r => r.Brand == filter.Brand);
        }

        if (!string.IsNullOrWhiteSpace(filter.State))
        {
            whereClause = whereClause.And(r => r.State == filter.State);
        }

        if (filter.UpdatedDateFromUtc.HasValue)
        {
            whereClause = whereClause.And(r => r.UpdatedOn >= filter.UpdatedDateFromUtc.Value);
        }

        if (filter.UpdatedDateToUtc.HasValue)
        {
            whereClause = whereClause.And(r => r.UpdatedOn <= filter.UpdatedDateToUtc.Value);
        }

        if (!filter.IncludeCancelled)
        {
            whereClause = whereClause.And(r => r.IsCancelled == false);
        }

        if (!filter.IncludeExpired)
        {
            whereClause = whereClause.And(r => r.ExpiryDateTime > DateTime.UtcNow);
        }

        if (!filter.IncludeActive)
        {
            var currentDateTime = DateTime.UtcNow;
            whereClause = whereClause.And(r => r.StartDateTime > currentDateTime ||
                                               r.ExpiryDateTime < currentDateTime ||
                                               r.IsCancelled == true);
        }

        return whereClause;
    }

    private void ThrowIfRewardDoesNotExist(RewardDomainModel reward, string id)
    {
        if (reward == null)
        {
            _logger.LogWarning("Could not find a reward with reward id {id}", id);
            throw new RewardNotFoundException(id);
        }
    }

    private void ValidateAndInitialise(RewardDomainModel reward)
    {
        var strategy = _creationStrategyFactory.GetRewardCreationStrategy(reward.Type);

        strategy.ValidateAndInitialise(reward);
    }

    private void OnDataAdded(Core.Infrastructure.Data.DataModels.Reward data)
    {
        var audit = new AuditReward
        {
            RewardId = data.Id,
            Activity = nameof(AuditActivity.BonusCreated),
            CreatedBy = data.CreatedBy
        };

        _context.AuditRewards.Add(audit);
    }

    private static Tag CreateTag(string tagName)
    {
        var dataModel = new Tag
        {
            Name = tagName,
            Comments = DomainConstants.AutoCreatedTagComments
        };

        return dataModel;
    }

    private void ThrowIfStartDateHasElapsed(RewardDomainModel reward)
    {
        if (reward.HasRewardStarted())
        {
            var message = $"Reward [Id:{reward.RewardId}] is locked as the UTC start date/time for reward is: {reward.Terms.Restrictions.StartDateTime}";
            _logger.LogWarning(message);
            throw new RewardsValidationException(message);
        }
    }

    private void ThrowIfLocked(RewardDomainModel existingReward)
    {
        if (existingReward.IsLocked)
        {
            var error = $"Trying to update reward {existingReward.RewardId} which is locked.";
            _logger.LogWarning(error);
            throw new RewardsValidationException(error);
        }
    }

    private async Task PublishRewardUpdated(RewardUpdated rewardEvent)
    {
        try
        {
            await _producer.SendAsync(DomainConstants.TopicProfileRewardUpdates, rewardEvent, rewardEvent.RewardRn, Guid.NewGuid().ToString());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ERROR PUBLISHING {@rewardEvent} ", rewardEvent);
            throw;
        }
    }

    private async Task PublishRewardCreated(RewardCreated rewardEvent)
    {
        try
        {
            await _producer.SendAsync(DomainConstants.TopicProfileRewardUpdates, rewardEvent, rewardEvent.RewardRn, Guid.NewGuid().ToString());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ERROR PUBLISHING {@rewardEvent} ", rewardEvent);
            throw;
        }
    }

    private void ThrowIfTryingToUpdateCustomerId(RewardDomainModel existingReward, RewardDomainModel rewardToUpdate)
    {
        if (existingReward.CustomerId == rewardToUpdate.CustomerId)
        {
            return;
        }

        var error = $"CustomerId of an existing reward cannot be updated. Existing customerId = {existingReward.CustomerId}; new customerId = {rewardToUpdate.CustomerId}";
        _logger.LogWarning(error);
        throw new RewardsValidationException(error);
    }

    private async Task<PagedResponse<RewardDomainModel>> ApplySortingToRewardsAsync(SortRequest filter,
        PagedRequest pagination, IQueryable<Core.Infrastructure.Data.DataModels.Reward> query)
    {
        IEnumerable<Core.Infrastructure.Data.DataModels.Reward> data;

        // Status is a computed field so we need to sort in-memory
        if (filter.SortBy != null && filter.SortBy.Contains(SortableRewardFields.Status.ToString(),
                StringComparison.InvariantCultureIgnoreCase))
        {
            data = await query.ToListAsync();
            if (filter.SortBy[0] == DomainConstants.SortByDescendingIndicator)
            {
                data = data
                    .OrderByDescending(x => x.GetStatus())
                    .ThenBy(x => x.StartDateTime)
                    .ThenBy(x => x.Name)
                    .AsQueryable()
                    .SkipWithPaging(pagination).Queryable
                    .AsNoTracking();
            }
            else
            {
                data = data
                    .OrderBy(x => x.GetStatus())
                    .ThenBy(x => x.StartDateTime)
                    .ThenBy(x => x.Name)
                    .AsQueryable()
                    .SkipWithPaging(pagination).Queryable
                    .AsNoTracking();
            }
        }
        else
        {
            data = query
                .SkipWithPaging(pagination, filter).Queryable
                .AsNoTracking();
        }

        return new()
        {
            ItemCount = query.Count(),
            Items = _mapper.Map<List<RewardDomainModel>>(data),
            Limit = pagination.Limit,
            Offset = pagination.Offset
        };
    }
}
