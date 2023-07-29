namespace Kindred.Rewards.Core.WebApi.Payloads;

public class ClaimItemApiModel
{
    public string RewardRn { get; set; }

    public string RewardId { get; set; }

    public string InstanceId { get; set; }

    public string CustomerId { get; set; }

    public string Status { get; set; }

    public string Class { get; set; }

    public string RewardType { get; set; }

    public string RewardName { get; set; }

    public string CouponRef { get; set; }

    public string BetRef { get; set; }

    public DateTime UpdatedOn { get; set; }

    public long IntervalId { get; set; }

    public int UsageId { get; set; }

    public decimal? RewardPayoutAmount { get; set; }

    public string BetOutcomeStatus { get; set; }

    public RewardRestrictionApiModel Restrictions { get; set; }

    public SettlementApiModel SettlementTerms { get; set; }

    public RewardParameterApiModelBase RewardParameters { get; set; }

    public Dictionary<string, string> Attributes { get; set; }

    public string CurrencyCode { get; set; }
}
