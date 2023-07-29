namespace Kindred.Rewards.Core.Extensions;

public static class NumberExtensions
{
    public static bool IsInRangeExclusive(this int subject, int minExclusive, int maxExclusive)
    {
        return subject > minExclusive && subject < maxExclusive;
    }

    public static bool IsInRangeInclusive(this int subject, int minInclusive, int maxInclusive)
    {
        return subject >= minInclusive && subject <= maxInclusive;
    }

    public static int ToInt(this string value, int defaultValue = default)
    {
        return string.IsNullOrWhiteSpace(value) ? defaultValue : int.Parse(value);
    }
}
