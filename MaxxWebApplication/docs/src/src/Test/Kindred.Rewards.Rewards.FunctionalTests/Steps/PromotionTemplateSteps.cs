using System.IdentityModel.Tokens.Jwt;

using FluentAssertions;
using FluentAssertions.Execution;

using Kindred.Infrastructure.Kafka;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Authorization.Middleware;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models.Messages.RewardTemplate;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Responses;
using Kindred.Rewards.Plugin.Reward.Models;
using Kindred.Rewards.Rewards.FunctionalTests.Common;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Helpers;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Setup;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.Tests.Common.Extensions;

using NUnit.Framework;

using Quartz.Util;

using TechTalk.SpecFlow;

namespace Kindred.Rewards.Rewards.FunctionalTests.Steps;

[Binding]
public class PromotionTemplateSteps : AcceptanceScenarioBase
{
    private readonly IApiClient _apiClient;
    private readonly IApiClient _promotionClient;
    private RewardTemplateResponse _getResponse;
    private GetPromotionTemplatesResponse _getAllResponse;

    public PromotionTemplateSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
    {
        _apiClient = new ApiClient("rewardtemplates", RestClientFactory, scenarioContext, SerializerSettings.GetJsonSerializerSettings);
        _promotionClient = new ApiClient("rewards", RestClientFactory, scenarioContext, SerializerSettings.GetJsonSerializerSettings);
    }

    private Dictionary<string, RewardTemplateResponse> TestPromotionTemplates
    {
        get
        {
            if (!FeatureContext.TryGetValue(nameof(TestPromotionTemplates), out Dictionary<string, RewardTemplateResponse> returnValue))
            {
                returnValue = new();
            }

            return returnValue;
        }

        set => FeatureContext.Set(value, nameof(TestPromotionTemplates));
    }

    public Dictionary<string, RewardApiModel> TestRewards
    {
        get
        {
            if (!FeatureContext.TryGetValue(nameof(TestRewards), out Dictionary<string, RewardApiModel> returnValue))
            {
                returnValue = new();
            }

            return returnValue;
        }

        set => FeatureContext.Set(value, nameof(TestRewards));
    }

    [StepArgumentTransformation]
    public static List<string> TransformToListOfString(string commaSeparatedList)
    {
        return commaSeparatedList.ExtractValues();
    }

    [Given(@"I have promotions templates in the system")]
    public async Task GivenIHavePromotionsTemplatesInTheSystem(Table table)
    {
        var testTemplates = new Dictionary<string, RewardTemplateResponse>();

        foreach (var row in table.Rows)
        {
            var request = PromotionTemplateBuilder.CreateCreationRequest(x =>
            {
                x.TemplateKey = CommonSteps.Prefix(row["key"]);
                x.Comments = CommonSteps.Prefix(row["comments"]);
            });

            var template = await _apiClient.PostAsync<CreateRewardTemplateRequest, RewardTemplateResponse>(request);

            template.Should().BeEquivalentTo(new
            {
                request.TemplateKey,
                request.Comments,
                Enabled = true,
                request.Title,
            });


            KafkaConsumerManager.Wait<RewardTemplateCreated>(DomainConstants.TopicProfileTemplateUpdates, (c) =>
            {
                var message = c.Value;
                return message.TemplateKey == request.TemplateKey && message.Enabled == true && message.Comments == request.Comments;

            });

            testTemplates.Add(request.TemplateKey, new() { TemplateKey = request.TemplateKey, Comments = request.Comments });
            TestPromotionTemplates = testTemplates;
        }
    }

    [When(@"I submit Get request for template key '(.*)' for ActiveState is '(.*)'")]
    public async Task WhenISubmitGetRequestForTemplateKeyForActiveStateIs(string templateKey, string isActive)
    {
        var key = CommonSteps.Prefix(templateKey);

        TestPromotionTemplates.TryGetValue(key, out var template);

        var templateTemplateKey = template?.TemplateKey ?? templateKey;

        var isActiveSuffix = string.Empty;

        var parseSuccess = bool.TryParse(isActive, out var active);

        if (parseSuccess)
        {
            isActiveSuffix = "?isActive=" + active;
        }

        _getResponse = await _apiClient.GetAsync<RewardTemplateResponse>(templateTemplateKey + isActiveSuffix);
    }

