using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;

using FluentValidation.AspNetCore;

using Kindred.Infrastructure.Hosting.WebApi.ExceptionHandling;
using Kindred.Infrastructure.Hosting.WebApi.Filters;
using Kindred.Infrastructure.Hosting.WebApi.Health;
using Kindred.Infrastructure.Hosting.WebApi.Interfaces;
using Kindred.Infrastructure.Hosting.WebApi.Middleware;
using Kindred.Rewards.Plugin.Base.Abstractions;
using Kindred.Rewards.Plugin.Base.Health;
using Kindred.Rewards.Plugin.Host.Dependency;
using Kindred.Rewards.Plugin.Host.Health;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.Host;

public static class ServiceRegistrations
{
    public static void ConfigureServices(this IServiceCollection services, ConfigurationManager configManager)
    {
        services.AddScoped<CorrelationIdMiddleware>();

        //add error handling
        services.AddSingleton<IApiExceptionVisitorResolver, ApiExceptionVisitorResolver>();
        services.AddSingleton<IApiExceptionVisitor, DefaultExceptionVisitor>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            //options.ExampleFilters();

            var schemaHelper = new SwashbuckleSchemaHelper();
            options.CustomSchemaIds(type => schemaHelper.GetSchemaId(type));
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        var mvcBuilder = services
            .AddMvc(c =>
            {
                c.Filters.ConfigureFilters();

                c.SuppressAsyncSuffixInActionNames = false;
            })
            .AddFluentValidation()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        
        services.LoadPlugins(configManager, mvcBuilder);

        services.AddHealthChecks()
            .AddCheck<InstanceDiagnosticsCheck>(nameof(InstanceDiagnosticsCheck))
            .AddCheck<RewardsPluginServiceCheck>(nameof(RewardsPluginServiceCheck));
    }

    public static void ConfigureFilters(this FilterCollection filterCollection)
    {
        var filters = new SortedList<int, Type>
        {
            { 10, typeof(DefaultExceptionHandlingFilter) },
            { 30, typeof(RequestAuditFilter) },
            { 40, typeof(ValidateModelStateFilter) }
        };

        foreach (var (key, value) in filters)
        {
            filterCollection.Add(value, key);
        }
    }

    public static void LoadPlugins(this IServiceCollection services, ConfigurationManager configuration, IMvcBuilder mvcBuilder)
    {
        var healthProbe = new HealthProbe();
        services.AddSingleton<IHealthProbe>(healthProbe);

        var pluginPaths = configuration.GetSection("appsettings:pluginPaths").Get<string[]>();

        foreach (var pluginPath in pluginPaths)
        {
            try
            {
                var pluginAssembly = LoadPlugin(pluginPath);

                var pluginRegistrantTypeName = pluginAssembly.GetTypes()
                    .Single(t => t.GetInterfaces().Any(i => i.Name == nameof(IRegistrant))).FullName;

                var pluginRegistrant = pluginAssembly.CreateInstance<IRegistrant>(pluginRegistrantTypeName!);

                // create services the host doesn't know about
                pluginRegistrant.Register(mvcBuilder, configuration, healthProbe);

            }
            catch (Exception e)
            {
                Log.Error(e, "Error loading plugin");
            }
        }
    }

    static Assembly LoadPlugin(string relativePath)
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

    #region create instance from typeName

    public static object CreateInstance(this Assembly assembly, string typeName, params object[] paramArray)
    {
        if (paramArray.Length > 0)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            var activationAttrs = Array.Empty<object>();
            return assembly.CreateInstance(typeName, false, BindingFlags.CreateInstance, null, paramArray, culture, activationAttrs)!;
        }

        return assembly.CreateInstance(typeName)!;
    }

    public static T CreateInstance<T>(this Assembly assembly, string typeName, params object[] paramArray) => (T)CreateInstance(assembly, typeName, paramArray);

    #endregion
}

public class SwashbuckleSchemaHelper
{
    private readonly Dictionary<string, int> _schemaNameRepetition = new();

    // borrowed from https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/95cb4d370e08e54eb04cf14e7e6388ca974a686e/src/Swashbuckle.AspNetCore.SwaggerGen/SchemaGenerator/SchemaGeneratorOptions.cs#L44
    private string DefaultSchemaIdSelector(Type modelType)
    {
        if (!modelType.IsConstructedGenericType)
        {
            return modelType.Name.Replace("[]", "Array");
        }

        var prefix = modelType.GetGenericArguments()
            .Select(DefaultSchemaIdSelector)
            .Aggregate((previous, current) => previous + current);

        return prefix + modelType.Name.Split('`').First();
    }

    public string GetSchemaId(Type modelType)
    {
        var id = DefaultSchemaIdSelector(modelType);

        if (!_schemaNameRepetition.ContainsKey(id))
        {
            _schemaNameRepetition.Add(id, 0);
        }

        var count = _schemaNameRepetition[id] + 1;
        _schemaNameRepetition[id] = count;

        return $"{id}{(count > 1 ? count.ToString() : "")}";
    }
}
