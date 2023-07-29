using FluentValidation;

using Kindred.Infrastructure.Hosting.WebApi.Interfaces;
using Kindred.Rewards.Core.ExceptionHandling;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Infrastructure;
using Kindred.Rewards.Core.Mapping.Converters;
using Kindred.Rewards.Core.WebApi.Validation;
using Kindred.Rewards.Plugin.Base.Abstractions;
using Kindred.Rewards.Plugin.Base.Health;
using Kindred.Rewards.Plugin.Reward.Mappings;
using Kindred.Rewards.Plugin.Reward.Models;
using Kindred.Rewards.Plugin.Reward.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.Reward;
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

        services.AddValidatorsFromAssemblyContaining(typeof(ValidatorBase<>), ServiceLifetime.Singleton);
        services.AddValidatorsFromAssemblyContaining(typeof(RewardRequestValidator), ServiceLifetime.Singleton);

        var registrantMetadata = new RegistrantMetadata(typeof(Registrant));
        configurationBuilder.AddConfiguration(registrantMetadata.Config);

        
        services.AddCoreServices(registrantMetadata.Config);

        services.AddInfrastructure(registrantMetadata.Config);

        services.TryAddScoped<IRewardService, RewardService>();
        services.TryAddScoped<ITemplateCancellationService, TemplateCancellationService>();

        services.AddSingleton<IRewardCreationStrategyFactory, RewardCreationStrategyFactory>();

        services.AddSingleton<FreeBetCreationStrategy>();
        services.AddSingleton<UniBoostCreationStrategy>();
        services.AddSingleton<UniBoostReloadCreationStrategy>();
        services.AddSingleton<ProfitBoostCreationStrategy>();

        services.AddAutoMapper(typeof(RewardApiToDomainMapping));

        services.AddSwaggerExamplesFromAssemblyOf<Registrant>();

        mvcBuilder.AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new RewardParameterApiModelBaseConverter()));

        services.AddSingleton<IApiExceptionVisitor, InvalidModelStateExceptionApiErrorVisitor>();
        services.AddSingleton<IApiExceptionVisitor, InvalidModelExceptionApiErrorVisitor>();
        services.AddSingleton<IApiExceptionVisitor, RewardsValidationExceptionApiErrorVisitor<RewardsValidationException>>();
        services.AddSingleton<IApiExceptionVisitor, NotFoundExceptionApiErrorVisitor<RewardNotFoundException>>();
        services.AddSingleton<IApiExceptionVisitor, NotFoundExceptionApiErrorVisitor<PromotionNotFoundException>>();
        services.AddSingleton<IApiExceptionVisitor, NotFoundExceptionApiErrorVisitor<PromotionsNotFoundException>>();
        services.AddSingleton<IApiExceptionVisitor, RewardsValidationExceptionApiErrorVisitor<RewardRnInvalidException>>();

        return services;
    }

    private static IMvcBuilder AddModule(this IMvcBuilder builder, Type type)
    {
        return builder
            .AddApplicationPart(type.Assembly)
            .AddControllersAsServices();
    }

}
