using Maxx.Plugin.FileUploaderPostGreSQL.Extensions;
using Maxx.Plugin.FileUploaderPostGreSQL.Models;
using Maxx.Plugin.FileUploaderPostGreSQL.Services;
using Maxx.Plugin.Shared.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maxx.Plugin.FileUploaderPostGreSQL;

public class Registrant : IPluginRegistrant
{
    public IMvcBuilder Register(IMvcBuilder mvcBuilder, IConfiguration hostConfiguration, IConfigurationBuilder configurationBuilder, IHealthProbe healthProbe)
    {
        mvcBuilder
            .AddApplicationPart(typeof(Registrant).Assembly)
            .AddControllersAsServices();

        var configs = typeof(Registrant).LoadConfigurationsFromAssemblyWithType(configurationBuilder);

        mvcBuilder.Services.AddDbContext<DbContextClass>();
        mvcBuilder.Services.AddScoped<IFileService, FileService>();

        //mvcBuilder.Services.AddHealthChecks()
        //    .AddDbContextCheck<DbContextClass>();

        return mvcBuilder;
    }
}
