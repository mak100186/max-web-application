using System.Net;

using FluentAssertions;

using Kindred.Rewards.Rewards.FunctionalTests.Common.Setup;
using Kindred.Rewards.Rewards.Tests.Common;

using RestSharp;

using TechTalk.SpecFlow;

namespace Kindred.Rewards.Rewards.FunctionalTests.Steps;

[Binding]
public class CommonSteps : AcceptanceScenarioBase
{

    public CommonSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
    {
    }

    public static long PrefixCustomerId(string baseCustomerId)
    {
        return Math.Abs($"{TestConstants.TestPrefix}{baseCustomerId}".GetHashCode());
    }

    public static string Prefix(string value)
    {
        return $"{TestConstants.TestPrefix}{value}";
    }

    [Then(@"the HttpStatusCode should be (.*)")]
    public void ThenTheHttpStatusCodeShouldBe(int statusCode)
    {
        ScenarioContext
            .TryGetValue(TestConstants.RestResponse, out RestResponse response);

        if (statusCode <= 299 && !response.IsSuccessful)
        {
            throw new(response.Content);
        }

        ScenarioContext
            .Get<RestResponse>(TestConstants.RestResponse)
            .StatusCode
            .Should()
            .Be(Enum.Parse<HttpStatusCode>(statusCode.ToString()));
    }

    [Then(@"Response should contain error '(.*)'")]
    public void ThenCreateResponseShouldContainError(string error)
    {
        var responseStatusCode = ScenarioContext.Get<string>(TestConstants.RestResponseSharedError);
        responseStatusCode.Should().Contain(error);
    }
}
