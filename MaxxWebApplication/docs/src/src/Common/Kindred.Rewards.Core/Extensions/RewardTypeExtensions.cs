using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Extensions;

public static class RewardTypeExtensions
{
    public static RewardCategory ToRewardCategory(this RewardType rewardType)
    {
        return rewardType switch
        {
            RewardType.Profitboost => RewardCategory.Boost,
            RewardType.Uniboost => RewardCategory.Boost,
            RewardType.UniboostReload => RewardCategory.Boost,

            RewardType.Freebet => RewardCategory.RiskFree,

            _ => throw new ArgumentOutOfRangeException(nameof(rewardType), rewardType, null)
        };
    }
}
