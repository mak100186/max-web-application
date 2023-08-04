using System.Runtime.CompilerServices;

using Newtonsoft.Json;

namespace Maxx.Plugin.Common.Extensions;
public static class ObjectExtensions
{
    public static bool IsEqual<T>(this T a, T b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if ((a == null) || (b == null))
        {
            return false;
        }

        var serializedA = JsonConvert.SerializeObject(a);
        var serializedB = JsonConvert.SerializeObject(b);

        return serializedA == serializedB;
    }
}
