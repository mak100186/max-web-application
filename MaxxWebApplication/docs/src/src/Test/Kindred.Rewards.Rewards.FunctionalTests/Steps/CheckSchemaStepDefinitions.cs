using System.Net;

using FluentAssertions;

using Kindred.Rewards.Core.Models.Messages;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Helpers;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Setup;
using Kindred.Rewards.Rewards.Tests.Common;

using RestSharp;

using TechTalk.SpecFlow;

using Unibet.Infrastructure.Messaging.KafkaFlow.Helpers;

namespace Kindred.Rewards.Rewards.FunctionalTests.Steps;

[Binding]
public class CheckSchemaStepDefinitions : AcceptanceScenarioBase
{
    protected readonly IApiClient ApiClient;

    private const string SchemaPath = "messages/";

    public RestResponse RestResponse { get; set; }

    protected CheckSchemaStepDefinitions(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
    {
        ApiClient = new ApiClient("schema", RestClientFactory, scenarioContext, SerializerSettings.GetJsonSerializerSettings);
    }

    [Given(@"A request to the /schema/messages endpoint")]
    public void GivenARequestToTheSchemaMessagesEndpoint()
    {
    }

    [When(@"The request is submitted")]
    public async Task WhenTheRequestIsSubmitted()
    {
        RestResponse = await ApiClient.GetAsync(SchemaPath);
    }

    [Then(@"I should receive a response of status code (.*)")]
    public void ThenIShouldReceiveAResponseOfStatusCode(int statusCode)
    {
        ScenarioContext
            .Get<RestResponse>(TestConstants.RestResponse)
            .StatusCode
            .Should()
            .Be(Enum.Parse<HttpStatusCode>(statusCode.ToString()));
    }

    [Then(@"I expect all populated list of message schemas")]
    public void ThenIExpectAllPopulatedListOfMessageSchemas()
    {
        var content = RestResponse.Content;

        var messagesAssembly = typeof(RewardClaim).Assembly;
        var messages = KspMessageHelper
            .GetKspMessagesTypes(messagesAssembly)
            .Select(x => x.Name);

        foreach (var className in messages)
        {
            RestResponse.Content.Should().Contain(className);
        }
    }
}
