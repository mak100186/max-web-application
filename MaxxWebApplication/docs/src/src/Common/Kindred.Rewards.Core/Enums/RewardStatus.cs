namespace Kindred.Rewards.Core.Enums;

public enum RewardStatus
{
    /// <summary>
    /// Customer opted-in, reward is available
    /// </summary>
    Active = 1,

    /// <summary>
    /// Customer used the reward to place a bet
    /// </summary>
    Claimed = 2,

    /// <summary>
    /// Bet is settled
    /// </summary>
    Settled = 3,

    /// <summary>
    /// Reward is expired, cannot be activated anymore
    /// </summary>
    Expired = 4,

    /// <summary>
    /// Reward is cancelled, cannot be activated anymore
    /// </summary>
    Cancelled = 5
}
