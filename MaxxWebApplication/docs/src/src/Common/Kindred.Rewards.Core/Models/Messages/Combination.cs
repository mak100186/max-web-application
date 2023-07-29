namespace Kindred.Rewards.Core.Models.Messages;

public class Combination
{
    public string Rn { get; set; }
    public IReadOnlyCollection<Selection> Selections { get; set; }
    public CombinationSettlement Settlement { get; set; }
}
