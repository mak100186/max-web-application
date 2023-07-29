using Kindred.Rewards.Plugin.Base.Health;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kindred.Rewards.Plugin.Base.Abstractions;
public interface IRegistrant
{
    IMvcBuilder Register(IMvcBuilder mvcBuilder, IConfigurationBuilder configurationBuilder, IHealthProbe healthProbe);
}

