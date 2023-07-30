using System.Reflection;

namespace Maxx.Plugin.Common.Extensions;

public static class AssemblyExtensions
{
    public static string GetAssemblyName(this Type type) => Assembly.GetAssembly(type).FullName;
}
