using System.Collections;

using AutoFixture;

using AutoMapper;

using FluentAssertions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Events;
using Kindred.Rewards.Core.Models.RewardClaims.Bet;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Plugin.Claim.Models.Dto;
using Kindred.Rewards.Plugin.Claim.Services.Strategies;
using Kindred.Rewards.Plugin.Claim.Services.Strategies.Base;
using Kindred.Rewards.Rewards.Tests.Common;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.Tests.Common.Extensions.DataModifiers;
using Kindred.Rewards.Rewards.UnitTests.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Plugins.Claim.Strategies;

internal class ProfitBoostClaimStrategyTests : TestBase
{
    private static IEnumerable ProfitBoostDataFactory
    {
        get
        {

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 5,
                        Formula = "doubles",
                        Type = RewardType.Profitboost,
                        BetOutcome = BetOutcome.Winning,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 2m,
                                Selection = new() { Outcome = "4-342-34-2-342" }
                            }
                        },
                        Terms = RewardBuilder.ProfitBoostTerms
                            .WithLegTable(new() { { "1", "100" } })
                            .WithMaxStake(9999)
                    })
                .SetName("RewardPayout with one stage")
                .Returns(new { RewardPaymentAmount = new decimal?(5m), BoostedOdds = new decimal?(3m) });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 3,
                        Formula = "doubles",
                        Type = RewardType.Profitboost,
                        BetOutcome = BetOutcome.Winning,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 3.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 3.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            }
                        },
                        Terms = RewardBuilder.ProfitBoostTerms
                            .WithLegTable(new() { { "2", "10" } })
                            .WithMaxStake(9999)
                    })
                .SetName("RewardPayout with two stages")
                .Returns(new { RewardPaymentAmount = new decimal?(3.375m), BoostedOdds = new decimal?(13.375m) });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 3,
                        Formula = "trebles",
                        Type = RewardType.Profitboost,
                        BetOutcome = BetOutcome.Winning,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 3.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 3.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 1.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            }
                        },
                        Terms = RewardBuilder.ProfitBoostTerms
                            .WithMinimumOdds(minimumStageOdds: 1)
                            .WithLegTable(new() { { "3", "10" } })
                            .WithMaxStake(9999)
                    })
                .SetName("RewardPayout with three stages")
                .Returns(new { RewardPaymentAmount = new decimal?(5.2125m), BoostedOdds = new decimal?(20.1125m) });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 3,
                        Formula = "trebles",
                        Type = RewardType.Profitboost,
                        BetOutcome = BetOutcome.Winning,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 3.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 3.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 10.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            }
                        },
                        Terms = RewardBuilder.ProfitBoostTerms
                            .WithMinimumOdds(minimumStageOdds: 2)
                            .WithMaxExtraWinnings(maxExtraWinnings: 30)
                            .WithMaxStake(9999)
                    })
                .SetName("RewardPayout with three stages, capped by max extra winnings")
                .Returns(new { RewardPaymentAmount = new decimal?(30m), BoostedOdds = new decimal?(138.625m) });

            // TODO Determine if this boosted odds is correct, it is wrong because the winnings are capped.
            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 3,
                        Formula = "sevenfolds",
                        Type = RewardType.Profitboost,
                        BetOutcome = BetOutcome.Winning,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 3.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 3.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 10.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 3.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 3.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 10.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 10.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            }
                        },
                        Terms = RewardBuilder.ProfitBoostTerms
                            .WithMinimumOdds(minimumStageOdds: 2)
                            .WithMaxExtraWinnings(100)
                            .WithLegTable(new() { { "1", "10" }, { "2", "10" }, { "4", "15" }, { "7", "20" }, { "10", "0" } })
                    })
                .SetName("RewardPayout with seven stages, capped by max extra winnings")
                .Returns(new { RewardPaymentAmount = new decimal?(100m), BoostedOdds = new decimal?(173749.43489583333333333333333m) });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 3,
                        Formula = "sevenfolds",
                        Type = RewardType.Profitboost,
                        BetOutcome = BetOutcome.Winning,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 3.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 2m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 1m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 1m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 1m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 1m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 1m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            }
                        },
                        Terms = RewardBuilder.ProfitBoostTerms.WithMinimumOdds(1)
                    })
                .SetName("RewardPayout with seven stages, not capped by maxwin")
                .Returns(new { RewardPaymentAmount = new decimal?(3.60m), BoostedOdds = new decimal?(8.20m) });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 3,
                        Formula = "doubles",
                        Type = RewardType.Profitboost,
                        BetOutcome = BetOutcome.Winning,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 3.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            }
                        },
                        Terms = RewardBuilder.ProfitBoostTerms
                            .WithMinimumOdds(minimumStageOdds: 5)
                    })
                .SetName("RewardPayout with two stages and one stage falling below the minimum odd set for the reward.")
                .Returns(new { RewardPaymentAmount = new decimal?(1.2m), BoostedOdds = new decimal?(5.4m) });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 3,
                        Formula = "doubles",
                        Type = RewardType.Profitboost,
                        BetOutcome = BetOutcome.Winning,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 1.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 1.0m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            }
                        },
                        Terms = RewardBuilder.ProfitBoostTerms.WithMinimumOdds(minimumStageOdds: 5)
                    })
                .SetName("RewardPayout with two stages and all stages fall below the set minimum odds.")
                .Returns(new { RewardPaymentAmount = new decimal?(), BoostedOdds = new decimal?() });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 10000,
                        Formula = "doubles",
                        Type = RewardType.Profitboost,
                        BetOutcome = BetOutcome.Winning,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 15m,
                                Selection = new() { Outcome = "4-342-34-2-342" }
                            }
                        },
                        Terms = RewardBuilder.ProfitBoostTerms
                            .WithMaxStake(10001)
                            .WithLegTable(new() { { "1", "10" } })
                    })
                .SetName("RewardPayout with one stage and leg table is configured to boost bets with 1 stage by 10%.")
                .Returns(new { RewardPaymentAmount = new decimal?(14000m), BoostedOdds = new decimal?(16.4m) });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 10000,
                        Formula = "doubles",
                        Type = RewardType.Profitboost,
                        BetOutcome = BetOutcome.Winning,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 15m,
                                Selection = new() { Outcome = "4-342-34-2-342" }
                            }
                        },
                        Terms = RewardBuilder.ProfitBoostTerms
                            .WithMaxStake(100001)
                            .WithMaxExtraWinnings(maxExtraWinnings: 5000)
                            .WithLegTable(new() { { "1", "10" } })
                    })
                .SetName("RewardPayout with one stage, max extra winnings set to 5000 and leg table is configured to boost bets with 1 stage by 10%.")
                .Returns(new { RewardPaymentAmount = new decimal?(5000m), BoostedOdds = new decimal?(15.5m) });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 100,
                        Formula = "doubles",
                        Type = RewardType.Profitboost,
                        BetOutcome = BetOutcome.Winning,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 2m,
                                Selection = new() { Outcome = "4-342-34-2-342" }
                            }
                        },
                        Terms = RewardBuilder.ProfitBoostTerms
                            .WithMaxStake(50)
                            .WithLegTable(new() { { "1", "10" } })
                    })
                .SetName("RewardPayout with one stage, stake is above the max stake of 50 and leg table is configured to boost bets with 1 stage by 10%.")
                .Returns(new { RewardPaymentAmount = new decimal?(5m), BoostedOdds = new decimal?(2.05m) });
        }
    }

    private string _claimInstanceId;
    private ProfitBoostClaimStrategy _atTest;

    [SetUp]
    public void SetUp()
    {
        var context = new RewardsDbContextMock(new DbContextOptionsBuilder<RewardsDbContextMock>()
            .UseInMemoryDatabase(databaseName: "RewardsDbContext")
            .Options);

        _claimInstanceId = Guid.NewGuid().ToString();
        var claim = Mapper.Map<RewardClaim>(RewardClaimBuilder.CreateBonusClaim(RewardType.Profitboost, model =>
        {
            model.InstanceId = _claimInstanceId;
        }));

        context.RewardClaims.Add(claim);
        context.SaveChanges();

        _atTest = new(context, Mapper, new Mock<ILogger<ProfitBoostClaimStrategy>>().Object);
    }

    [Test]
    public async Task CalculateRewardPaymentAmountAsync_WhenCombinationHasOneStage_ReturnsBoosted_ForOneStage()
    {
        // Arrange
        var betRn = Guid.NewGuid().ToString();
        var customerId = "123456";

        // Act
        var result = await _atTest.ProcessClaimAsync(new SettleClaimParameterDto
        {
            BetPayoff = 0,
            BetRn = betRn,
            CombinationRn = Guid.NewGuid().ToString(),
            AggregatedSegmentStatus = SettlementSegmentStatus.Won,
            CombinationSettlementStatus = SettlementCombinationStatus.Resolved,
            CustomerId = customerId,
            Formula = "Singles",
            RewardClaim = new()
            {
                Id = 1,
                Terms = new()
                {
                    RewardParameters = new Dictionary<string, string>
                    {
                        { RewardParameterKey.LegTable, "{\"1\":\"10\",\"2\":\"10\",\"4\":\"15\",\"7\":\"20\",\"10\":\"0\"}" },
                        { RewardParameterKey.MaxStakeAmount, "5" },
                    },
                },
                InstanceId = _claimInstanceId,
                BetRn = betRn,
                CouponRn = Guid.NewGuid().ToString(),
                CustomerId = customerId,
                RewardId = Guid.NewGuid().ToString(),
                PromotionName = "Test"
            },
            CombinationStages = new List<CompoundStageDomainModel>
            {
                new()
                {
                    ContestType = "Soccer",
                    Market = "Market",
                    RequestedOutcome = "ksp:outcome.1[football:202110042300:clube_do_remo_pa_vs_coritiba_fc_pr]:1x2:plain:clube_do_remo_pa",
                    RequestedPrice = 1.5M,
                }
            },
            BetStake = 16,
            CombinationStake = 5
        });

        // Assert
        result.Payoff.Should().Be(0.25M);
    }


    [Test]
    public async Task CalculateRewardPaymentAmountAsync_WhenCombinationHasTwoStages_ReturnsBoosted_ForTwoStages()
    {
        // Arrange
        var betRn = Guid.NewGuid().ToString();
        var customerId = "123456";

        // Act
        var result = await _atTest.ProcessClaimAsync(new SettleClaimParameterDto
        {
            BetPayoff = 0,
            BetRn = betRn,
            CombinationRn = Guid.NewGuid().ToString(),
            AggregatedSegmentStatus = SettlementSegmentStatus.Won,
            CombinationSettlementStatus = SettlementCombinationStatus.Resolved,
            CustomerId = customerId,
            Formula = "Double",
            RewardClaim = new()
            {
                Id = 1,
                Terms = new()
                {
                    RewardParameters = new Dictionary<string, string>
                    {
                        { RewardParameterKey.LegTable, "{\"1\":\"10\",\"2\":\"10\",\"4\":\"15\",\"7\":\"20\",\"10\":\"0\"}" },
                        { RewardParameterKey.MaxStakeAmount, "5" },
                    },
                },
                InstanceId = _claimInstanceId,
                BetRn = betRn,
                CouponRn = Guid.NewGuid().ToString(),
                CustomerId = customerId,
                RewardId = Guid.NewGuid().ToString(),
                PromotionName = "Test"
            },
            CombinationStages = new List<CompoundStageDomainModel>
            {
                new()
                {
                    ContestType = "Soccer",
                    Market = "Market",
                    RequestedOutcome = "ksp:outcome.1[football:202110042300:clube_do_remo_pa_vs_coritiba_fc_pr]:1x2:plain:clube_do_remo_pa",
                    RequestedPrice = 1.5M
                },
                new()
                {
                    ContestType = "Soccer",
                    Market = "Market",
                    RequestedOutcome = "ksp:outcome.1[football:202110042300:clube_do_remo_pa_vs_coritiba_fc_pr]:1x2:plain:clube_do_remo_pa",
                    RequestedPrice = 10M
                }
            },
            BetStake = 16,
            CombinationStake = 5
        });

        // Assert
        result.Payoff.Should().Be(7M);
    }

    [Test]
    [TestCaseSource(typeof(ProfitBoostClaimStrategyTests), nameof(ProfitBoostDataFactory))]
    public async Task<dynamic> CalculateRewardPaymentAmountAsync_ShouldReturnCorrectBoostedOdds(RewardBet bet)
    {
        // Arrange
        var scope = new TestScope();

        // Act
        await scope.RewardStrategy.ProcessClaimAsync(bet);

        // Assert
        return new
        {
            bet.RewardPaymentAmount,
            bet.BoostedOdds
        };
    }

    [Test]
    public async Task ProcessClaimAsync_RewardPaymentAmountShouldBeNull_WhenCalledForLosingBet()
    {
        // Arrange
        var scope = new TestScope();
        var bet = new RewardBet { BetOutcome = BetOutcome.Losing };

        // Act
        await scope.RewardStrategy.ProcessClaimAsync(bet);

        // Assert
        bet.RewardPaymentAmount.Should().BeNull();
    }

    private class TestScope
    {
        public TestScope()
        {
            var fixture = new Fixture();
            var logger = fixture.Freeze<Mock<ILogger<ProfitBoostClaimStrategy>>>();
            var mapper = new Mock<IMapper>();
            var context = new RewardsDbContextMock(new DbContextOptionsBuilder<RewardsDbContextMock>()
                .UseInMemoryDatabase(databaseName: "RewardsDbContext")
                .Options);

            TestReward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Profitboost, null);

            RewardStrategy = new ProfitBoostClaimStrategy(context, mapper.Object, logger.Object);
        }

        public RewardClaimStrategyBase RewardStrategy { get; }

        public RewardDomainModel TestReward { get; }
    }
}
