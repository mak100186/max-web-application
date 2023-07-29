using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Validations;

namespace Kindred.Rewards.Plugin.UniBoostReload.Services;

public interface IFeature
{
    bool Enabled { get; set; }

    void ApplyCreationRules(RewardDomainModel reward);
}

public class ReloadFeature : IFeature
{
    public bool Enabled { get; set; }

    public void ApplyCreationRules(RewardDomainModel reward)
    {
        if (reward == null)
        {
            throw new ArgumentNullException(nameof(reward));
        }

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
}