    [Then(@"Get response should return template for template key '(.*)'")]
    public void ThenGetResponseShouldReturnTemplate(string templateKey)
    {
        var key = CommonSteps.Prefix(templateKey);
        _getResponse.ShouldNotBeNull();
        TestPromotionTemplates.ContainsKey(key).Should().BeTrue();
        _getResponse.TemplateKey.Should().Be(key);
        _getResponse.Comments.Should().Be(TestPromotionTemplates[key].Comments);
    }

    [When(@"I submit GetAll request")]
    public async Task WhenISubmitGetAllRequest()
    {
        _getAllResponse = await _apiClient.GetAllAsync<GetPromotionTemplatesResponse>();
    }

    [When(@"I submit GetAll request with following filters:")]
    public async Task WhenISubmitGetAllRequestWithFollowingFilters(Table table)
    {
        var request = new GetPromotionTemplatesRequest
        {
            IncludeDisabled = bool.Parse(table.Rows[0]["includeDisabled"])
        };
        _getAllResponse = await _apiClient.GetAllAsync<GetPromotionTemplatesRequest, GetPromotionTemplatesResponse>(request);
    }

    [Then(@"GetAll response should return templates")]
    public void ThenGetAllResponseShouldReturnTemplates()
    {
        _getAllResponse.ShouldNotBeNull();
        _getAllResponse.Items.Should().NotBeNullOrEmpty();
        _getAllResponse.ItemCount.Should().BeGreaterOrEqualTo(TestPromotionTemplates.Count);
    }

    [Then(@"GetAll response should include the following templates:")]
    public void ThenGetAllResponseShouldContainKey(Table table)
    {
        foreach (var key in table.Rows)
        {
            _getAllResponse.Items.Where(x => x.TemplateKey == key[0]).Should().NotBeNullOrEmpty();
        }
    }

    [When(@"I submit Create request with template key '(.*)'")]
    public async Task WhenISubmitCreateRequestWithTemplateKey(string key)
    {
        var request = PromotionTemplateBuilder.CreateCreationRequest(x =>
        {
            x.TemplateKey = CommonSteps.Prefix(key);
            x.Comments = key;
        });

        await _apiClient.PostAsync(request);
    }

    [When(@"I submit CreateMapping request for following promotions:")]
    public async Task WhenISubmitCreateMappingRequestForFollowingPromotions(Table table)
    {
        foreach (var row in table.Rows)
        {
            var (request, key) = CreatePromotionTemplateMapRequest(row);

            await _apiClient.PutAsync($"{key}/mapping", request);

            KafkaConsumerManager.Wait<RewardTemplateUpdated>(DomainConstants.TopicProfileTemplateUpdates, (c) =>
            {
                var message = c.Value;

                return message.TemplateKey == key && message.Enabled && EnumerableShouldMatch(message.Rewards.Select(x => x.RewardRn), request.RewardRns);

            });

            //there is async clearing of cache (using Task.Run) in 'mapping' call in WebApi, due to which,
            //if we call GetEntitlements immediately after this,
            //there is a chance that we will get cached result which will be unexpected for the scenario, resulting in failed acceptance test.
            //In real world scenario, it is acceptable. In acceptance tests, the tests may fail sporadically.
            //TO safeguard against it, we are sleeping here.
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }
    }

    [When(@"I submit CreateMapping request for following rewards that do not exist:")]
    public async Task WhenISubmitCreateMappingRequestForFollowingRewardsThatDoNotExist(Table table)
    {
        foreach (var row in table.Rows)
        {
            var (request, key) = CreatePromotionTemplateMapRequest(row);

            await _apiClient.PutAsync($"{key}/mapping", request);


            Assert.Throws<TimeoutException>(() =>
            {
                KafkaConsumerManager.Wait<RewardTemplateUpdated>(DomainConstants.TopicProfileTemplateUpdates, (c) =>
                {
                    var message = c.Value;

                    return message.TemplateKey == key && message.Enabled && EnumerableShouldMatch(message.Rewards.Select(x => x.RewardRn), request.RewardRns);
                }, 5000);
            });

            //there is async clearing of cache (using Task.Run) in 'mapping' call in WebApi, due to which,
            //if we call GetEntitlements immediately after this,
            //there is a chance that we will get cached result which will be unexpected for the scenario, resulting in failed acceptance test.
            //In real world scenario, it is acceptable. In acceptance tests, the tests may fail sporadically.
            //TO safeguard against it, we are sleeping here.
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }
    }

