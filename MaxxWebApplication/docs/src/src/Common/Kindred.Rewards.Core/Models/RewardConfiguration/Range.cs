namespace Kindred.Rewards.Core.Models.RewardConfiguration;

public class Range<T>
    where T : IComparable<T>
{
    public Range(T start, T end)
    {
        Start = start;
        End = end;
    }

    public T Start { get; set; }

    public T End { get; set; }

    public override string ToString()
    {
        return $"[{Start} - {End}]";
    }

    public bool IsValid()
    {
        return Start.CompareTo(End) < 0;
    }

    public bool ContainsValue(T value)
    {
        return Start.CompareTo(value) <= 0 && value.CompareTo(End) <= 0;
    }

    public bool ContainsRange(Range<T> range)
    {
        return IsValid() && range.IsValid() && ContainsValue(range.Start) && ContainsValue(range.End);
    }
}
