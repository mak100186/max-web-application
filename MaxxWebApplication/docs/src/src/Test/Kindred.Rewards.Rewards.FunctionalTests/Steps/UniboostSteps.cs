using FluentAssertions;

using Kindred.Rewards.Core.Models.Messages;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Setup;

using Newtonsoft.Json;

using TechTalk.SpecFlow;

namespace Kindred.Rewards.Rewards.FunctionalTests.Steps;

[Binding]
public class UniboostSteps : AcceptanceScenarioBase
{
    public UniboostSteps(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
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

    [Then(@"I expect the correct Payout with a payout of (.*)")]
    public void ThenIExpectTheCorrectPayoutOf(decimal rewardPayoutAmount)
    {
        var claims = RewardClaimMessages;
        claims.Count.Should().Be(1);
        var claimEvent = claims.First();
        claimEvent.RewardPayoutAmount.Should().Be(rewardPayoutAmount);

        PayoutAmount = rewardPayoutAmount;
    }

    [Then(@"I expect the correct StakeDeduction with a value of (.*)")]
    public void ThenIExpectTheCorrectStakeDeduction(decimal expectedStakeDeduction)
    {
        var claims = RewardClaimMessages;
        claims.Count.Should().Be(1);
        var claimEvent = claims.First();
        claimEvent.StakeDeduction.Should().Be(expectedStakeDeduction);
    }



    [Then(@"I expect a reverse payment of the same amount")]
    public void ThenIExpectAReversePaymentOfTheSameAmount()
    {
        var claims = RewardClaimMessages;
        claims.Count.Should().Be(2);
        claims.Last().RewardPayoutAmount.Should().Be(PayoutAmount * -1);
    }

    [BeforeScenario("uniboost")]
    [AfterScenario("uniboost")]
    public void CleanUp()
    {
        RewardClaimMessages = new List<RewardClaim>();
    }
}
