using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maxx.Plugin.Shared.Abstractions;

public interface IPluginRegistrant
{
    IMvcBuilder Register(IMvcBuilder mvcBuilder, IConfiguration hostConfiguration, IConfigurationBuilder configurationBuilder, IHealthProbe healthProbe);
}
