using Kindred.Rewards.Core.WebApi.Enums;

namespace Kindred.Rewards.Core.WebApi.Payloads;

public class DomainRestrictionApiModel
{
    public MultiConfigApiModel MultiConfig { get; set; }
    public OddLimitsApiModel OddLimits { get; set; }
    public IList<string> FilterContestRefs { get; set; }
    public IList<string> FilterContestTypes { get; set; }
    public IList<string> FilterContestCategories { get; set; }
    public ContestStatus? FilterContestStatuses { get; set; }
    public IList<string> FilterOutcomes { get; set; }

    public DomainRestrictionApiModel()
    {
        MultiConfig = new();
        OddLimits = new();
    }
}
