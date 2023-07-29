namespace Kindred.Rewards.Rewards.FunctionalTests.Common.Helpers;

public static class StringExtensions
{
    public static int? ToNullableInt(this string value)
    {
        return int.TryParse(value, out var returnValue)
            ? returnValue
            : null;
    }
}
