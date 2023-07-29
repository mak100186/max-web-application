using System.Reflection;

using Kindred.Infrastructure.Kafka.Extensions;
using Kindred.Rewards.Core.Client;
using Kindred.Rewards.Core.Helpers;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Infrastructure.Net;
using Kindred.Rewards.Core.Mapping;
using Kindred.Rewards.Core.Mapping.Converters;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Rn;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Serilog;

namespace Kindred.Rewards.Core.Infrastructure;
public static class ServiceRegistrations
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        if (Environment.GetCommandLineArgs().Contains("--migrate"))
        {
            var contextFactory = app.Services.GetRequiredService<IDbContextFactory<RewardsDbContext>>();
            await using var context = await contextFactory.CreateDbContextAsync();

            if (bool.TryParse(app.Configuration[DomainConstants.ShouldDropDbBetweenBuilds], out var shouldDropBetweenBuilds))
            {
                if (shouldDropBetweenBuilds)
                {
                    Log.Warning("Dropping schema + data!");
                    await context.Database.ExecuteSqlRawAsync("DROP SCHEMA IF EXISTS public CASCADE");

                    Log.Warning("Recreating schema + data!");
                    await context.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS public");
                }
            }

            var pending = await context.Database.GetPendingMigrationsAsync();

            Log.Information(
                pending.Any()
                    ? $"Applying Migrations :{Environment.NewLine}{string.Join(Environment.NewLine, pending)}"
                    : "No outstanding migrations");

            await context.Database.MigrateAsync();

            Environment.Exit(0);
        }
    }
    public static void InitializeOddsLadder(this IServiceProvider serviceProvider)
    {
        var oddsLadderClient = serviceProvider.GetRequiredService<IOddsLadderClient>();

        oddsLadderClient.Initialise();
    }

    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration config)
    {
        services.TryAddSingleton<ITimeService, TimeService>();
        services.TryAddSingleton<IConverter<string, RewardRn>, RewardRnConverter>();
        services.AddAutoMapper(Assembly.GetAssembly(typeof(RewardApiToDomainMapping)));

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddTemplateConfigurations(config)
            .AddExternalClients()
            .AddRestSharp()
            .AddDatabase(config)
            .AddKafka(config)
            .AddAutoMapper(typeof(RewardDomainToDataMapping));

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString(DomainConstants.ConnectionString);
        services.AddDbContextFactory<RewardsDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }

    private static IServiceCollection AddRestSharp(this IServiceCollection services)
    {
        services.TryAddSingleton<Func<string, int, IRestClientWrapper>>((baseUrl, timeout) => new RestClientWrapper(baseUrl, timeout));
        services.TryAddSingleton<Func<string, IRestClientWrapper>>(baseUrl => new RestClientWrapper(baseUrl));

        return services;
    }

    private static IServiceCollection AddTemplateConfigurations(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<BlueprintsConfigurationModel>(config.GetSection(DomainConstants.TemplateBlueprints));

        return services;
    }

    private static IServiceCollection AddExternalClients(this IServiceCollection services)
    {
        services.TryAddSingleton<IOddsLadderClient, OddsLadderClient>();

        return services;
    }
}

