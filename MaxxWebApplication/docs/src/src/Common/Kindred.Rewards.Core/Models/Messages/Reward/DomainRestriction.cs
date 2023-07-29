namespace Kindred.Rewards.Core.Models.Messages.Reward;

public class DomainRestriction
{
    public MultiConfig MultiConfig { get; set; }
    public OddLimits OddLimits { get; set; }
    public IList<string> FilterContestRefs { get; set; }
    public IList<string> FilterContestTypes { get; set; }
    public string FilterContestStatuses { get; set; }
    public IList<string> FilterOutcomes { get; set; }

    public DomainRestriction()
    {
        MultiConfig = new();
        OddLimits = new();
    }
}
