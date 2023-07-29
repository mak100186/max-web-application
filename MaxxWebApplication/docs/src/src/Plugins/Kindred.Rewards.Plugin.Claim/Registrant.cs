using System.Reflection;

using Kindred.Infrastructure.Hosting.WebApi.Interfaces;
using Kindred.Rewards.Core.ExceptionHandling;
using Kindred.Rewards.Core.Infrastructure;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Mapping;
using Kindred.Rewards.Plugin.Base.Abstractions;
using Kindred.Rewards.Plugin.Base.Health;
using Kindred.Rewards.Plugin.Claim.Clients.MarketMirror;
using Kindred.Rewards.Plugin.Claim.Exceptions;
using Kindred.Rewards.Plugin.Claim.Services;
using Kindred.Rewards.Plugin.Claim.Services.Strategies;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.Claim;

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

        services.AddScoped<IClaimService, ClaimService>();
        services.AddScoped<IClaimSettleService, ClaimSettleService>();

        services.AddSingleton<IMarketMirrorClient, MarketMirrorClient>();
        services.AddSingleton<IRewardClaimStrategyFactory, RewardClaimStrategyFactory>();

        services.AddScoped<FreeBetClaimStrategy>();
        services.AddScoped<UniBoostClaimStrategy>();
        services.AddScoped<UniBoostReloadClaimStrategy>();
        services.AddScoped<ProfitBoostClaimStrategy>();

        services.AddAutoMapper(Assembly.GetAssembly(typeof(RewardDomainToApiMapping)));

        services.AddCoreServices(registrantMetadata.Config);

        services.AddInfrastructure(registrantMetadata.Config);

        services.AddSwaggerExamplesFromAssemblyOf<Registrant>();

        services.AddSingleton<IApiExceptionVisitor, NotFoundExceptionApiErrorVisitor<ClaimNotFoundException>>();

        services.AddHealthChecks()
            .AddDbContextCheck<RewardsDbContext>();

        return services;
    }

    private static IMvcBuilder AddModule(this IMvcBuilder builder, Type type)
    {
        return builder
            .AddApplicationPart(type.Assembly)
            .AddControllersAsServices();
    }

}
