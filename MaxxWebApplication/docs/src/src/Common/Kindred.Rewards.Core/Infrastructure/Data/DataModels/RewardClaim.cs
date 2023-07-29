namespace Kindred.Rewards.Core.Infrastructure.Data.DataModels;

// fields CustomerId, RewardId, IntervalId, UsageId 
// are used for concurrency control on RewardClaim creation
public class RewardClaim : BaseEditableDataModel<int>
{
    public string RewardId { get; set; }

    public string CustomerId { get; set; }

    public string CouponRn { get; set; }

    public string BetRn { get; set; }

    public string Status { get; set; }

    public string TermsJson { get; set; }

    public string BetJson { get; set; }

    public long IntervalId { get; set; }

    public int UsageId { get; set; }

    public string InstanceId { get; set; }

    public decimal? RewardPayoutAmount { get; set; }

    public string RewardName { get; set; }

    public string RewardType { get; set; }

    public string BetOutcomeStatus { get; set; }

    public string CurrencyCode { get; set; }

    public string CreatedBy { get; set; }
    public RewardClaimPayoffMetadata PayoffMetadata { get; set; }
    public ICollection<CombinationRewardPayoff> CombinationRewardPayoffs { get; } = new List<CombinationRewardPayoff>();
}
