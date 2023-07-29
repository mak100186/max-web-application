using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Models.RewardClaims;

public class RewardClaimUsageDomainModel
{
    public int ActiveUsagesCount { get; set; }

    public int LastUsageId { get; set; }

    public int CurrentUsageId { get; set; }

    public long IntervalId { get; set; }

    public int? ClaimRemaining { get; set; }

    public ICollection<BetOutcome?> BetOutcomeStatuses { get; set; }

    public int PendingBets => BetOutcomeStatuses?.Count(b => b == null) ?? 0;

    public int WinningBets => BetOutcomeStatuses?.Count(b => b is BetOutcome.Winning or BetOutcome.WinningAndPartialRefund) ?? 0;

    public RewardClaimUsageDomainModel Evaluate()
    {
        return ClaimRemaining <= 0 ? null : this;
    }
}
