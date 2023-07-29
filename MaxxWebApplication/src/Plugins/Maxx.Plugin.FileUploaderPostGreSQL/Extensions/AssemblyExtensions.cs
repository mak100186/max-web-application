using System.Reflection;

namespace Maxx.Plugin.FileUploaderPostGreSQL.Extensions;

public static class AssemblyExtensions
{
    public static string GetAssemblyName(this Type type) => Assembly.GetAssembly(type).FullName;
}
