using System.Reflection;
using System.Runtime.Loader;

using Kindred.Rewards.Plugin.Base.Health;

namespace Kindred.Rewards.Plugin.Host.Dependency;

public class AssemblyLoader : AssemblyLoadContext
{
    private readonly IHealthProbe _healthProbe;
    private readonly AssemblyDependencyResolver _resolver;

    public Assembly? Assembly { get; }

    public AssemblyLoader(string pluginPath, string pluginAssemblyName, IHealthProbe healthProbe)
    {
        _healthProbe = healthProbe;
        var fullPathToPlugin = Path.GetFullPath(Path.Combine(pluginPath, pluginAssemblyName));

        _resolver = new(fullPathToPlugin);

        var assemblyName = AssemblyName.GetAssemblyName(fullPathToPlugin);
        Assembly = LoadFromAssemblyName(assemblyName);
        if (Assembly == null)
        {
            var message = $"Assembly \"{assemblyName}\" not found";
            _healthProbe.AddError(nameof(AssemblyLoader), message);
            throw new(message);
        }
    }

    #region Overrides

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
    }

    #endregion
}
