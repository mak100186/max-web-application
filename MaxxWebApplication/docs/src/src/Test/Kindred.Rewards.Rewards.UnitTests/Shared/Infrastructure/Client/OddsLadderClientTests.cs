using System.Net;

using AutoFixture;
using AutoFixture.AutoMoq;

using FluentAssertions;

using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Client;
using Kindred.Rewards.Core.Infrastructure.Net;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Rewards.UnitTests.Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using RestSharp;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Infrastructure.Client;


[TestFixture]
[Category("Unit")]
[Parallelizable(ParallelScope.All)]

public class OddsLadderClientTests : TestBase
{
    private const string DefaultContestType = "default";
    private const string OddsLadderValues = "2.2,1.1,3.3,5.5,4.4";

    private static readonly OddsLadder s_oddsLadder = new()
    {
        ContestType = DefaultContestType,
        PreGameOddsLadder = OddsLadderValues.Split(',').Select(d => new Odds { Key = decimal.Parse(d) }).ToList(),
        InPlayOddsLadder = OddsLadderValues.Split(',').Select(d => new Odds { Key = decimal.Parse(d) }).ToList()
    };

    [Test]
    public async Task GetOddsLadder_ReturnsOddsFromCache_WhenOddsArePresentInCache()
    {
        //arrange
        var context = new Context();
        context.RestClient
            .Setup(x => x.ExecuteAsync<OddsLadder>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetOddsLadderResponse);

        context.RestClient
            .Setup(x => x.ExecuteAsync<OddsLadderOffset>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetOddsLadderOffsetResponse);

        //act
        await context.OddsLadderService.Initialise();

        //assert
        context.RestClient.Verify(s => s.ExecuteAsync<OddsLadder>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        var defaultOddsLadder = await context.OddsLadderService.GetOddsLadder(DefaultContestType);

        //specific call was NOT made
        context.RestClient.Verify(s => s.ExecuteAsync<OddsLadderOffset>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()), Times.Never);

        defaultOddsLadder.Should().NotBeNull();
        defaultOddsLadder.ContestType.Should().Be(DefaultContestType);
        defaultOddsLadder.InPlayOddsLadder.Count.Should().Be(OddsLadderValues.Split(',').Length);
        defaultOddsLadder.PreGameOddsLadder.Count.Should().Be(OddsLadderValues.Split(',').Length);
    }

    [Test]
    public async Task GetOddsLadder_ReturnsDefaultOddsLadder_WhenContestSpecificOneIsNotFound()
    {
        //arrange
        var context = new Context();
        context.RestClient
            .Setup(x => x.ExecuteAsync<OddsLadder>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetOddsLadderResponse);

        context.RestClient
            .Setup(x => x.ExecuteAsync<OddsLadderOffset>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetOddsLadderOffsetResponse);

        //act
        await context.OddsLadderService.Initialise();

        //assert
        var defaultOddsLadder = await context.OddsLadderService.GetOddsLadder("unknownContestType");

        //specific call was made
        context.RestClient.Verify(s => s.ExecuteAsync<OddsLadder>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        context.RestClient.Verify(s => s.ExecuteAsync<OddsLadderOffset>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()), Times.Never);

        defaultOddsLadder.Should().NotBeNull();
        defaultOddsLadder.ContestType.Should().Be(DefaultContestType);
        defaultOddsLadder.InPlayOddsLadder.Count.Should().Be(OddsLadderValues.Split(',').Length);
        defaultOddsLadder.PreGameOddsLadder.Count.Should().Be(OddsLadderValues.Split(',').Length);
    }

    private static RestResponse<OddsLadderOffset> GetOddsLadderOffsetResponse()
    {
        var response = new OddsLadderOffset { Items = new() { s_oddsLadder } };
        return new() { StatusCode = HttpStatusCode.OK, Data = response };
    }

    private static RestResponse<OddsLadder> GetOddsLadderResponse()
    {
        return new() { StatusCode = HttpStatusCode.OK, Data = s_oddsLadder };
    }

    private class Context
    {
        public Context()
        {
            Logger = Fixture.Freeze<Mock<ILogger<OddsLadderClient>>>();
            RestClientFactory = Fixture.Freeze<Mock<Func<string, int, IRestClientWrapper>>>();
            RestClient = Fixture.Freeze<Mock<IRestClientWrapper>>();

            RestClientFactory.Setup(s => s(It.IsAny<string>(), It.IsAny<int>())).Returns(RestClient.Object);

            var configMock = Fixture.Freeze<Mock<IConfiguration>>();

            configMock.Setup(c => c[It.Is<string>(x => x == DomainConstants.OfferPriceManagerBaseUrl)]).Returns("https://localhost:5555");
            configMock.Setup(c => c[It.Is<string>(x => x == DomainConstants.ShouldFetchOnDemand)]).Returns("true");

            OddsLadderService = new OddsLadderClient(
                Logger.Object,
                RestClientFactory.Object,
                configMock.Object);
        }

        public IOddsLadderClient OddsLadderService { get; }

        public Mock<ILogger<OddsLadderClient>> Logger { get; }

        private Mock<Func<string, int, IRestClientWrapper>> RestClientFactory { get; }

        public Mock<IRestClientWrapper> RestClient { get; }

        private IFixture Fixture { get; } = new Fixture().Customize(new AutoMoqCustomization());
    }
}
