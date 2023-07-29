using System.Reflection;
using System.Text.Json.Serialization;

using FluentValidation.AspNetCore;

using Maxx.Plugin.Shared.Abstractions;
using Maxx.Plugin.Shared.Health;

using MaxxWebApplication.Health;
using MaxxWebApplication.Logging;

using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace MaxxWebApplication.Dependency;

public static class ServiceRegistrations
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Host.AddLogger();

        var mvcBuilder = builder.Services
            .AddMvc(c => c.SuppressAsyncSuffixInActionNames = false)
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                var schemaHelper = new SwashbuckleSchemaHelper();
                options.CustomSchemaIds(type => schemaHelper.GetSchemaId(type));
            })
            .Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; })
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .LoadPlugins(builder.Configuration, mvcBuilder)
            .AddHealthChecks().AddCheck<ServiceHealthCheck>(nameof(ServiceHealthCheck));

        return builder;
    }

    public static IApplicationBuilder ConfigureDevelopmentEnvironment(this IApplicationBuilder app)
    {
        var webApp = (WebApplication)app;
        if (webApp.Environment.IsDevelopment())
        {
            webApp
                .UseSwagger()
                .UseSwaggerUI();
        }

        return app;
    }

    public static IApplicationBuilder ConfigureApp(this IApplicationBuilder app)
    {
        var webApp = (WebApplication)app;
        var healthProbe = new HealthProbe();
        var configuration = webApp.Configuration;

        var pluginPaths = configuration.GetSection("appsettings:pluginPaths").Get<string[]>();

        foreach (var pluginPath in pluginPaths)
        {
            try
            {
                var pluginAssembly = LoadPlugin(pluginPath);

                var appConfigurantTypeName = pluginAssembly
                    .GetTypes()
                    .Single(t => t.GetInterfaces().Any(i => i.Name == nameof(IAppConfigurant)))
                    .FullName;

                if (string.IsNullOrWhiteSpace(appConfigurantTypeName))
                {
                    Log.Information("No configurant found in {pluginPath}", pluginPath);

                    continue;
                }

                var appConfigurant = pluginAssembly.CreateInstance<IAppConfigurant>(appConfigurantTypeName);

                appConfigurant.Configure(webApp);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error configuring plugin");
                healthProbe.AddError("Host.ServiceRegistration", e.Message);
            }
        }

        if (Environment.GetCommandLineArgs().Contains("--migrate"))
        {
            Environment.Exit(0);
        }

        return app;
    }

    #region plugin loading

    private static IServiceCollection LoadPlugins(this IServiceCollection services, ConfigurationManager configuration, IMvcBuilder mvcBuilder)
    {
        var healthProbe = new HealthProbe();
        services.AddSingleton<IHealthProbe>(healthProbe);

        var pluginPaths = configuration.GetSection("appsettings:pluginPaths").Get<string[]>();

        foreach (var pluginPath in pluginPaths)
        {
            try
            {
                var pluginAssembly = LoadPlugin(pluginPath);

                var pluginRegistrantTypeName = pluginAssembly
                    .GetTypes()
                    .Single(t => t.GetInterfaces().Any(i => i.Name == nameof(IPluginRegistrant)))
                    .FullName;

                var pluginRegistrant = pluginAssembly.CreateInstance<IPluginRegistrant>(pluginRegistrantTypeName!);

                // create services the host doesn't know about
                pluginRegistrant.Register(mvcBuilder, configuration, configuration, healthProbe);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error loading plugin");
                healthProbe.AddError("Host.ServiceRegistration", e.Message);
            }
        }

        return services;
    }

    private static Assembly LoadPlugin(string relativePath)
    {
        // Navigate up to the solution root
        var root = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

        var pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
        Console.WriteLine($"Loading plugin: {pluginLocation}");

        var loadContext = new PluginLoadContext(pluginLocation);

        return loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(pluginLocation));
    }

    private static object CreateInstance(this Assembly assembly, string typeName, params object[] paramArray)
    {
        if (paramArray.Length > 0)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            var activationAttrs = Array.Empty<object>();

            return assembly.CreateInstance(typeName, false, BindingFlags.CreateInstance, null, paramArray, culture, activationAttrs)!;
        }

        return assembly.CreateInstance(typeName)!;
    }

    private static T CreateInstance<T>(this Assembly assembly, string typeName, params object[] paramArray) => (T)CreateInstance(assembly, typeName, paramArray);

    #endregion
}
