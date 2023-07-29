using Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim.Enums;

namespace Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim;

public class SegmentPayload
{
    public SettlementSegmentStatus Status { get; set; }
    public double Dividend { get; set; }
}
