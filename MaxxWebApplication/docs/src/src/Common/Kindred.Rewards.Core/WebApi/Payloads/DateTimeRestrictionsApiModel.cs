namespace Kindred.Rewards.Core.WebApi.Payloads;

public class DateTimeRestrictionsApiModel
{
    public DateTime StartDateTime { get; set; }
    public DateTime ExpiryDateTime { get; set; }
    public string ClaimInterval { get; set; }
    public string ClaimAllowedPeriod { get; set; }
    public int ClaimsPerInterval { get; set; }
    public int RemainingClaimsPerInterval { get; set; }
    public string TimezoneId { get; set; }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            StartDateTime,
            ExpiryDateTime,
            ClaimInterval,
            ClaimAllowedPeriod,
            ClaimsPerInterval,
            RemainingClaimsPerInterval,
            TimezoneId);
    }
}
