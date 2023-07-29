namespace Kindred.Rewards.Core.Extensions;

public static class StringExtensions
{
    public static bool HasLeadingOrTrailingDot(this string value)
    {
        return value != null && (value.StartsWith(".") || value.EndsWith("."));
    }
    public static int GetInt(this string val)
    {
        return string.IsNullOrWhiteSpace(val) ? 0 : int.TryParse(val, out var i) ? i : 0;
    }
}
