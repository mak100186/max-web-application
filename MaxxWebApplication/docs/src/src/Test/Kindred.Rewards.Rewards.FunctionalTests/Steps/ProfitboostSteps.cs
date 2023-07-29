using FluentAssertions;

using Kindred.Rewards.Core.Models.Messages;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Setup;

using Newtonsoft.Json;

using TechTalk.SpecFlow;

namespace Kindred.Rewards.Rewards.FunctionalTests.Steps;

[Binding]
public class ProfitboostSteps : AcceptanceScenarioBase
{

    public ProfitboostSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
    {
    }

    public ICollection<RewardClaim> RewardClaimMessages
    {
        get
        {
            if (!ScenarioContext.ContainsKey(nameof(RewardClaim)))
            {
                ScenarioContext.Set(JsonConvert.SerializeObject(new List<RewardClaim>()), nameof(RewardClaim));
            }

            return JsonConvert.DeserializeObject<List<RewardClaim>>(ScenarioContext.Get<string>(nameof(RewardClaim)));
        }

        set => ScenarioContext.Set(JsonConvert.SerializeObject(value), nameof(RewardClaim));
    }

    private decimal PayoutAmount
    {
        get => ScenarioContext.Get<decimal>(nameof(PayoutAmount));

        set => ScenarioContext.Set(value, nameof(PayoutAmount));
    }

    [Then(@"I expect the correct Payout with rewardPaymentAmount of (.*)")]
    public void ThenIExpectTheCorrectPayoutWithBoostedOddsOfAndRewardPaymentAmountOf(decimal rewardPaymentAmount)
    {
        var claims = RewardClaimMessages;
        claims.Count.Should().Be(1);
        var claimEvent = claims.First();

        claimEvent.RewardPayoutAmount.Should().Be(rewardPaymentAmount);

        PayoutAmount = rewardPaymentAmount;
    }

    [Then(@"I expect a reverse payment of the same payout amount")]
    public void ThenIExpectAReversePaymentOfTheSameAmount()
    {
        var claims = RewardClaimMessages;
        claims.Count.Should().Be(2);
        claims.Last().RewardPayoutAmount.Should().Be(-1 * PayoutAmount);
    }
}
