namespace Kindred.Rewards.Core.WebApi.Payloads;

public class RewardRestrictionApiModel
{
    public DateTime? StartDateTime { get; set; }

    public DateTime? ExpiryDateTime { get; set; }

    public string ClaimInterval { get; set; }

    public string ClaimAllowedPeriod { get; set; }

    public int? ClaimsPerInterval { get; set; }

    public string TimezoneId { get; set; }
}
