using FluentAssertions;

using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Responses;
using Kindred.Rewards.Plugin.Reward.Models;
using Kindred.Rewards.Rewards.Tests.Common.Extensions;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Kindred.Rewards.Rewards.FunctionalTests.Steps;

[Binding]
public class PromotionsSteps : RewardStepsBase<RewardApiModel, GetRewardResponse, GetRewardsRequest, GetRewardsResponse, RewardRequest>
{
    private GetAllowedMarketTypesResponse _getAllowedMarketTypesResponse;

    public PromotionsSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
    {
    }

    [Given(@"I have promotions in the system")]
    public async Task GivenIHavePromotionsInTheSystem(Table table)
    {
        await GivenIHaveRewardsInTheSystem(table, false);
    }

    [When(@"I submit GetPromotion request for '(.*)'")]
    public async Task WhenISubmitGetPromotionRequestFor(string promoName)
    {
        await WhenISubmitGetRewardRequestFor(promoName);
    }

    [When(@"I submit GetPromotion request for reward key '(.*)'")]
    public async Task WhenISubmitGetPromotionRequestForRewardRn(string rewardRn)
    {
        await WhenISubmitGetRewardRequestForRewardRn(rewardRn);
    }

    [Then(@"the GetPromotion response should return promotion '(.*)'")]
    public void ThenTheGetPromotionResponseShouldReturnPromotion(string promoName)
    {
        ThenTheGetRewardResponseShouldReturnReward(promoName);
    }

    [When(@"I submit GetPromotions request for following criteria")]
    public async Task WhenISubmitGetPromotionsRequestForFollowingCriteria(Table table)
    {
        await WhenISubmitGetRewardsRequestForFollowingCriteria(table);
    }

    [Then(@"the GetPromotions response should return promotion '(.*)'")]
    public void ThenTheGetPromotionsResponseShouldReturnPromotion(string promoName)
    {
        ThenTheGetRewardsResponseShouldReturnReward(promoName);
    }

    [Then(@"the GetPromotions response should not return promotion '(.*)'")]
    public void ThenTheGetPromotionsResponseShouldNotReturnPromotion(string promoName)
    {
        ThenTheGetRewardsResponseShouldNotReturnReward(promoName);
    }

    [When(@"I submit UpdatePromotion request for '(.*)'")]
    public async Task WhenISubmitUpdatePromotionRequestFor(string promoName)
    {
        await WhenISubmitUpdateRewardRequestFor(promoName);
    }

    [When(@"I UpdatePromotion for '(.*)'")]
    public async Task WhenIUpdatePromotionFor(string promoName)
    {
        await WhenIUpdateRewardFor(promoName);
    }

    [When(@"I submit UpdatePromotion request for '(.*)' with invalid parameters")]
    public async Task WhenISubmitUpdatePromotionRequestForWithInvalidParameters(string promoName)
    {
        await WhenISubmitUpdateRewardRequestForWithInvalidParameters(promoName);
    }

    [When(@"I submit UpdatePromotion request for '(.*)' with invalid multileg")]
    public async Task WhenISubmitUpdatePromotionRequestForWithInvalidMultileg(string promoName)
    {
        await WhenISubmitUpdateRewardRequestForWithInvalidMultileg(promoName);
    }

    [Then(@"the UpdatePromotion response should return updated promotion")]
    public void ThenTheUpdatePromotionResponseShouldReturnUpdatedPromotion()
    {
        ThenTheUpdateRewardResponseShouldReturnUpdatedReward();
    }

    [When(@"I submit CancelPromotion request for '(.*)'")]
    public async Task WhenISubmitCancelPromotionRequestFor(string promoName)
    {
        await WhenISubmitCancelRewardRequestFor(promoName);
    }

    [Then(@"the CancelPromotion response should return cancelled promotion")]
    public void ThenTheCancelPromotionResponseShouldReturnCancelledPromotion()
    {
        ThenTheCancelRewardResponseShouldReturnCancelledReward();
    }

    [When(@"I submit CreatePromotion request for '(.*)'")]
    public async Task WhenISubmitCreatePromotionRequestFor(string promoName)
    {
        await WhenISubmitCreateRewardRequestFor(promoName, false);
    }

    [When(@"I submit GetPromotions request with name '(.*)'")]
    public async Task WhenISubmitGetPromotionsRequestWithName(string promoName)
    {
        await WhenISubmitGetRewardsRequestWithNameOrCustomerId(promoName, null);
    }

    [Then(@"GetPromotions response should return all promotions with name '(.*)'")]
    public void ThenGetPromotionsResponseShouldReturnAllPromotionsWithName(string promoName)
    {
        ThenGetRewardsResponseShouldReturnAllRewardsWithName(promoName);
    }

    [Then(@"the GetPromotion response should return promotion templates '(.*)'")]
    public void ThenTheGetPromotionResponseShouldReturnPromotionTemplates(List<string> expectedTemplateKeys)
    {
        GetRewardResponse.ShouldNotBeNull();
        GetRewardResponse.PromotionRns.Should().NotBeNullOrEmpty();

        foreach (var prefixedTemplateKey in expectedTemplateKeys.Select(CommonSteps.Prefix))
        {
            GetRewardResponse.PromotionRns.Should().Contain(t => t == prefixedTemplateKey, $"Expected template key {prefixedTemplateKey} not found");
        }
    }

    [Then(@"the GetPromotions response should return promotion templates '(.*)' for promotion '(.*)'")]
    public void ThenTheGetPromotionsResponseShouldReturnPromotionTemplates(List<string> expectedTemplateKeys, string promoName)
    {
        var promo = GetRewardsResponse.Items.SingleOrDefault(p => p.Reporting.Name == CommonSteps.Prefix(promoName));
        Assert.NotNull(promo);

        foreach (var expectedTemplateKey in expectedTemplateKeys.Select(CommonSteps.Prefix))
        {
            promo.PromotionRns.Should().Contain(t => t == expectedTemplateKey, $"Expected template key {expectedTemplateKey} not found");
        }
    }

    [Given(@"I submit UpdatePromotion request for '(.*)' to expire")]
    public async Task GivenISubmitUpdatePromotionRequestForToExpire(string promoName)
    {
        await GivenISubmitUpdateRewardRequestForToExpire(promoName);
    }

    [Then(@"GetPromotions response should not return promotions with name '(.*)'")]
    public void ThenGetPromotionsResponseShouldNotReturnPromotionsWithName(string promoName)
    {
        ThenTheGetRewardsResponseShouldNotReturnReward(promoName);
    }

    [When(@"I submit GetPromotionsDefaults Request for type '(.*)'")]
    public async Task WhenISubmitGetPromotionsDefaultsRequestForType(string type)
    {
        _getAllowedMarketTypesResponse = await ApiClient.GetAsync<GetAllowedMarketTypesResponse>($"{type}/defaults");
    }


    [When(@"I submit CreatePromotion request for UniboostReload with the following reload options")]
    public async Task WhenISubmitCreatePromotionRequestForUniboostReloadWithTheFollowingReloadOptions(Table table)
    {
        await WhenISubmitCreateRewardRequestForReloadableBonusWithFollowingReloadOptions(table);
    }

    [Then(@"UniboostReload promotion should have been created")]
    public void ThenUniboostReloadPromotionShouldHaveBeenCreated()
    {
        ThenUniboostReloadRewardShouldHaveBeenCreated();
    }
}
