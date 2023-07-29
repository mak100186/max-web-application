namespace Kindred.Rewards.Core.Enums;

public enum BetStatus
{
    /// <summary>
    /// Bet is settled. Mark reward as settled.
    /// </summary>
    Settled,

    /// <summary>
    /// Bet is cancelled. Re-activate the reward
    /// </summary>
    Cancelled
}
