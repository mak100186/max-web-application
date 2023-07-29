namespace Kindred.Rewards.Core.Models.RewardConfiguration;

public class LegDefinition
{
    public LegDefinition(int start, int end, decimal value)
    {
        LegRange = new(start, end);
        Value = value;
    }

    //count (not index) of legs i.e. 2-3, 4-6 or 5-20
    public Range<int> LegRange { get; set; }

    public decimal Value { get; set; }

    public bool ContainsLeg(int leg)
    {
        return LegRange.ContainsValue(leg);
    }

    public override string ToString()
    {
        return $"[{LegRange} - {Value}]";
    }
}
