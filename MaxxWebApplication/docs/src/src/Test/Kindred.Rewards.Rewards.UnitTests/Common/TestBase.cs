using System.ComponentModel.DataAnnotations;
using System.Reflection;

using AutoFixture;
using AutoFixture.AutoMoq;

using AutoMapper;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Mapping;
using Kindred.Rewards.Core.Mapping.Converters;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Rn;
using Kindred.Rewards.Core.WebApi;
using Kindred.Rewards.Plugin.FreeBet.Mappings;
using Kindred.Rewards.Plugin.ProfitBoost.Mappings;
using Kindred.Rewards.Plugin.UniBoost.Mappings;
using Kindred.Rewards.Plugin.UniBoostReload.Mappings;
using Kindred.Rewards.Rewards.UnitTests.Common.AutoFixtureCustomizations;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Common;

public abstract class TestBase
{
    private const string OddsLadderValues = "1.01,1.02,1.025,1.0303030303,1.04,1.05,1.0625,1.0714285714,1.0833333333,1.1,1.1111111111,1.125,1.1428571429,1.1666666667,1.1818181818,1.2,1.2222222222,1.25,1.2857142857,1.3,1.3333333333,1.3636363636,1.4,1.4444444444,1.4705882353,1.5,1.5333333333,1.5714285714,1.6153846154,1.6666666667,1.7272727273,1.8,1.8333333333,1.9,1.9090909091,2,2.1,2.2,2.25,2.375,2.4,2.5,2.6,2.625,2.75,2.8,2.875,3,3.125,3.2,3.25,3.4,3.5,3.6,3.75,3.8,4,4.2,4.3333333333,4.4,4.5,4.6,4.8,5,5.5,6,6.5,7,7.5,8,8.5,9,9.5,10,11,12,13,15,17,19,21,23,26,29,34,41,51,67,81,101,151,201,251,301,501,1001";

    protected IFixture Fixture { get; private set; }

    protected IMapper Mapper { get; set; }

    protected TestBase()
    {
        Mapper = InitMapper();
    }

    [OneTimeSetUp]
    public virtual void TestFixtureSetUp()
    {
        Fixture = new Fixture();
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        foreach (var customization in GetCustomizations())
        {
            Fixture = Fixture.Customize(customization);
        }
    }

    protected string GetExpectedErrorMessage<T>(T error)
    {
        return error.GetAttributeValue<DisplayAttribute>(ApiConstants.Description);
    }

    protected virtual IReadOnlyCollection<ICustomization> GetCustomizations()
    {
        return new ICustomization[] { new AutoMoqCustomization(), new CreateRewardRequestCustomization() };
    }

    private static IMapper InitMapper()
    {
        var services = new ServiceCollection();
        //add all mappings from each plugin and core

        services.AddAutoMapper(
            Assembly.GetAssembly(typeof(FreebetApiToDomainMapping)),
            Assembly.GetAssembly(typeof(ProfitBoostApiToDomainMapping)),
            Assembly.GetAssembly(typeof(Plugin.Reward.Mappings.RewardApiToDomainMapping)),
            Assembly.GetAssembly(typeof(RewardApiToDomainMapping)),
            Assembly.GetAssembly(typeof(UniBoostApiToDomainMapping)),
            Assembly.GetAssembly(typeof(UniBoostReloadApiToDomainMapping)));

        services.AddSingleton<IConverter<string, RewardRn>, RewardRnConverter>();
        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetService<IMapper>();
    }

    internal static OddsLadder OddsLadder(string oddsLadderOverride = null) =>
        new()
        {
            PreGameOddsLadder = CreatePreGameOddsLadder(oddsLadderOverride),
            InPlayOddsLadder = CreateInPlayOddsLadder(oddsLadderOverride)
        };

    private static List<Odds> CreatePreGameOddsLadder(string oddsLadderValues)
    {
        if (oddsLadderValues == null)
        {
            return OddsLadderValues
                .Split(',')
                .Select(d => new Odds { Key = decimal.Parse(d) })
                .ToList();
        }

        return oddsLadderValues
            .Split(',')
            .Select(d => new Odds { Key = decimal.Parse(d) })
            .ToList();
    }

    private static List<Odds> CreateInPlayOddsLadder(string oddsLadderValues)
    {
        var preGameOddsLadder = CreatePreGameOddsLadder(oddsLadderValues);

        // create a slightly different oddsLadder for testing purposes
        var inPlayOddsLadder = new List<Odds>(preGameOddsLadder);
        inPlayOddsLadder.RemoveRange(0, 2);

        return inPlayOddsLadder;
    }
}
