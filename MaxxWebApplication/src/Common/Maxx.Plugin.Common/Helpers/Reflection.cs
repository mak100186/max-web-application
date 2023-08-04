using System.Runtime.CompilerServices;

namespace Maxx.Plugin.Common.Helpers;
public class Reflection
{
    public static string GetCurrentMethodName([CallerMemberName] string caller = "")
    {
        return caller;
    }
}
