using Maxx.Plugin.FileUploader.Extensions;
using Maxx.Plugin.FileUploader.Models;
using Maxx.Plugin.FileUploader.Services;
using Maxx.Plugin.Shared.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maxx.Plugin.FileUploader;


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
