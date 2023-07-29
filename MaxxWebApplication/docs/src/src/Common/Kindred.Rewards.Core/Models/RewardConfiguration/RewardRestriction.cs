using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Models.RewardConfiguration;

public class RewardRestriction
{
    public DateTime StartDateTime { get; set; }

    public DateTime ExpiryDateTime { get; set; }

    public int? ClaimsPerInterval { get; set; }

    public string ClaimInterval { get; set; }

    public string ClaimAllowedPeriod { get; set; }

    public IList<string> AllowedContestRefs { get; set; }

    public IList<string> AllowedContestTypes { get; set; }

    public IList<string> AllowedContestCategories { get; set; }

    public ContestStatus? AllowedContestStatuses { get; set; }

    public IList<string> AllowedOutcomes { get; set; }

    public RewardReloadConfig Reload { get; set; }

    public string TimezoneId { get; set; }
    public OddLimitsConfig OddLimits { get; set; }
}
