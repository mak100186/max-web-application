namespace Kindred.Rewards.Core.Models.Events;

public class CompoundStage
{
    public string Market { get; set; }
    public string ContestType { get; set; }
    public decimal OriginalOdds { get; set; }
    public Selection Selection { get; set; }
}
