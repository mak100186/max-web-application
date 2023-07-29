using System.Collections;

using FluentAssertions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models.Events;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.Tests.Common.Extensions.DataModifiers;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Models.Events;

internal class RewardBetTests
{
    public static IEnumerable RewardBetDataFactory
    {
        get
        {
            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms.WithMinimumOdds(1.4m),
                        Stages = new List<CompoundStage>
                        {
                            new() { OriginalOdds = 1.5m }, new() { OriginalOdds = 0.5m }
                        }
                    },
                    new List<CompoundStage> { new() { OriginalOdds = 1.5m } }
                )
                .SetName("2 stages with one above the configured minimum stage odds");

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms.WithMinimumOdds(1.0m),
                        Stages = new List<CompoundStage> { new() { OriginalOdds = 0.5m }, new() { OriginalOdds = 0.4m } }
                    },
                    new List<CompoundStage>()
                )
                .SetName("2 stages, none above configured minimum stage odd");

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms,
                        Stages = new List<CompoundStage>
                        {
                            new() { OriginalOdds = 0.5m }, new() { OriginalOdds = 0.4m }
                        }
                    },
                    new List<CompoundStage> { new() { OriginalOdds = 0.5m }, new() { OriginalOdds = 0.4m } }
                )
                .SetName("2 stages, with no minimum stage odd set");
        }
    }

    [Test]
    [TestCaseSource(typeof(RewardBetTests), nameof(RewardBetDataFactory))]
    public void StagesAboveMinimumOddRestriction_ShouldReturn_AllStagesAboveMinimumStageOdds(RewardBet rewardBet, List<CompoundStage> expected)
    {
        // Act
        var result = rewardBet.StagesAboveMinimumOddRestriction();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
