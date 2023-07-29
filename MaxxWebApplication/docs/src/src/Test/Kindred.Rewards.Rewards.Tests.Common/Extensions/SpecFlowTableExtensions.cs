using System.ComponentModel;

using TechTalk.SpecFlow;

namespace Kindred.Rewards.Rewards.Tests.Common.Extensions;

public static class SpecFlowTableExtensions
{
    public static bool ContainsKeyThatIsNotEmpty(this TableRow row, string key)
    {
        return row.ContainsKey(key) && !string.IsNullOrWhiteSpace(row[key]);
    }

    public static string GetValue(this TableRow row, string key, string prefix = null)
    {
        if (row == null)
        {
            return null;
        }

        return row.ContainsKeyThatIsNotEmpty(key)
            ? $"{prefix ?? string.Empty}{row[key]}"
            : null;
    }

    public static T GetValueFromRow<T>(this TableRow row, string key, T defaultValue)
    {
        var value = row.GetValue(key);

        if (value == null)
        {
            return defaultValue;
        }

        var conv = TypeDescriptor.GetConverter(typeof(T));
        return (T)conv.ConvertFrom(value);
    }
}
