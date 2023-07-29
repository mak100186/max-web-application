namespace Kindred.Rewards.Core.Extensions;

public static class DictionaryExtensions
{
    public static T GetValue<T>(this IDictionary<string, string> parameters, string key)
    {
        parameters.TryGetValue(key, out var stringValue);
        return (T)Convert.ChangeType(stringValue, typeof(T));
    }

    public static bool TryGetValue<T>(this IDictionary<string, string> parameters, string key, out T value)
    {
        try
        {
            if (parameters.TryGetValue(key, out var valueString))
            {
                value = (T)Convert.ChangeType(valueString, typeof(T));
                return true;
            }
        }
        catch
        {
            // ignored
        }


        value = default;
        return false;
    }


    public static bool HasAnyKeys<T, TU>(this IDictionary<T, TU> dictionary, IReadOnlyCollection<T> keys)
    {
        return dictionary.Keys.Intersect(keys).Any();
    }
}
