using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Infrastructure.Data.DataModels.Extensions;

public static class RewardExtensions
{
    public static string GetStatus(this Reward reward)
    {
        if (reward.IsCancelled)
        {
            return RewardStatus.Cancelled.ToString();
        }

        return reward.ExpiryDateTime <= DateTime.UtcNow ? RewardStatus.Expired.ToString() : RewardStatus.Active.ToString();
    }
}
