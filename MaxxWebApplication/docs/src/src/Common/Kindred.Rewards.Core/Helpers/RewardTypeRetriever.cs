using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Helpers;

public static class RewardTypeRetriever
{
    public static IReadOnlyCollection<RewardType> GetReloadableRewardTypes()
    {
        return new List<RewardType>
        {
            RewardType.UniboostReload
        };
    }
}
