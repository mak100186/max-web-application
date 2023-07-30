using Maxx.Plugin.Shared.Abstractions;
using Maxx.Plugin.TodoListPostGreSQL.Extensions;
using Maxx.Plugin.TodoListPostGreSQL.Models;
using Maxx.Plugin.TodoListPostGreSQL.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maxx.Plugin.TodoListPostGreSQL;

public class Registrant : IPluginRegistrant
{
    public IMvcBuilder Register(IMvcBuilder mvcBuilder, IConfiguration hostConfiguration, IConfigurationBuilder configurationBuilder, IHealthProbe healthProbe)
    {
        mvcBuilder
            .AddApplicationPart(typeof(Registrant).Assembly)
            .AddControllersAsServices();

        var configs = typeof(Registrant).LoadConfigurationsFromAssemblyWithType(configurationBuilder);

        mvcBuilder.Services.AddDbContext<DbContextClass>();
        mvcBuilder.Services.AddScoped<ITodosService, TodosService>();

        //mvcBuilder.Services.AddHealthChecks()
        //    .AddDbContextCheck<DbContextClass>();

        return mvcBuilder;
    }
}
