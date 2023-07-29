using AutoMapper;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Infrastructure.Data.Extensions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Events;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Validations;
using Kindred.Rewards.Plugin.Claim.Models.Dto;
using Kindred.Rewards.Plugin.Claim.Services.Strategies.Features;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.Claim.Services.Strategies.Base;

public interface IRewardStrategy
{
    Task ProcessClaimAsync(RewardBet rewardBet);
    RewardClaimUsageDomainModel CalculateRemainingClaims(RewardDomainModel reward, RewardClaimUsageDomainModel usage);
    ClaimResultDomainModel ValidateClaim(RewardClaimDomainModel reward, BatchClaimItemDomainModel claim);
    Task<SettleClaimResultDto> ProcessClaimAsync(SettleClaimParameterDto settleClaim);
    Task<ClaimResultDto> CalculateClaimResultAsync(ClaimParameterDto claimParameterDto);

}

public class RewardClaimStrategyBase : IRewardStrategy
{
    private readonly RewardsDbContext _context;
    private readonly IMapper _mapper;
    protected readonly ILogger Logger;

    public string RewardName { get; protected set; }

    protected RewardClaimStrategyBase(RewardsDbContext context,
        IMapper mapper,
        ILogger logger)
    {
        _context = context;
        _mapper = mapper;
        Logger = logger;

        // Features are the common behaviors that can be applied to any reward
        Features = new()
        {
            { FeatureType.Reload, new ReloadFeature(logger) }
        };
    }

    public Dictionary<FeatureType, IFeature> Features { get; }

    public async Task<SettleClaimResultDto> ProcessClaimAsync(SettleClaimParameterDto settleClaim)
    {
        var rewardPaymentAmount = await CalculateRewardPaymentAmountAsync(settleClaim);


        var claim = _context.RewardClaims
            .First(claim => claim.InstanceId == settleClaim.RewardClaim.InstanceId);

        claim.BetOutcomeStatus = settleClaim.BetOutcome?.ToString();
        claim.CombinationRewardPayoffs.Add(new()
        {
            BetPayoff = settleClaim.BetPayoff,
            BetRn = settleClaim.BetRn,
            BetStake = settleClaim.BetStake,
            ClaimInstanceId = settleClaim.RewardClaim.InstanceId,
            CombinationPayoff = rewardPaymentAmount.Payoff,
            CombinationRn = settleClaim.CombinationRn
        });

        await _context.SaveChangesAsync();

        return new() { Payoff = rewardPaymentAmount.Payoff };
    }


    public virtual async Task ProcessClaimAsync(RewardBet rewardBet)
    {
        if (ShouldCalculateRewardPayout(rewardBet))
        {
            await CalculateRewardPaymentAmountAsync(rewardBet);
        }
    }

    public virtual async Task<ClaimResultDto> CalculateClaimResultAsync(ClaimParameterDto claimParameterDto)
    {
        return new();
    }

    public virtual RewardClaimUsageDomainModel CalculateRemainingClaims(RewardDomainModel reward, RewardClaimUsageDomainModel usage)
    {
        var remainingClaims = usage;
        remainingClaims.ClaimRemaining = reward.Terms.Restrictions.ClaimsPerInterval - usage.ActiveUsagesCount;

        foreach (var feature in Features.Where(f => f.Value.Enabled))
        {
            remainingClaims = feature.Value.GetRemainingClaims(reward, usage);
        }

        return remainingClaims?.Evaluate();
    }

    public virtual ClaimResultDomainModel ValidateClaim(
        RewardClaimDomainModel reward,
        BatchClaimItemDomainModel claim)
    {
        var claimResult = new ClaimResultDomainModel
        {
            Claim = reward
        };

        if (!CommonClaimValidations.ValidateMinimumCompoundOdds(reward, claim, claimResult))
        {
            return claimResult;
        }

        if (!CommonClaimValidations.ValidateMinimumStageOdds(reward, claim, claimResult))
        {
            return claimResult;
        }

        return claimResult;
    }

