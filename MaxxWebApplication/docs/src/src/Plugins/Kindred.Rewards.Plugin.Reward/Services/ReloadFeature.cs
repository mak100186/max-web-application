using FluentAssertions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Validations;

using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.Reward.Services;

public interface IFeature
{
    bool Enabled { get; set; }

    void ApplyCreationRules(RewardDomainModel reward);

    RewardClaimUsageDomainModel GetRemainingClaims(RewardDomainModel reward, RewardClaimUsageDomainModel activeClaims);
}

public class FeatureBase : IFeature
{
    protected FeatureBase(ILogger logger)
    {
        Logger = logger;
    }

    public bool Enabled { get; set; }

    protected ILogger Logger { get; }

    public virtual void ApplyCreationRules(RewardDomainModel reward)
    {
    }

    public virtual RewardClaimUsageDomainModel GetRemainingClaims(RewardDomainModel reward, RewardClaimUsageDomainModel usage)
    {
        return usage?.Evaluate();
    }
}
public class ReloadFeature : FeatureBase
{
    public ReloadFeature(ILogger logger) : base(logger)
    {
    }

    public override void ApplyCreationRules(RewardDomainModel reward)
    {
        reward.Should().NotBeNull($"{nameof(reward)} cannot be null");

        var reload = reward.Terms.Restrictions.Reload;

        if (!Enabled)
        {
            if (reload != null)
            {
                throw new RewardsValidationException($"{FeatureType.Reload} is not supported for {reward.Type}");
            }

            return;
        }

        reward.ThrowIfReloadConfigurationsAreInvalid();
    }

    public override RewardClaimUsageDomainModel GetRemainingClaims(RewardDomainModel reward, RewardClaimUsageDomainModel usage)
    {
        reward.Should().NotBeNull($"{nameof(reward)} cannot be null");
        usage.Should().NotBeNull($"{nameof(usage)} cannot be null");

        return !Enabled || usage.ActiveUsagesCount == 0
            ? usage
            : ShouldExecuteReloadLogic(reward, usage)
                ? CalculateReload(reward, usage)
                : usage.Evaluate();
    }

    private RewardClaimUsageDomainModel CalculateReload(RewardDomainModel reward, RewardClaimUsageDomainModel usage)
    {
        var remaining = reward.Terms.Restrictions.ClaimsPerInterval;
        var nextReload = reward.Terms.Restrictions.Reload.MaxReload;

        foreach (var betStatus in usage.BetOutcomeStatuses)
        {
            switch (betStatus)
            {
                // If bet is lost
                case BetOutcome.Losing:
                case BetOutcome.FullRefund:

                    remaining--;

                    // Keep reloading if MaxReload=null i.e. unlimited
                    if (!nextReload.HasValue)
                    {
                        remaining++;
                    }
                    else if (nextReload > 0)
                    {
                        // top-up reload
                        remaining++;
                        nextReload--;
                    }

                    break;

                // If bet is won
                case BetOutcome.Winning:
                case BetOutcome.WinningAndPartialRefund:

                    remaining--;
                    break;

                // if bet is pending
                default:

                    remaining--;
                    break;
            }
        }

        usage.ClaimRemaining = remaining;

        return usage.Evaluate();
    }

    private bool ShouldExecuteReloadLogic(RewardDomainModel reward, RewardClaimUsageDomainModel usage)
    {
        if (reward.Terms.Restrictions.Reload == null)
        {
            return false;
        }

        // Wait if all claim's bet outcome result is pending
        if (usage.PendingBets >= reward.Terms.Restrictions.ClaimsPerInterval)
        {
            Logger.LogInformation("Waiting for all claims bet outcome. [Reward={@reward}][Usage={@usage}]", reward, usage);
            return false;
        }

        // Stop offering reload if winning bet count reached the limit
        if (usage.WinningBets >= reward.Terms.Restrictions.Reload.StopOnMinimumWinBets)
        {
            Logger.LogInformation("Stop reload. Winning count is reached. [Reward={@reward}][Usage={@usage}]", reward, usage);
            return false;
        }

        return true;
    }
}
