namespace Kindred.Rewards.Core.Models.Messages;

public class RewardClaim : IMessageInstance
{
    public string Type { get; set; }

    public string InstanceId { get; set; }

    public string CustomerId { get; set; }

    public string Status { get; set; }

    public string RewardName { get; set; }

    public string CouponRef { get; set; }

    public string BetRef { get; set; }

    /// <summary>
    /// Gets or sets the pay out amount when a reward is settled and used for odds boost for now
    /// </summary>
    public decimal? RewardPayoutAmount { get; set; }

    public RewardTerms Terms { get; set; }

    public decimal StakeDeduction { get; set; } = 0;

    public string RewardRn { get; set; }

    public string RewardId { get; set; }

    public DateTime UpdatedOn { get; set; }

    public string CurrencyCode { get; set; }
}
