using Kindred.Rewards.Plugin.Claim.Clients.MarketMirror.Responses;

using Newtonsoft.Json;

using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Kindred.Rewards.Rewards.FunctionalTests.Mocks;

internal static class MarketMirrorMock
{
    internal static readonly WireMockServer s_server;

    private static readonly GetContestsResponse s_inPlayContests = new()
    {
        Contests = new List<ContestDetails>
        {
            new() { ContestKey = "InPlayContest", ContestStatus = ContestStatus.InPlay }
        }
    };

    private static readonly GetContestsResponse s_preGameContests = new()
    {
        Contests = new List<ContestDetails>
        {
            new() { ContestKey = "PreGameContest", ContestStatus = ContestStatus.PreGame }
        }
    };

    static MarketMirrorMock()
    {
        // TODO This should be dynamic port but the way our Acceptance tests prevents this
        // If/When we use TestServer to spin up the service this will be a lot easier as we can override the config
        s_server = WireMockServer.Start(60421);
    }

    public static void SetUp()
    {
        s_server
            .Given(Request.Create().WithPath("/contestDetails")
                .UsingGet()
                .WithParam("contestKeys", new ExactMatcher("InPlayContest")))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody(JsonConvert.SerializeObject(s_inPlayContests))
            );

        s_server
            .Given(Request.Create().WithPath("/contestDetails").UsingGet()
                .WithParam("contestKeys", new ExactMatcher("PreGameContest")))
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody(JsonConvert.SerializeObject(s_preGameContests))
            );
    }
}
