using System.Reflection;

using Kindred.Infrastructure.Hosting.WebApi.Interfaces;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Authorization;
using Kindred.Rewards.Core.ExceptionHandling;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Infrastructure;
using Kindred.Rewards.Core.Mapping;
using Kindred.Rewards.Core.Mapping.Converters;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Plugin.Base.Abstractions;
using Kindred.Rewards.Plugin.Base.Health;
using Kindred.Rewards.Plugin.Template.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.Template;
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

        var blueprintConfigSection = registrantMetadata.Config.GetSection(DomainConstants.TemplateBlueprints);
        services.Configure<BlueprintsConfigurationModel>(blueprintConfigSection);

        configurationBuilder.AddConfiguration(registrantMetadata.Config);
        
        services.AddCoreServices(registrantMetadata.Config);

        services.AddInfrastructure(registrantMetadata.Config);

        services.AddAutoMapper(Assembly.GetAssembly(typeof(RewardDomainToApiMapping)));

        services.TryAddScoped<ITemplateCrudService, TemplateCrudService>();

        services.AddSwaggerExamplesFromAssemblyOf<Registrant>();

        mvcBuilder.AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new RewardParameterApiModelBaseConverter()));

        services.AddSingleton<IApiExceptionVisitor, NotFoundExceptionApiErrorVisitor<PromotionTemplateNotFoundException>>();

        services.AddHttpContextAccessor();
        services.AddScoped<IAuthorisationService, AuthorisationService>();
        return services;
    }

    private static IMvcBuilder AddModule(this IMvcBuilder builder, Type type)
    {
        return builder
            .AddApplicationPart(type.Assembly)
            .AddControllersAsServices();
    }

}
