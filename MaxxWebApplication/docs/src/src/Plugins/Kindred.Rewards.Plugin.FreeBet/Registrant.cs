using Kindred.Rewards.Core.Infrastructure;
using Kindred.Rewards.Plugin.Base.Abstractions;
using Kindred.Rewards.Plugin.Base.Health;
using Kindred.Rewards.Plugin.FreeBet.Mappings;
using Kindred.Rewards.Plugin.FreeBet.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.FreeBet;
public class Registrant : IRegistrant
{
    public IMvcBuilder Register(IMvcBuilder mvcBuilder, IConfigurationBuilder configurationBuilder, IHealthProbe healthProbe)
    {
        mvcBuilder.Services.Register(mvcBuilder, configurationBuilder, healthProbe);

        return mvcBuilder;
    }
}

public static class StaticRegistrant
{
    public static IServiceCollection Register(this IServiceCollection services, IMvcBuilder mvcBuilder, IConfigurationBuilder configurationBuilder, IHealthProbe healthProbe)
    {
        mvcBuilder.AddModule(typeof(Registrant));

        var registrantMetadata = new RegistrantMetadata(typeof(Registrant));
        configurationBuilder.AddConfiguration(registrantMetadata.Config);

        
        services.AddCoreServices(registrantMetadata.Config);

        services.AddInfrastructure(registrantMetadata.Config);

        services.TryAddScoped<IFreeBetService, FreeBetService>();

        services.AddAutoMapper(typeof(FreebetApiToDomainMapping));

        services.AddSwaggerExamplesFromAssemblyOf<Registrant>();

        return services;
    }

    private static IMvcBuilder AddModule(this IMvcBuilder builder, Type type)
    {
        return builder
            .AddApplicationPart(type.Assembly)
            .AddControllersAsServices();
    }

}