    [Then(@"Get response should return template '(.*)' with mapped promotions '(.*)'")]
    public void ThenGetResponseShouldReturnTemplateWithMappedPromotions(string templateKey, List<string> promotionNames)
    {
        var expectedPromotions = promotionNames.Select(CommonSteps.Prefix);

        _getResponse.ShouldNotBeNull();
        _getResponse.TemplateKey.Should().Be(CommonSteps.Prefix(templateKey));
        _getResponse.Rewards.Select(p => p.Name).Should().BeEquivalentTo(expectedPromotions);
    }

    [When(@"I submit Delete request for template key '(.*)'")]
    public async Task WhenISubmitDeleteRequestForTemplateKey(string templateKey)
    {
        IEnumerable<string> rewardRns = new List<string>();

        await WhenISubmitGetRequestForTemplateKeyForActiveStateIs(templateKey, "true");

        if (_getResponse?.Rewards.Count >= 0)
        {
            rewardRns = _getResponse.Rewards.Select(x => x.RewardRn);
        }

        await _apiClient.DeleteAsync(CommonSteps.Prefix(templateKey));

        KafkaConsumerManager.Wait<RewardTemplateUpdated>(DomainConstants.TopicProfileTemplateUpdates, (c) =>
        {
            var message = c.Value;

            return message.TemplateKey == CommonSteps.Prefix(templateKey) && message.Enabled == false;
        }, 5000);
    }

    [When(@"I submit CreatePromotion request for following promotions:")]
    public async Task WhenISubmitCreatePromotionRequestForFollowingPromotions(Table table)
    {
        var testPromotions = new Dictionary<string, RewardApiModel>();

        foreach (var row in table.Rows)
        {
            var request = RewardRequestBuilder.CreateRewardRequestForType<RewardRequest>(RewardType.Uniboost, p =>
                {
                    p.Name = CommonSteps.Prefix(row["promotion_name"]);
                    p.Restrictions.ClaimsPerInterval = int.Parse(row["claim_limit"]);
                    p.Restrictions.StartDateTime = DateTime.UtcNow.AddSeconds(
                        row.GetValueFromRow("starts_in_seconds", -1));

                    if (row.ContainsKeyThatIsNotEmpty("claim_interval"))
                    {
                        p.Restrictions.ClaimInterval = row["claim_interval"];
                    }
                });
            request.RewardType = Core.WebApi.Enums.RewardType.Uniboost.ToString();

            var created = await _promotionClient.PostAsync<RewardRequest, RewardApiModel>(request);

            created.ShouldNotBeNull();
            created.RewardRn.Should().NotBeNullOrEmpty();
            testPromotions.Add(request.Name, created);
        }

        TestRewards = testPromotions;
    }

    [Then(@"the template key '(.*)' should be disabled")]
    public void ThenTheTemplateKeyShouldBeDisabled(string templateKey)
    {
        _getResponse.ShouldNotBeNull();
        _getResponse.TemplateKey.Should().Be(CommonSteps.Prefix(templateKey));
        _getResponse.Enabled.Should().BeFalse();
    }

    [Then(@"GetAll response should not return template key '(.*)'")]
    public void ThenGetAllResponseShouldNotReturnTemplateKey(string templateKey)
    {
        _getAllResponse.ShouldNotBeNull();
        _getAllResponse.Items.Should().NotBeNullOrEmpty();
        _getAllResponse.Items.Should().NotContain(p => p.TemplateKey == CommonSteps.Prefix(templateKey));
    }

