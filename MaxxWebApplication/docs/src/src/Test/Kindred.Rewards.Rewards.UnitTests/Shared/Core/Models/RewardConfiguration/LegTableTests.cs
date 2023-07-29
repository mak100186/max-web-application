using System.Collections;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models.Events;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Models.RewardConfiguration;
public class LegTableTests
{
    public static IEnumerable LegTableDataFactory
    {
        get
        {
            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms,
                        Stages = new List<CompoundStage>
                        {
                            new()
                        }
                    })
                .SetName("1 stage")
                .Returns(10);

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms,
                        Stages = new List<CompoundStage>
                        {
                            new(),
                            new()
                        }
                    })
                .SetName("2 stages")
                .Returns(10);

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms,
                        Stages = new List<CompoundStage>
                        {
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("3 stages")
                .Returns(10);

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms,
                        Stages = new List<CompoundStage>
                        {
                            new(),
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("4 stages")
                .Returns(15);

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms,
                        Stages = new List<CompoundStage>
                        {
                            new(),
                            new(),
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("5 stages")
                .Returns(15);

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms,
                        Stages = new List<CompoundStage>
                        {
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("6 stages")
                .Returns(15);
            ;

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms,
                        Stages = new List<CompoundStage>
                        {
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("7 stages")
                .Returns(20);

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms,
                        Stages = new List<CompoundStage>
                        {
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("8 stages")
                .Returns(20);

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms,
                        Stages = new List<CompoundStage>
                        {
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("9 stages")
                .Returns(20);

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms,
                        Stages = new List<CompoundStage>
                        {
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("10 stages")
                .Returns(0);

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms,
                        Stages = new List<CompoundStage>
                        {
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("11 stages")
                .Returns(0);

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Formula = "fourfolds",
                        Type = RewardType.Profitboost,
                        Terms = RewardBuilder.ProfitBoostTerms,
                        Stages = new List<CompoundStage>
                        {
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("20 stages")
                .Returns(0);
        }
    }

    [Test]
    [TestCaseSource(typeof(LegTableTests), nameof(LegTableDataFactory))]
    public decimal GetLegDefinition_ShouldReturn_CorrectLegDefinitionGivenBetTypeAndStageCount(RewardBet rewardBet)
    {
        // Act / Assert
        return LegTable
            .GetLegTable(rewardBet.Terms.RewardParameters, rewardBet.GetBetType())
            .GetLegDefinition(rewardBet.GetStageCount())
            .Value;
    }
}
