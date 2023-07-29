using FluentAssertions;

using Kindred.Rewards.Core.WebApi.Responses;
using Kindred.Rewards.Plugin.Claim.Models.Requests;
using Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim;
using Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim.Enums;
using Kindred.Rewards.Plugin.Claim.Models.Responses;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Helpers;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Setup;
using Kindred.Rewards.Rewards.FunctionalTests.Steps.Context;
using Kindred.Rewards.Rewards.FunctionalTests.Steps.Context.Models;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.Tests.Common.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using TechTalk.SpecFlow;

namespace Kindred.Rewards.Rewards.FunctionalTests.Steps;

[Binding]
public class ClaimLifecycleSteps : AcceptanceScenarioBase
{
    private readonly IApiClient _claimApiClient;
    private readonly BetContext _betContext;

    public ClaimLifecycleSteps(ScenarioContext scenarioContext, FeatureContext featureContext, BetContext betContext) : base(scenarioContext, featureContext
    )
    {
        _claimApiClient = new ApiClient("rewardclaims", RestClientFactory, scenarioContext, SerializerSettings.GetJsonSerializerSettings);
        _betContext = betContext;
    }

    private string InstanceId => ScenarioContext.Get<BatchClaimResponse>(nameof(BatchClaimResponse)).Responses[0].Claim.Rn;

    private BatchClaimResponse BatchClaimResponse
    {
        get => ScenarioContext.Get<BatchClaimResponse>(nameof(BatchClaimResponse));

        set => ScenarioContext.Set(value, nameof(BatchClaimResponse));
    }

    private SettleClaimResponse SettleClaimResponse
    {
        get => ScenarioContext.Get<SettleClaimResponse>(nameof(SettleClaimResponse));

        set => ScenarioContext.Set(value, nameof(SettleClaimResponse));
    }

    [Then(@"I expect to receive RewardClaimSettledEvent")]
    public void ThenIExpectToReceiveRewardClaimSettledEvent()
    {
        throw new PendingStepException();
    }

    [When(@"I settle the bet for following claim")]
    public void WhenISettleTheBetForFollowingClaim(Table table)
    {
        throw new PendingStepException();
    }

    [Then(@"I expect to receive RewardClaimUnSettledEvent")]
    public void ThenIExpectToReceiveRewardClaimUnSettledEvent()
    {
        throw new PendingStepException();
    }

    [When(@"I unsettle the bet for following claim")]
    public void WhenIUnsettleTheBetForFollowingClaim(Table table)
    {
        throw new PendingStepException();
    }


    [When(@"a request to settle the claim is received for bet '([^']*)' with final payoff '([^']*)' and combination settlement statuses:")]
    public async Task WhenARequestToSettleTheClaimIsReceivedForBetWithFinalPayoffAndCombinationSettlementStatuses(string betRn, string finalPayoff, Table table)
    {
        var bet = _betContext.ReferenceBets.Single(bet => bet.Rn == betRn);

        foreach (var row in table.Rows)
        {
            var combinationRn = row.GetValueFromRow("combinationRn", ObjectBuilder.CreateString());
            var segmentStatuses = row.GetValueFromRow("segmentStatuses", ObjectBuilder.CreateString()).Split(",");
            var settlementStatus = row.GetValueFromRow("settlementStatus", ObjectBuilder.CreateString());

            var combination = bet.Combinations.First(x => x.Rn == combinationRn);
            combination.Settlement = new()
            {
                Segments = segmentStatuses
                    .Select(segmentStatus => new CombinationSettlementSegment { Status = segmentStatus }).ToList(),
                Status = settlementStatus,
            };
        }

        bet.Settlement = new()
        {
            FinalPayoff = decimal.Parse(finalPayoff),
            Status = bet.Combinations.Any(x => x.Settlement.Status != "Resolved") ? "Pending" : "Resolved"
        };

        bet.RewardClaims = new()
        {
            new() { ClaimRn = InstanceId, RewardRn = "someRn" }
        };

        var request = new SettleClaimRequest
        {
            AcceptedCombinations = bet.Combinations.Select(combination => new CombinationPayload
            {
                Rn = combination.Rn,
                Settlement = new()
                {
                    Segments = combination.Settlement.Segments.Select(segment => new SegmentPayload
                    {
                        Status = Enum.Parse<SettlementSegmentStatus>(segment.Status)
                    }),
                    Status = Enum.Parse<SettlementCombinationStatus>(combination.Settlement.Status)
                },
                Selections = combination.Selection.Select(selection => new SelectionPayload
                {
                    Outcome = selection.Outcome
                })
            }),
            CustomerId = bet.CustomerId,
            Formula = Enum.Parse<Formula>(bet.Formula),
            RewardClaims = bet.RewardClaims.Select(claim => new RewardClaimPayload
            {
                ClaimRn = claim.ClaimRn,
                RewardRn = claim.RewardRn
            }),
            Rn = bet.Rn,
            Stages = bet.Stages.Select(stage => new CompoundStagePayload
            {
                Market = stage.Market,
                Odds = new()
                {
                    Price = stage.Odds.Price,
                },
                Selection = new()
                {
                    Outcome = stage.Selection.Outcome
                }
            }),
            Settlement = new()
            {
                FinalPayoff = bet.Settlement.FinalPayoff,
                Status = Enum.Parse<SettlementBetStatus>(bet.Settlement.Status)
            },
            Stake = bet.Stake,
        };

        SettleClaimResponse = await _claimApiClient.PostAsync<SettleClaimRequest, SettleClaimResponse>("settle", request);
    }

    [Then(@"the response should be:")]
    public void ThenTheResponseShouldBe(Table table)
    {
        foreach (var row in table.Rows)
        {
            SettleClaimResponse.ShouldNotBeNull();

            SettleClaimResponse.RewardClaimSettlement[0].Payoff.Should().Be(decimal.Parse(row["payOff"]));

            if (string.IsNullOrEmpty(row["prevPayOff"]))
            {
                SettleClaimResponse.PrevRewardClaimSettlement[0].Payoff.Should().BeNull();
            }
            else
            {
                SettleClaimResponse.PrevRewardClaimSettlement[0].Payoff.Should().Be(decimal.Parse(row["prevPayOff"]));
            }
        }
    }
}