    private (UpdatePromotionTemplateMapRequest request, string key) CreatePromotionTemplateMapRequest(TableRow row)
    {
        var templateKey = CommonSteps.Prefix(row["template_key"]);
        var promotionNames = row["promotions"].ExtractValues().Select(CommonSteps.Prefix).ToList();
        var rewardRns = new List<string>();

        foreach (var name in promotionNames)
        {
            var promotion = TestRewards.TryGetAndReturn(name);

            var rn = row.GetValueFromRow<string>(TableColumns.RewardRn, default);

            rewardRns.Add(promotion?.RewardRn ?? rn);
        }

        return (new()
        {
            RewardRns = rewardRns
        }, templateKey);
    }

    private static bool EnumerableShouldMatch(IEnumerable<string> first, IEnumerable<string> second)
    {
        var scope = new AssertionScope();
        first.Should().BeEquivalentTo(second);

        return !scope.HasFailures();
    }

    [Given(@"I submit CreatePromotion request by user '([^']*)'")]
    public async Task GivenISubmitCreatePromotionRequestByUser(string username, Table table)
    {
        var testTemplates = new Dictionary<string, RewardTemplateResponse>();

        foreach (var row in table.Rows)
        {
            var request = PromotionTemplateBuilder.CreateCreationRequest(x =>
            {
                x.TemplateKey = CommonSteps.Prefix(row["key"]);
                x.Comments = CommonSteps.Prefix(row["comments"]);
                x.Title = CommonSteps.Prefix(row["title"]);
            });

            var jwt = new JwtSecurityToken();
            jwt.Payload[AuthConstants.AuthorisationUsername] = username;

            var headers = new Dictionary<string, string> { { AuthConstants.RequestHeader, jwt.ToString() } };

            var template = await _apiClient.PostAsync<CreateRewardTemplateRequest, RewardTemplateResponse>(request, headers);

            template.Should().BeEquivalentTo(new
            {
                request.TemplateKey,
                request.Comments,
                request.Title,
                username,
                Enabled = true,
            });


            KafkaConsumerManager.Wait<RewardTemplateCreated>(DomainConstants.TopicProfileTemplateUpdates, (c) =>
            {
                var message = c.Value;
                return message.TemplateKey == request.TemplateKey && message.Enabled == true && message.Comments == request.Comments;

            });

            testTemplates.Add(request.TemplateKey, new() { TemplateKey = request.TemplateKey, Comments = request.Comments });
            TestPromotionTemplates = testTemplates;
        }
    }

    [Then(@"Get response should return expected createdBy field with value '([^']*)'")]
    public void ThenGetResponseShouldReturnExpectedCreatedByFieldWithValue(string username)
    {
        _getResponse.CreatedBy.Should().Be(username);
    }

    [Given(@"I submit MapPromotion request by user '([^']*)'")]
    public async Task GivenISubmitMapPromotionRequestByUser(string username, Table table)
    {
        var jwt = new JwtSecurityToken();
        jwt.Payload[AuthConstants.AuthorisationUsername] = username;
        var headers = new Dictionary<string, string> { { AuthConstants.RequestHeader, jwt.ToString() } };

        foreach (var row in table.Rows)
        {
            var (request, key) = CreatePromotionTemplateMapRequest(row);

            await _apiClient.PutAsync($"{key}/mapping", request, headers);

            KafkaConsumerManager.Wait<RewardTemplateUpdated>(DomainConstants.TopicProfileTemplateUpdates, (c) =>
            {
                var message = c.Value;

                return message.TemplateKey == key && message.Enabled && EnumerableShouldMatch(message.Rewards.Select(x => x.RewardRn), request.RewardRns);

            });

            //there is async clearing of cache (using Task.Run) in 'mapping' call in WebApi, due to which,
            //if we call GetEntitlements immediately after this,
            //there is a chance that we will get cached result which will be unexpected for the scenario, resulting in failed acceptance test.
            //In real world scenario, it is acceptable. In acceptance tests, the tests may fail sporadically.
            //TO safeguard against it, we are sleeping here.
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }
    }

    [Then(@"Get response should return expected updatedBy with value '([^']*)'")]
    public void ThenGetResponseShouldReturnExpectedUpdatedByWithValue(string username)
    {
        _getResponse.UpdatedBy.Should().Be(username);
    }

}
