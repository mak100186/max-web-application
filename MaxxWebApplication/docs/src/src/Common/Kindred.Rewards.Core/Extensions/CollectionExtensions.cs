using System.Diagnostics.CodeAnalysis;

using Kindred.Infrastructure.Core.Extensions.Extensions;

namespace Kindred.Rewards.Core.Extensions;

public static class CollectionExtensions
{
    public static T MinOrDefault<T>([AllowNull] this ICollection<T> collection)
    {
        return collection == null ? default : collection.Any() ? collection.Min() : default;
    }

    public static string ToCsv<T>([AllowNull] this IEnumerable<T> collection, string delimiter)
    {
        return collection == null ? string.Empty : string.Join(delimiter, collection.Select(c => c.ToString()));
    }

    public static bool OrderedSequenceEqual<T>([AllowNull] this IReadOnlyCollection<T> collection, [AllowNull] IReadOnlyCollection<T> compareTo)
    {
        var previous = collection as List<T> ?? new List<T>();
        var next = compareTo as List<T> ?? new List<T>();

        return next.IsNullOrEmpty() && previous.IsNullOrEmpty() || !previous.IsNullOrEmpty() && previous.OrderBy(x => x).SequenceEqual(next.OrderBy(y => y));
    }

    public static void ThrowIfNullOrEmpty<T>(this IReadOnlyCollection<T> arg, string message = "")
    {
        if (arg.IsNullOrEmpty())
        {
            throw new ArgumentNullException(message);
        }
    }

    public static bool ElementsContainEmptyOrWhiteSpace([AllowNull] this IReadOnlyCollection<string> collection)
    {
        if (collection is null)
        {
            return false; //we are allowing collection to be null but not its elements
        }

        return collection.Any(string.IsNullOrEmpty) || collection.Any(t => t.Any(char.IsWhiteSpace));
    }

    public static void ForAll<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (var item in collection)
        {
            action(item);
        }
    }

    public static void AddIfNotPresent<T>(this ICollection<T> collection, T item)
    {
        if (!collection.Contains(item))
        {
            collection.Add(item);
        }
    }
}
