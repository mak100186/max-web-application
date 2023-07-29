namespace Kindred.Rewards.Core.Extensions;

public static class CommaDelimitedHelper
{
    public static List<string> ExtractValues(this string value)
    {
        return value?
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList()
            ?? new List<string>();
    }
}
