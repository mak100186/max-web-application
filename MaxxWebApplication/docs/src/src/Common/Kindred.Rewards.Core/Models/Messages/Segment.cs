using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Models.Messages;

public class Segment
{
    public SettlementSegmentStatus Status { get; set; }
    public double Dividend { get; set; }
}
