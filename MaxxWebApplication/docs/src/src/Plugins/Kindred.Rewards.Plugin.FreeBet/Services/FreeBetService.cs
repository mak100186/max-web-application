using AutoMapper;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Infrastructure.Kafka;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Infrastructure.Data.Extensions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Messages.Reward;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.Models.Rewards;
using Kindred.Rewards.Core.Validations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Reward = Kindred.Rewards.Core.Infrastructure.Data.DataModels.Reward;

namespace Kindred.Rewards.Plugin.FreeBet.Services;


public interface IFreeBetService
{
    Task<RewardDomainModel> PatchAsync(string id, RewardPatchDomainModel patchModel);
    Task<RewardDomainModel> CreateAsync(RewardDomainModel rewardDomainModel);
    Task<RewardDomainModel> UpdateAsync(RewardDomainModel rewardDomainModel, bool byPassStartDateValidation = false);
}


public class FreeBetService : IFreeBetService
{
    private readonly RewardsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<FreeBetService> _logger;
    private readonly IKafkaProducerManager _producer;

    public FreeBetService(RewardsDbContext context,
        IMapper mapper,
        ILogger<FreeBetService> logger,
        IKafkaProducerManager producer)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _producer = producer;
    }
    public IReadOnlyCollection<string> RequiredParameterKeys => new List<string>
    {
        RewardParameterKey.Amount,
    };

    public IReadOnlyCollection<string> OptionalParameterKeys => new List<string>
    {
        RewardParameterKey.MinStages,
        RewardParameterKey.MaxStages,
        RewardParameterKey.MinCombinations,
        RewardParameterKey.MaxCombinations,
        RewardParameterKey.AllowedFormulae,
        RewardParameterKey.MaxExtraWinnings
    };

    public static IDictionary<string, string> DefaultParameterKeys => new Dictionary<string, string>
    {
        { RewardParameterKey.Amount, "10" },
        { RewardParameterKey.MinStages, "1" },
        { RewardParameterKey.MaxStages, $"{DomainConstants.MaxNumberOfLegsInMulti}" },
        { RewardParameterKey.MinCombinations, $"{DomainConstants.MinNumberOfCombinationsInMulti}" },
        { RewardParameterKey.MaxCombinations, $"{DomainConstants.MaxNumberOfCombinationsInMulti}" },
        { RewardParameterKey.AllowedFormulae, AllowedFormulae.ToCsv() },
        { RewardParameterKey.MaxExtraWinnings, null },
    };

    private static readonly IReadOnlyCollection<BetTypes> s_allowedBetTypes = new List<BetTypes>
    {
        BetTypes.SingleLeg,
        BetTypes.StandardMultiLeg,
        BetTypes.SystemMultiLeg
    };

    public static readonly IReadOnlyCollection<string> AllowedFormulae = FormulaExtensions.GetFormulaeForBetTypes(s_allowedBetTypes);

    public async Task<RewardDomainModel> CreateAsync(RewardDomainModel rewardDomainModel)
    {
        ValidateAndInitialise(rewardDomainModel);

        _logger.LogInformation("Creating reward {@rewardDomainModel}", rewardDomainModel);

        rewardDomainModel.CreatedOn = DateTime.UtcNow;
        rewardDomainModel.UpdatedOn = DateTime.UtcNow;

        var foundTags = await ExtractTags(rewardDomainModel);


        var data = _mapper.Map<Reward>(rewardDomainModel);
        data.RewardTags = foundTags.Select(t => new RewardTag
        {
            Reward = data,
            Tag = t,
        }).ToList();

        _context.Rewards.Add(data);
        OnDataAdded(data);
        await _context.SaveChangesAsync();

        var rewardCreatedEvent = _mapper.Map<RewardCreated>(rewardDomainModel);

        await PublishRewardCreated(rewardCreatedEvent);

        return rewardDomainModel;
    }

    public async Task<RewardDomainModel> PatchAsync(string id, RewardPatchDomainModel patchModel)
    {
        var rewardToUpdate = await GetAsync(id);

        ThrowIfRewardDoesNotExist(rewardToUpdate, rewardToUpdate.RewardId);
        ThrowIfRewardNotLockedAndNotStarted(rewardToUpdate);

        if (patchModel.Comments != null)
        {
            rewardToUpdate.Comments = patchModel.Comments;
        }

        if (patchModel.Name != null)
        {
            rewardToUpdate.Name = patchModel.Name;
        }

        return await UpdateInternalAsync(rewardToUpdate);
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

    private static List<RewardTag> CreateRewardTags(IEnumerable<Tag> tags, Reward data)
    {
        return tags.Select(t => new RewardTag { Reward = data, Tag = t, }).ToList();
    }


    private void OnDataUpdated(RewardDomainModel existingData, Reward updatedData)
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

    private async Task<RewardDomainModel> GetAsync(string id)
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

    private void ThrowIfRewardNotLockedAndNotStarted(RewardDomainModel reward)
    {
        if (reward.IsLocked == false && reward.HasRewardStarted() == false)
        {
            _logger.LogWarning("Reward [Id:{rewardId}] is not locked and reward has not started. Therefore reward cannot be updated.", reward.RewardId);
            throw new RewardsValidationException($"Reward [Id:{reward.RewardId}] is not locked and reward has not started. Therefore reward cannot be updated.");
        }
    }

    private void ThrowIfRewardDoesNotExist(RewardDomainModel reward, string id)
    {
        if (reward == null)
        {
            _logger.LogWarning("Could not find a reward with reward id {id}", id);
            throw new RewardNotFoundException(id);
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

    private void ValidateAndInitialise(RewardDomainModel reward)
    {
        reward.ThrowIfTagsContainEmptyOrWhiteSpace();

        reward.ThrowIfRequiredParameterKeysAreMissing(RequiredParameterKeys);

        reward.ThrowIfInvalidParameterKeysAreFound(RequiredParameterKeys, OptionalParameterKeys);

        reward.SetDefaultMultiConfigRewardParameters(DefaultParameterKeys);

        reward.ThrowIfAmountIsNotValid();

        reward.ThrowIfOptionalDecimalParameterWasInvalid(RewardParameterKey.MaxExtraWinnings);

        reward.ThrowIfFormulaeAreInvalid(RewardParameterKey.AllowedFormulae);

        var betTypes = reward.Terms.RewardParameters.GetApplicableBetTypes();

        reward.ThrowIfMultiConfigsAreInvalid(betTypes, s_allowedBetTypes);

        reward.Terms.SettlementTerms = new() { ReturnStake = false };
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

    private static Tag CreateTag(string tagName)
    {
        var dataModel = new Tag
        {
            Name = tagName,
            Comments = DomainConstants.AutoCreatedTagComments
        };

        return dataModel;
    }

    private void OnDataAdded(Reward data)
    {
        var audit = new AuditReward
        {
            RewardId = data.Id,
            Activity = nameof(AuditActivity.BonusCreated),
            CreatedBy = data.CreatedBy
        };

        _context.AuditRewards.Add(audit);
    }
}
