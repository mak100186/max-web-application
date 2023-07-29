namespace Kindred.Rewards.Core.Enums;

public enum RewardClaimStatus
{
    /// <summary>
    /// Customer claimed the reward to place a bet
    /// </summary>
    Claimed = 1,

    /// <summary>
    /// Bet is settled and Claim passed though by settlement process
    /// </summary>
    Settled = 2,

    /// <summary>
    /// Claim was revoked, which means it cannot be settled and reward can be claimed again
    /// </summary>
    Revoked = 3
}
