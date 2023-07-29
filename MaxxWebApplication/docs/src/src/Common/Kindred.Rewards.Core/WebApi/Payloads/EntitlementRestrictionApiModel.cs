namespace Kindred.Rewards.Core.WebApi.Payloads;

public class EntitlementRestrictionApiModel
{
    public DateTime? StartDateTime { get; set; }

    public DateTime? ExpiryDateTime { get; set; }

    public string ClaimInterval { get; set; }

    public string ClaimAllowedPeriod { get; set; }

    public int? ClaimLimit { get; set; }

    public IList<string> AllowedContestRefs { get; set; }

    public IList<string> AllowedContestTypes { get; set; }

    public IList<string> AllowedOutcomes { get; set; }

    public string TimezoneId { get; set; }
}
