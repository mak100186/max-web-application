using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace Kindred.Rewards.Plugin.Base.Abstractions;
public class RegistrantMetadata
{
    public IConfigurationRoot Config { get; }

    public RegistrantMetadata(Type type)
    {
        var assembly = Assembly.GetAssembly(type);
        var environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var fileInfo = new FileInfo(assembly.Location);
        var pluginFolder = fileInfo.DirectoryName;
        var pluginFilepath = fileInfo.FullName;

        var parts = pluginFilepath.Split('.');
        var shortName = parts[^2].ToLower();

        var builder = new ConfigurationBuilder();

        builder
        .AddJsonFile(Path.Combine(pluginFolder, $"appsettings.{shortName}.json"), optional: true)
        .AddJsonFile(Path.Combine(pluginFolder, $"appsettings.{shortName}.{environmentVariable}.json"), optional: true);

        builder.AddEnvironmentVariables();

        Config = builder.Build();
    }

}
