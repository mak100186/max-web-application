using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

using Kindred.Rewards.Rewards.FunctionalTests.Steps.Context;
using Kindred.Rewards.Rewards.FunctionalTests.Steps.Context.Models;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.Tests.Common.Extensions;

using TechTalk.SpecFlow;

namespace Kindred.Rewards.Rewards.FunctionalTests.Steps;

[Binding]
public class BetSteps
{
    private readonly BetContext _betContext;

    public BetSteps(BetContext betContext)
    {
        _betContext = betContext;
    }

    [Given(@"a bet '([^']*)' for customer '([^']*)' that has formulae '([^']*)' and stake '([^']*)' with the combinations:")]
    public void GivenABetThatHasFormulaeAndStakeWithTheCombinations(string betRn, string customerId, string formulae, string stake,
        Table table)
    {
        var bet = new Bet { Formula = formulae, Stake = decimal.Parse(stake) };

        foreach (var row in table.Rows)
        {
            var combinationRn = row.GetValueFromRow("combinationRn", ObjectBuilder.CreateString());
            var selectionOutcomes = row.GetValueFromRow("selectionOutcomes", ObjectBuilder.CreateString()).Split(",");

            bet.Rn = betRn;
            bet.CustomerId = CommonSteps.PrefixCustomerId(customerId).ToString();
            bet.Combinations.Add(new()
            {
                Rn = combinationRn,
                Selection = selectionOutcomes.Select(outcome => new Selection { Outcome = outcome }).ToList(),
            });
        }

        _betContext.ReferenceBets.Add(bet);
    }

    [Given(@"the bet '([^']*)' has the following stages:")]
    public void GivenTheBetHasTheFollowingStages(string betRn, Table table)
    {
        var bet = _betContext.ReferenceBets.First(bet => bet.Rn == betRn);

        foreach (var row in table.Rows)
        {
            var outcome = row.GetValueFromRow("outcome", ObjectBuilder.CreateString());
            var price = row.GetValueFromRow("price", ObjectBuilder.CreateString());
            var market = row.GetValueFromRow("market", ObjectBuilder.CreateString());

            bet.Stages.Add(new()
            {
                Market = market,
                Odds = new() { Price = decimal.Parse(price) },
                Selection = new() { Outcome = outcome }
            });
        }
    }
}
