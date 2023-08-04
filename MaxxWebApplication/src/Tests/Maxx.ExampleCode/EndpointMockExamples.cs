using FluentAssertions;

using Maxx.Plugin.Common.Extensions;
using Maxx.Plugin.Common.Helpers;
using Maxx.TestCommon.EndpointMock;

using Microsoft.Extensions.Configuration;

namespace Maxx.ExampleCode;

public class EndpointMockExamples
{
    [Test]
    public async Task Test1()
    {
        var config = typeof(EndpointMockExamples).LoadAppSettings($"Data/appsettings.{GetType().Name}.{Reflection.GetCurrentMethodName()}.json");

        var client = new MarketMirrorClient(config);

        var response1 = await client.GetContests(new List<string> { "testContestKey1" });

        response1.Should().NotBeNull();
        response1.Contests.Count().Should().Be(1);
        response1.Contests.First().ContestKey.Should().Be("testContestKey1");
        response1.Contests.First().ContestType.Should().Be("football");
        response1.Contests.First().ContestStatus.Should().Be(ContestStatus.InPlay);

        var response2 = await client.GetContests(new List<string> { "testContestKey2" });

        response2.Should().NotBeNull();
        response2.Contests.Count().Should().Be(1);
        response2.Contests.First().ContestKey.Should().Be("testContestKey2");
        response2.Contests.First().ContestType.Should().Be("soccer");
        response2.Contests.First().ContestStatus.Should().Be(ContestStatus.PreGame);
    }


    #region Private
    private class MarketMirrorClient
    {
        private readonly Mocks<List<string>, EndpointResponse>? _mockTestData;

        public MarketMirrorClient(IConfiguration config)
        {
            _mockTestData = config.GetMocksForMethod<List<string>, EndpointResponse>(nameof(MarketMirrorClient), nameof(GetContests));
        }

        public async Task<EndpointResponse> GetContests(ICollection<string> contestKeys)
        {
            //mock code comes here
            var mock = _mockTestData?.GetApplicableMock(nameof(contestKeys), contestKeys.ToList());
            if (mock != null)
            {
                return mock.Response;
            }

            //concrete code comes here
            return default;
        }
    }

    private class EndpointResponse
    {
        public IEnumerable<ContestDetails> Contests { get; set; }
    }

    private class ContestDetails
    {
        public string ContestKey { get; set; }
        public string ContestType { get; set; }
        public ContestStatus ContestStatus { get; set; }
    }

    private enum ContestStatus
    {
        PreGame,
        InPlay,
        Concluded,
        Cancelled
    }
    #endregion
}

