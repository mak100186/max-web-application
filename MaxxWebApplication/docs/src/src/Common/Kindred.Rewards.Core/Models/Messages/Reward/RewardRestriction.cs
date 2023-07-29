namespace Kindred.Rewards.Core.Models.Messages.Reward;

public class RewardRestriction
{
    public DateTime? StartDateTime { get; set; }

    public DateTime? ExpiryDateTime { get; set; }

    public string ClaimInterval { get; set; }

    public string ClaimAllowedPeriod { get; set; }

    public int? ClaimsPerInterval { get; set; }

    public string TimezoneId { get; set; }
}
