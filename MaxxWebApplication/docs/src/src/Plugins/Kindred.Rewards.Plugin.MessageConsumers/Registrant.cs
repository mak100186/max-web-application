using System.Reflection;

using FluentValidation;

using Kindred.Infrastructure.Kafka.Handlers;
using Kindred.Rewards.Core.Infrastructure;
using Kindred.Rewards.Core.Mapping;
using Kindred.Rewards.Plugin.Base.Abstractions;
using Kindred.Rewards.Plugin.Base.Health;
using Kindred.Rewards.Plugin.MessageConsumers.EventHandlers;
using Kindred.Rewards.Plugin.MessageConsumers.Services;
using Kindred.Rewards.Plugin.MessageConsumers.Validators;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kindred.Rewards.Plugin.MessageConsumers;

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
        var registrantMetadata = new RegistrantMetadata(typeof(Registrant));

        configurationBuilder.AddConfiguration(registrantMetadata.Config);

        
        services.AddCoreServices(registrantMetadata.Config);

        services.AddInfrastructure(registrantMetadata.Config);

        services.AddSingleton<IClaimService, ClaimService>();
        services.AddSingleton<ICustomerRewardService, CustomerRewardService>();

        services.AddSingleton<IValidator<BetRejected>, BetMessageEventValidator>();
        services.AddSingleton<IValidator<BetCancelled>, BetMessageEventValidator>();
        services.AddSingleton<IValidator<CouponFailed>, CouponMessageEventValidator>();
        services.AddSingleton<IValidator<CouponDeclined>, CouponMessageEventValidator>();

        services.AddAutoMapper(Assembly.GetAssembly(typeof(RewardDomainToApiMapping)));

        services.AddSingleton<IMessageHandler, MissionAchievedRewardHandler>();
        services.AddSingleton<IMessageHandler, CouponFailedMessageHandler>();
        services.AddSingleton<IMessageHandler, CouponDeclinedMessageHandler>();
        services.AddSingleton<IMessageHandler, BetCancelledMessageHandler>();
        services.AddSingleton<IMessageHandler, BetRejectedMessageHandler>();
        services.AddSingleton<IMessageHandler, OddsLadderCreatedHandler>();
        services.AddSingleton<IMessageHandler, OddsLadderUpdatedHandler>();
        services.AddSingleton<IMessageHandler, OddsLadderDeletedHandler>();


        return services;
    }
}
