using System.Web;

using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Responses;
using Kindred.Rewards.Plugin.Reward.Models;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Kindred.Rewards.Rewards.FunctionalTests.Steps;

[Binding]
public class BonusesSteps : RewardStepsBase<RewardApiModel, GetRewardResponse, GetRewardsRequest, GetRewardsResponse, RewardRequest>
{
    private DateTimeOffset? _updateTimeFrom;
    private DateTimeOffset? _updateTimeTo;

    public BonusesSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
    {
    }

    [Given(@"I have bonuses in the system")]
    public async Task GivenIHaveBonusesInTheSystem(Table table)
    {
        await GivenIHaveRewardsInTheSystem(table);
    }

    [When(@"I submit GetBonus request for '(.*)'")]
    public async Task WhenISubmitGetBonusRequestFor(string promoName)
    {
        await WhenISubmitGetRewardRequestFor(promoName);
    }

    [When(@"I submit GetBonus request for reward key '(.*)'")]
    public async Task WhenISubmitGetBonusRequestForRewardRn(string rewardRn)
    {
        await WhenISubmitGetRewardRequestForRewardRn(rewardRn);
    }

    [Then(@"the GetBonus response should return bonus '(.*)'")]
    public void ThenTheGetBonusResponseShouldReturnBonus(string promoName)
    {
        ThenTheGetRewardResponseShouldReturnReward(promoName);
    }

    [When(@"I submit GetBonuses request for following criteria")]
    public async Task WhenISubmitGetBonusesRequestForFollowingCriteria(Table table)
    {
        await WhenISubmitGetRewardsRequestForFollowingCriteria(table, _updateTimeFrom, _updateTimeTo);
        _updateTimeFrom = null;
        _updateTimeTo = null;
    }

    [Then(@"the GetBonuses response should return bonus '(.*)'")]
    public void ThenTheGetBonusesResponseShouldReturnBonus(string promoName)
    {
        ThenTheGetRewardsResponseShouldReturnReward(promoName);
    }

    [Then(@"the GetBonuses response should not return bonus '(.*)'")]
    public void ThenTheGetBonusesResponseShouldNotReturnBonus(string promoName)
    {
        ThenTheGetRewardsResponseShouldNotReturnReward(promoName);
    }

    [When(@"I submit UpdateBonus request for '(.*)'")]
    public async Task WhenISubmitUpdateBonusRequestFor(string promoName)
    {
        await WhenISubmitUpdateRewardRequestFor(promoName);
    }

    [When(@"I UpdateBonus for '(.*)'")]
    public async Task WhenIUpdateBonusFor(string promoName)
    {
        await WhenIUpdateRewardFor(promoName);
    }

    [When(@"I submit UpdateBonus request for '(.*)' with '(.*)' as Customer ID")]
    protected async Task WhenISubmitUpdateBonusRequestWithCustomerId(string promoName, string customerId)
    {
        var promotion = TestRewards[CommonSteps.Prefix(promoName)];

        TestReward = promotion;

        var request = CreateCreateRewardReq();

        request.CustomerId = customerId;

        await ApiClient.PutAsync(HttpUtility.UrlEncode(TestReward.RewardRn), request);
    }

    [When(@"I submit UpdateBonus request for '(.*)' with invalid parameters")]
    public async Task WhenISubmitUpdateBonusRequestForWithInvalidParameters(string promoName)
    {
        await WhenISubmitUpdateRewardRequestForWithInvalidParameters(promoName);
    }

    [When(@"I submit UpdateBonus request for '(.*)' with invalid multileg")]
    public async Task WhenISubmitUpdateBonusRequestForWithInvalidMultileg(string promoName)
    {
        await WhenISubmitUpdateRewardRequestForWithInvalidMultileg(promoName);
    }

    [Then(@"the UpdateBonus response should return updated bonus")]
    public void ThenTheUpdateBonusResponseShouldReturnUpdatedBonus()
    {
        ThenTheUpdateRewardResponseShouldReturnUpdatedReward();
    }

    [When(@"I submit CancelBonus request for '(.*)'")]
    public async Task WhenISubmitDeleteBonusRequestFor(string promoName)
    {
        await WhenISubmitCancelRewardRequestFor(promoName);
    }

