using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace Maxx.Plugin.FileUploaderPostGreSQL.Extensions;

public static class ConfigurationExtensions
{
    public static IConfiguration LoadConfigurationsFromAssemblyWithType(this Type type, IConfigurationBuilder? builder = default)
    {
        var assembly = Assembly.GetAssembly(type);
        var environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var fileInfo = new FileInfo(assembly.Location);

        var parts = fileInfo.FullName.Split('.');
        var shortName = parts[^2].ToLower();
        var pluginFolder = fileInfo.DirectoryName;

        builder ??= new ConfigurationBuilder();
        return builder
            .AddJsonFile(Path.Combine(pluginFolder, $"appsettings.{shortName}.json"), optional: false)
            .AddJsonFile(Path.Combine(pluginFolder, $"appsettings.{shortName}.{environmentVariable}.json"), optional: false)
            .Build();
    }
}
