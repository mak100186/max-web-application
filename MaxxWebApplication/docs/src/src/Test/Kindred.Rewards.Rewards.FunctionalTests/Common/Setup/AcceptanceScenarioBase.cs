using System.Net.Sockets;
using System.Reflection;

using Kindred.Infrastructure.Core.Configuration.SelfReferencing;
using Kindred.Infrastructure.Core.NUnit.Logging;
using Kindred.Infrastructure.Kafka.Extensions;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Infrastructure.Net;
using Kindred.Rewards.Core.Mapping;
using Kindred.Rewards.Core.Mapping.Converters;
using Kindred.Rewards.Core.Models.Rn;
using Kindred.Rewards.Plugin.FreeBet.Mappings;
using Kindred.Rewards.Plugin.ProfitBoost.Mappings;
using Kindred.Rewards.Plugin.UniBoost.Mappings;
using Kindred.Rewards.Plugin.UniBoostReload.Mappings;
using Kindred.Rewards.Rewards.Tests.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NUnit.Framework;

using RestSharp;

using Serilog;

using TechTalk.SpecFlow;

using Environment = System.Environment;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Kindred.Rewards.Rewards.FunctionalTests.Common.Setup;

[Binding]
public class AcceptanceScenarioBase
{
    protected static IServiceProvider ServiceProvider { get; private set; }

    protected static IConfiguration Config;

    private static ILogger s_logger;

    protected ScenarioContext ScenarioContext { get; }
    protected FeatureContext FeatureContext { get; }

    protected static Func<string, IRestClientWrapper> RestClientFactory => controller => new RestClientWrapper($"{Config[DomainConstants.WebApiUrl]}{controller}");

    protected AcceptanceScenarioBase(ScenarioContext scenarioContext, FeatureContext featureContext)
    {
        ScenarioContext = scenarioContext;
        FeatureContext = featureContext;
    }

    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        try
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            ServiceProvider = Configure(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true)
                    .AddEnvironmentVariables()
                    .AddReferencingProvider()
                    .AddInMemoryCollection(new[] { new KeyValuePair<string, string>(HostDefaults.EnvironmentKey, environment) })
                    .Build(), new ServiceCollection())
                .BuildServiceProvider();

            s_logger = ServiceProvider.GetRequiredService<ILogger<AcceptanceScenarioBase>>();

            Config = ServiceProvider.GetRequiredService<IConfiguration>();

            ServiceProvider.AddKafkaHandlers();

            await CleanUpTestData();
        }
        catch (SocketException se)
        {
            s_logger.LogError(se, "Before test run socket exception");
        }
        catch (Exception ex)
        {
            s_logger.LogError(ex, "Before test run exception");
        }
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        await CleanUpTestData();
    }

    public static IServiceCollection Configure(IConfiguration configuration, IServiceCollection services)
    {
        services.AddSingleton(configuration);

        services.AddNUnitLogging();

        services.TryAddSingleton<IConverter<string, RewardRn>, RewardRnConverter>();

        services.AddDbContextFactory<RewardsDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString(DomainConstants.ConnectionString)));

        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger(), true));

        services.AddKafka(configuration);

        //add all mappings from each plugin and core
        services.AddAutoMapper(
            Assembly.GetAssembly(typeof(FreebetApiToDomainMapping)),
            Assembly.GetAssembly(typeof(ProfitBoostApiToDomainMapping)),
            Assembly.GetAssembly(typeof(Plugin.Reward.Mappings.RewardApiToDomainMapping)),
            Assembly.GetAssembly(typeof(RewardApiToDomainMapping)),
            Assembly.GetAssembly(typeof(UniBoostApiToDomainMapping)),
            Assembly.GetAssembly(typeof(UniBoostReloadApiToDomainMapping)));

        return services;
    }

    [BeforeScenario("promooddsladder")]
    public static async Task BeforeLadderInteraction()
    {
        var baseUrl = Config[DomainConstants.OfferPriceManagerBaseUrl];
        const string ladderUrl = "OddsLadders/?nocache=true";

        s_logger.LogInformation("OddsLadderUrl = {baseUrl}{ladderUrl}", baseUrl, ladderUrl);

        var client = new RestClient(baseUrl);
        var request = new RestRequest(ladderUrl);

        RestResponse result;
        try
        {
            result = await client.GetAsync(request);
        }
        catch (Exception e)
        {
            s_logger.LogWarning("Exception occurred:{@e}", e);
            throw;
        }

        if (!result.IsSuccessful)
        {
            s_logger.LogInformation("Unsuccessful");
            Assert.Inconclusive($"Test Relies on oddsLadder from Offer Manager, cannot get Offer Manager ladder {ladderUrl}");
        }

        s_logger.LogInformation("Connection to price manager successful");
    }

    [BeforeScenario]
    public static async Task CleanUpTestData()
    {
        s_logger.LogInformation("Cleaning Test Data");
        var contextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<RewardsDbContext>>();
        await using var context = await contextFactory.CreateDbContextAsync();

        var claims = context.RewardClaims
            .Where(b => b.CouponRn.StartsWith(TestConstants.TestPrefix)
                        || b.RewardId.StartsWith(TestConstants.TestPrefix)
                        || b.RewardName.StartsWith(TestConstants.TestPrefix));

        var rewards = context.Rewards
            .Include(a => a.RewardTags).ThenInclude(t => t.Tag)
            .Where(b => b.Name.StartsWith(TestConstants.TestPrefix)
                        || claims.Select(x => x.CustomerId).Contains(b.CustomerId));

        var rewardIds = rewards.Select(x => x.Id);

        context.CombinationRewardPayoffs.RemoveRange(context.CombinationRewardPayoffs.Where(c => claims.Select(x => x.InstanceId).Contains(c.ClaimInstanceId)));
        context.RewardClaims.RemoveRange(claims);
        context.Tags.RemoveRange(context.Tags.Where(x => rewards.SelectMany(b => b.RewardTags.Select(bt => bt.Tag.Id)).Distinct().Contains(x.Id)));
        context.CustomerRewards.RemoveRange(context.CustomerRewards.Where(cr => rewardIds.Contains(cr.RewardId)));
        context.AuditRewards.RemoveRange(context.AuditRewards.Where(x => rewardIds.Contains(x.RewardId)));
        context.Rewards.RemoveRange(context.Rewards.Where(x => rewardIds.Contains(x.Id)));
        context.RewardTemplates.RemoveRange(context.RewardTemplates.Where(t => t.Key.StartsWith(TestConstants.TestPrefix)));
        context.RewardTemplateCustomers.RemoveRange(context.RewardTemplateCustomers.Where(t => t.PromotionTemplateKey.StartsWith(
            TestConstants.TestPrefix)));

        await context.SaveChangesAsync();

        s_logger.LogInformation("Cleaning Test Data successful");
    }
}