    [Then(@"the CancelBonus response should return cancelled bonus")]
    public void ThenTheCancelBonusResponseShouldReturnCancelledBonus()
    {
        ThenTheCancelRewardResponseShouldReturnCancelledReward();
    }

    [When(@"I submit CreateBonus request for '(.*)'")]
    public async Task WhenISubmitCreateBonusRequestFor(string promoName)
    {
        await WhenISubmitCreateRewardRequestFor(promoName);
    }

    [When(@"I create Bonus for following criteria:")]
    public async Task WhenICreateBonusForFollowingCriteria(Table table)
    {
        await WhenISubmitCreateBonusRequestFor(table);
    }


    [When(@"I submit GetBonuses request with customer id '(.*)'")]
    public async Task WhenISubmitGetBonusesRequestWithCustomerId(string customerId)
    {
        await WhenISubmitGetRewardsRequestWithNameOrCustomerId(null, customerId);
    }

    [Then(@"GetBonuses response should return all bonuses with name '(.*)'")]
    public void ThenGetBonusesResponseShouldReturnAllBonusesWithName(string promoName)
    {
        ThenGetRewardsResponseShouldReturnAllRewardsWithName(promoName);
    }

    [Given(@"I submit UpdateBonus request for '(.*)' to expire")]
    public async Task GivenISubmitUpdateBonusRequestForToExpire(string promoName)
    {
        await GivenISubmitUpdateRewardRequestForToExpire(promoName);
    }

    [Then(@"GetBonuses response should not return bonuses with name '(.*)'")]
    public void ThenGetBonusesResponseShouldNotReturnBonusesWithName(string promoName)
    {
        ThenTheGetRewardsResponseShouldNotReturnReward(promoName);
    }

    [When(@"I submit CreateBonus request for UniboostReload with the following reload options")]
    public async Task WhenISubmitCreateBonusRequestForUniboostReloadWithTheFollowingReloadOptions(Table table)
    {
        await WhenISubmitCreateRewardRequestForReloadableBonusWithFollowingReloadOptions(table);
    }

    [Then(@"UniboostReload bonus should have been created")]
    public void ThenUniboostReloadBonusShouldHaveBeenCreated()
    {
        ThenUniboostReloadRewardShouldHaveBeenCreated();
    }

    [Then(@"I capture the UpdateDateFrom time")]
    public void ThenICaptureTheUpdateDateFromTime()
    {
        Thread.Sleep(1000);
        _updateTimeFrom = DateTimeOffset.UtcNow;
        Thread.Sleep(500);
    }

    [Then(@"I capture the UpdateDateTo time")]
    public void ThenICaptureTheUpdateDateToTime()
    {
        Thread.Sleep(500);
        _updateTimeTo = DateTimeOffset.UtcNow;
        Thread.Sleep(1000);
    }
    [Then(@"It should contain only '(.*)' bonuses")]
    public void ThenItShouldContainOnlyBonuses(int numOfRewards)
    {
        ThenItShouldContainOnlyRewards(numOfRewards);
    }

    [When(@"I submit a patch bonus request for the following criteria for '([^']*)'")]
    public async Task WhenISubmitAPatchBonusRequestForTheFollowingCriteriaFor(string promoName, Table table)
    {
        await WhenISubmitAPatchRewardRequestForTheFollowingCriteria(promoName, table);
    }

    [When(@"I submit a null patch bonus request for '([^']*)'")]
    public async Task WhenISubmitANullPatchBonusRequestFor(string promoName)
    {
        await WhenISubmitANullPatchRewardRequestFor(promoName);
    }

    [Then(@"the patch bonus response should return updated bonus with the following")]
    public void ThenThePatchBonusResponseShouldReturnUpdatedBonusWithTheFollowing(Table table)
    {
        ThenThePatchRewardResponseShouldReturnUpdatedBonusWithTheFollowing(table);
    }

    [When(@"I create a reward")]
    public async Task GivenICreateAReward(Table table)
    {
        //step for single row table only
        Assert.AreEqual(table.Rows.Count, 1);

        await CreateReward(table.Rows.First());
    }

    [Then(@"I expect RewardCreated message with reward type '([^']*)' and customerId '([^']*)'")]
    public void ThenIExpectRewardCreatedMessageWithRewardTypeAndCustomerId(string rewardType, string customerId)
    {
        ExpectRewardCreatedMessageWithRewardTypeAndCustomerId(rewardType, customerId);
    }
}
