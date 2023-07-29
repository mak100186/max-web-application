using Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim.Enums;

namespace Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim;

public class CombinationSettlementPayload
{
    public IEnumerable<SegmentPayload> Segments { get; set; }
    public SettlementCombinationStatus Status { get; set; }
    public double Payoff { get; set; }
}
