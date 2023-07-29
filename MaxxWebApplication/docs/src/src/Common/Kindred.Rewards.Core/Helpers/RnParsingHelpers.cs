using System.Globalization;
using System.Text.RegularExpressions;

namespace Kindred.Rewards.Core.Helpers;

public static class RnParsingHelpers
{
    public static IReadOnlyCollection<string> ValidNamespaces { get; } = new List<string>
        {
            DomainConstants.Rn.Namespaces.Ksp,
            //note: there might be more namespaces as mentioned in the design document, thus the need for a list. 
        };

    public static bool IsValidNamespace(this string namespaceValue)
    {
        return ValidNamespaces.Contains(namespaceValue);
    }

    public static bool IsGuid(this string value, bool shouldCheckWithoutDashes = false)
    {
        var isGuidWithDashes = Guid.TryParseExact(value, "D", out _);

        if (isGuidWithDashes)
        {
            return true;
        }

        if (!shouldCheckWithoutDashes)
        {
            return false;
        }

        return Guid.TryParseExact(value, "N", out var _);
    }

    public static bool IsNumeric(this string value)
    {
        return Regex.IsMatch(value, DomainConstants.Rn.RegExps.UnsignedNumbersWithoutDecimals);
    }

    public static bool IsDate(this string value)
    {
        return DateTime.TryParseExact(value, "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var _);
    }
}
