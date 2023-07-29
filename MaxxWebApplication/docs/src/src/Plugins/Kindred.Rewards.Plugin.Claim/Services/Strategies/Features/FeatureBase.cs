using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardClaims;

using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.Claim.Services.Strategies.Features;

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