    /// <summary>
    /// <see cref="CalculateRewardPaymentAmountAsync"/> is executed only when this method return true
    /// </summary>
    /// <param name="rewardBet">Bet that contains reward claim</param>
    /// <returns>If true, then CalculateRewardPaymentAmountAsync is executed</returns>
    protected virtual bool ShouldCalculateRewardPayout(RewardBet rewardBet)
    {
        rewardBet.StakeDeduction = 0;
        if (!rewardBet.BetOutcome.IsLosingBet())
        {
            return true;
        }

        rewardBet.RewardPaymentAmount = null;
        return false;

    }

    protected virtual async Task<bool> CalculateRewardPaymentAmountAsync(RewardBet rewardBet)
    {
        await CancelUnclaimedRewardsBySourceIdAsync(rewardBet.ClaimInstanceId, $"{RewardName} Reversed");

        Logger.LogInformation($"{RewardName} is reversed for bet {{@rewardBet}}", rewardBet);

        return true;
    }

    protected virtual async Task<RewardPaymentAmountResultDto> CalculateRewardPaymentAmountAsync(SettleClaimParameterDto settleClaim)
    {
        return new();
    }


    // do this
    public async Task CancelUnclaimedRewardsBySourceIdAsync(string sourceInstanceId, string cancellationReason)
    {

        var rewards = await GetRewardsBySourceInstanceIdAsync(sourceInstanceId);

        foreach (var reward in rewards)
        {
            if (reward.Terms.Restrictions.ExpiryDateTime.NotInTheFuture())
            {
                Logger.LogInformation("Reward {@reward} cannot be cancelled because it is expired", reward);
                continue;
            }

            if (await HasClaimedOrSettledAsync(reward.RewardId))
            {
                Logger.LogInformation("Reward {@reward} cannot be cancelled because it is claimed", reward);
                continue;
            }

            reward.IsCancelled = true;
            reward.CancellationReason = cancellationReason;

            var dataAfterUpdate = await UpdateAsync(reward);
            await _context.SaveChangesAsync();

            Logger.LogInformation("Reward {@reward} is successfully cancelled", dataAfterUpdate);
        }
    }

    public async Task<IReadOnlyCollection<RewardDomainModel>> GetRewardsBySourceInstanceIdAsync(
        string sourceInstanceId)
    {
        IReadOnlyCollection<Reward> data = await _context.Rewards
                           .Where(x => x.SourceInstanceId == sourceInstanceId && !x.IsCancelled)
                           .ToListAsync();

        var domain = _mapper.Map<IReadOnlyCollection<RewardDomainModel>>(data);

        return domain;
    }

    public async Task<bool> HasClaimedOrSettledAsync(string rewardRn)
    {
        return await _context.RewardClaims
            .AnyAsync(claim => claim.RewardId == rewardRn &&
                               (claim.Status == nameof(RewardClaimStatus.Claimed) || claim.Status == nameof(RewardClaimStatus.Settled)));
    }

    public async Task<Reward> UpdateAsync(RewardDomainModel domain)
    {
        List<Tag> foundTags = new();
        if (domain.Tags.IsNotNullAndNotEmpty())
        {
            var predicate = SearchPredicateBuilder.BuildSpecificTagsPredicate(domain.Tags);
            var repositoryTags = await _context.Tags.Where(predicate).ToListAsync();

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

        var repo = _context.Rewards
            .Include(b => b.RewardTags).ThenInclude(bt => bt.Tag);

        var data = repo.First(bonus => bonus.Id == domain.RewardId);

        var dataBeforeUpdate = _mapper.Map(data, new Reward());
        _mapper.Map(domain, data);

        data.RewardTags = foundTags.Select(t => new RewardTag
        {
            Reward = data,
            Tag = t,
        }).ToList();

        OnDataUpdated(dataBeforeUpdate, data);
        return data;
    }

    protected Tag CreateTag(string tagName)
    {
        Tag dataModel = new()
        {
            Name = tagName,
            Comments = DomainConstants.AutoCreatedTagComments
        };

        return dataModel;
    }

    protected void OnDataUpdated(Reward existingData, Reward updatedData)
    {
        var hasBeenCancelled = !existingData.IsCancelled && updatedData.IsCancelled;
        if (hasBeenCancelled)
        {
            AuditReward audit = new()
            {
                RewardId = existingData.Id,
                Activity = nameof(AuditActivity.BonusCancelled),
                CreatedBy = updatedData.CreatedBy
            };
            _context.AuditRewards.Add(audit);
        }
    }
}
