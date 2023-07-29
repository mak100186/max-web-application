using System.Collections;

using AutoFixture;

using AutoMapper;

using FluentAssertions;

using Kindred.Rewards.Core.Client;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Events;
using Kindred.Rewards.Core.Models.RewardClaims.Bet;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Plugin.Claim.Clients.MarketMirror;
using Kindred.Rewards.Plugin.Claim.Clients.MarketMirror.Responses;
using Kindred.Rewards.Plugin.Claim.Models.Dto;
using Kindred.Rewards.Plugin.Claim.Services.Strategies;
using Kindred.Rewards.Rewards.Tests.Common;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.Tests.Common.Extensions.DataModifiers;
using Kindred.Rewards.Rewards.UnitTests.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using ContestStatus = Kindred.Rewards.Plugin.Claim.Clients.MarketMirror.Responses.ContestStatus;

namespace Kindred.Rewards.Rewards.UnitTests.Plugins.Claim.Strategies;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[Category("Unit")]
internal class UniBoostClaimStrategyTests : TestBase
{
    private const string OddsLadderValues = "1.01,1.02,1.025,1.0303030303,1.04,1.05,1.0625,1.0714285714,1.0833333333,1.1,1.1111111111,1.125,1.1428571429,1.1666666667,1.1818181818,1.2,1.2222222222,1.25,1.2857142857,1.3,1.3333333333,1.3636363636,1.4,1.4444444444,1.4705882353,1.5,1.5333333333,1.5714285714,1.6153846154,1.6666666667,1.7272727273,1.8,1.8333333333,1.9,1.9090909091,2,2.1,2.2,2.25,2.375,2.4,2.5,2.6,2.625,2.75,2.8,2.875,3,3.125,3.2,3.25,3.4,3.5,3.6,3.75,3.8,4,4.2,4.3333333333,4.4,4.5,4.6,4.8,5,5.5,6,6.5,7,7.5,8,8.5,9,9.5,10,11,12,13,15,17,19,21,23,26,29,34,41,51,67,81,101,151,201,251,301,501,1001";

    private static IEnumerable BoostedOddsResultFactory
    {
        get
        {
            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 3,
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
                            }
                        },
                        Terms = RewardBuilder.UniboostTerms
                    }, OddsLadderValues)
                .SetName("Fixed win boosted reward payout with no configured minimum odds.")
                .Returns(new { RewardPaymentAmount = new decimal?(0.9m), BoostedOdds = new decimal?(3.8m) });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 3,
                        BetOutcome = BetOutcome.Winning,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 1.5m,
                                Selection = new() { Outcome = "4-342-34-2-342" }
                            }
                        },
                        Terms = RewardBuilder.UniboostTerms.WithMinimumOdds(1.6m)
                    }, OddsLadderValues)
                .SetName("RewardPayout with one stage and it falls below the configured minimum odds.")
                .Returns(new { RewardPaymentAmount = new decimal?(), BoostedOdds = new decimal?() });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 3,
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
                                OriginalOdds = 1.5m,
                                Selection = new()
                                {
                                    Outcome = "4-342-34-2-342"
                                }
                            }
                        },
                        Terms = RewardBuilder.UniboostTerms.WithMinimumOdds(1.0m)
                    }, OddsLadderValues)
                .SetName("RewardPayout with two stages does not apply reward because Uniboost can have at most one stage.")
                .Returns(new { RewardPaymentAmount = new decimal?(), BoostedOdds = new decimal?() });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 3,
                        BetOutcome = BetOutcome.Losing,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 1.5m,
                                Selection = new() { Outcome = "4-342-34-2-342" }
                            }
                        },
                        Terms = RewardBuilder.UniboostTerms.WithMinimumOdds(1.0m)
                    }, OddsLadderValues)
                .SetName("RewardPayout with one stage does not apply reward because bet did not win.")
                .Returns(new { RewardPaymentAmount = new decimal?(), BoostedOdds = new decimal?() });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 10000,
                        Formula = "doubles",
                        Type = RewardType.Uniboost,
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
                        Terms = RewardBuilder.UniboostTerms
                            .WithMaxStake(maxStake: 10001)
                    }, OddsLadderValues)
                .SetName("RewardPayout with one stage has max stake = 10001 and odds boosted from 15 to 21 from odds table.")
                .Returns(new { RewardPaymentAmount = new decimal?(60000), BoostedOdds = new decimal?(21m) });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 10000,
                        Formula = "doubles",
                        Type = RewardType.Uniboost,
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
                        Terms = RewardBuilder.UniboostTerms
                            .WithMaxStake(maxStake: 10001)
                            .WithMaxExtraWinnings(maxExtraWinnings: 5000)
                    }, OddsLadderValues)
                .SetName("RewardPayout with one stage has max stake = 10001, max win amount = 250k, max extra winnings = 5k and odds boosted from 15 to 21 from odds table.")
                .Returns(new { RewardPaymentAmount = new decimal?(5000), BoostedOdds = new decimal?(21m) });

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 15,
                        Formula = "doubles",
                        Type = RewardType.Uniboost,
                        BetOutcome = BetOutcome.Winning,
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 23m,
                                Selection = new() { Outcome = "4-342-34-2-342" }
                            }
                        },
                        Terms = RewardBuilder.UniboostTerms
                            .WithMaxStake(maxStake: 10)
                    }, "23,35")
                .SetName(
                    "RewardPayout with one stage has max stake = 10 and odds boosted from 23 to 35 from odds table.")
                .Returns(new { RewardPaymentAmount = new decimal?(120), BoostedOdds = new decimal?(35m) });
        }
    }

    private static IEnumerable<RewardBet> ExampleRewardBets
    {
        get
        {
            yield return new()
            {
                Stake = 3,
                BetOutcome = BetOutcome.Winning,
                Stages = new List<CompoundStage>
                {
                    new()
                    {
                        ContestType = TestConstants.DefaultContestType,
                        Market = TestConstants.DefaultMarketRn,
                        OriginalOdds = 3.5m,
                        Selection = new() { Outcome = "4-342-34-2-342" }
                    }
                },
                Terms = RewardBuilder.UniboostTerms
            };

            yield return new()
            {
                Stake = 3,
                BetOutcome = BetOutcome.Winning,
                Stages = new List<CompoundStage>
                {
                    new()
                    {
                        ContestType = TestConstants.DefaultContestType,
                        Market = TestConstants.DefaultMarketRn,
                        OriginalOdds = 1.5m,
                        Selection = new() { Outcome = "4-342-34-2-342" }
                    }
                },
                Terms = RewardBuilder.UniboostTerms.WithMinimumOdds(1.6m)
            };

            yield return new()
            {
                Stake = 3,
                BetOutcome = BetOutcome.Losing,
                Stages = new List<CompoundStage>
                {
                    new()
                    {
                        ContestType = TestConstants.DefaultContestType,
                        Market = TestConstants.DefaultMarketRn,
                        OriginalOdds = 1.5m,
                        Selection = new() { Outcome = "4-342-34-2-342" }
                    }
                },
                Terms = RewardBuilder.UniboostTerms.WithMinimumOdds(1.0m)
            };

            yield return new()
            {
                Stake = 10000,
                Formula = "doubles",
                Type = RewardType.Uniboost,
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
                Terms = RewardBuilder.UniboostTerms
                    .WithMaxStake(maxStake: 10001)
            };

            yield return new()
            {
                Stake = 10000,
                Formula = "doubles",
                Type = RewardType.Uniboost,
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
                Terms = RewardBuilder.UniboostTerms
                    .WithMaxStake(maxStake: 10001)
                    .WithMaxExtraWinnings(maxExtraWinnings: 5000)
            };

            yield return new()
            {
                Stake = 15,
                Formula = "doubles",
                Type = RewardType.Uniboost,
                BetOutcome = BetOutcome.Winning,
                Stages = new List<CompoundStage>
                {
                    new()
                    {
                        ContestType = TestConstants.DefaultContestType,
                        Market = TestConstants.DefaultMarketRn,
                        OriginalOdds = 23m,
                        Selection = new() { Outcome = "4-342-34-2-342" }
                    }
                },
                Terms = RewardBuilder.UniboostTerms
                    .WithMaxStake(maxStake: 10)
            };
        }
    }

    private string _existingClaimInstanceId;
    private UniBoostClaimStrategy _atTest;

    [OneTimeSetUp]
    public void SetUp()
    {
        var context = new RewardsDbContextMock(new DbContextOptionsBuilder<RewardsDbContextMock>()
            .UseInMemoryDatabase(databaseName: "RewardsDbContext")
            .Options);

        _existingClaimInstanceId = Guid.NewGuid().ToString();

        var claim = Mapper.Map<RewardClaim>(RewardClaimBuilder.CreateBonusClaim(RewardType.Uniboost, model =>
        {
            model.InstanceId = _existingClaimInstanceId;
        }));

        context.RewardClaims.Add(claim);
        context.SaveChanges();

        _atTest = new(context,
            Mapper, new Mock<ILogger<UniBoostClaimStrategy>>().Object, new Mock<IOddsLadderClient>().Object,
            new Mock<IMarketMirrorClient>().Object);
    }

    [Test]
    public async Task ProcessClaimAsync_WhenCombinationHasOneStage_ReturnsBoosted_ForOneStage()
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
                        { RewardParameterKey.MaxStakeAmount, "5" },
                    },
                },
                InstanceId = _existingClaimInstanceId,
                BetRn = betRn,
                CouponRn = Guid.NewGuid().ToString(),
                CustomerId = customerId,
                RewardId = Guid.NewGuid().ToString(),
                PromotionName = "Test",
                PayoffMetadata = new()
                {
                    Odds = new()
                    {
                        new()
                        {
                            Boosted = 1.75M,
                            Original = 1.5M,
                            Outcome = "ksp:outcome.1[football:202110042300:clube_do_remo_pa_vs_coritiba_fc_pr]:1x2:plain:clube_do_remo_pa"
                        }
                    }
                }
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
            BetStake = 5,
            CombinationStake = 5
        });

        // Assert
        result.Payoff.Should().Be(1.25M);
    }


    [Test]
    [TestCaseSource(typeof(UniBoostClaimStrategyTests), nameof(BoostedOddsResultFactory))]
    public async Task<dynamic> CalculateShouldReturnCorrectBoostedOdds(RewardBet bet, string oddsLadderOverride = null)
    {
        // Arrange
        var scope = new TestScope(oddsLadderOverride);

        // Act
        await scope.RewardStrategy.ProcessClaimAsync(bet);

        // Assert
        return new { bet.RewardPaymentAmount, bet.BoostedOdds };
    }

    [Test]
    public async Task ProcessClaimAsync_RewardPaymentAmountShouldBeNull_WhenCalledForLosingBet()
    {
        // Arrange
        var scope = new TestScope(null);
        var bet = new RewardBet { BetOutcome = BetOutcome.Losing };

        // Act
        await scope.RewardStrategy.ProcessClaimAsync(bet);

        // Assert
        bet.RewardPaymentAmount.Should().BeNull();
    }

    [Test]
    [TestCaseSource(nameof(ExampleRewardBets))]
    public async Task GetOddsLadder_IfMarketMirrorClientReturnsNothing_UsePreGameOddsLadder(RewardBet bet)
    {
        // Arrange
        var fixture = new Fixture();

        var marketMirrorClient = fixture.Freeze<Mock<IMarketMirrorClient>>();
        marketMirrorClient
            .Setup(c => c.GetContests(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(() => null);

        var scope = new TestScope(marketMirrorClient: marketMirrorClient.Object);

        // Act
        var expectedOddsLadder = OddsLadder().PreGameOddsLadder;
        var actualOddsLadder = await scope.RewardStrategy.GetOddsLadder(bet);

        // Assert
        Assert.AreEqual(expectedOddsLadder.Count, actualOddsLadder.Count);
    }

    [Test]
    [TestCaseSource(nameof(ExampleRewardBets))]
    public async Task GetOddsLadder_IfMarketMirrorClientReturnsPreGame_UsePreGameOddsLadder(RewardBet bet)
    {
        // Arrange
        var fixture = new Fixture();

        var marketMirrorClient = fixture.Freeze<Mock<IMarketMirrorClient>>();
        marketMirrorClient
            .Setup(c => c.GetContests(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new GetContestsResponse
            {
                Contests = new[] { new ContestDetails { ContestStatus = ContestStatus.PreGame } }
            });

        var scope = new TestScope(marketMirrorClient: marketMirrorClient.Object);

        // Act
        var expectedOddsLadder = OddsLadder().PreGameOddsLadder;
        var actualOddsLadder = await scope.RewardStrategy.GetOddsLadder(bet);

        // Assert
        Assert.AreEqual(expectedOddsLadder.Count, actualOddsLadder.Count);
    }

    [Test]
    [TestCaseSource(nameof(ExampleRewardBets))]
    public async Task GetOddsLadder_IfMarketMirrorClientReturnsInPlay_UseInPlayOddsLadder(RewardBet bet)
    {
        // Arrange
        var fixture = new Fixture();

        var marketMirrorClient = fixture.Freeze<Mock<IMarketMirrorClient>>();
        marketMirrorClient
            .Setup(c => c.GetContests(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new GetContestsResponse
            {
                Contests = new[] { new ContestDetails { ContestStatus = ContestStatus.InPlay } }
            });

        var scope = new TestScope(marketMirrorClient: marketMirrorClient.Object);

        // Act
        var expectedOddsLadder = OddsLadder().InPlayOddsLadder;
        var actualOddsLadder = await scope.RewardStrategy.GetOddsLadder(bet);

        // Assert
        Assert.AreEqual(expectedOddsLadder.Count, actualOddsLadder.Count);
    }


    [Test]
    [TestCase("ksp:market.1:[football:202305051700:fc_torpedo_belaz_zhodino_vs_fc_slutsk]:fc_torpedo_belaz_zhodino_clean_sheet:base",
        "football:202305051700:fc_torpedo_belaz_zhodino_vs_fc_slutsk")]
    [TestCase("ksp:market.1:[football:202304081830:1_fc_heidenheim_vs_fc_st_pauli]:both_teams_to_score:base",
        "football:202304081830:1_fc_heidenheim_vs_fc_st_pauli")]
    public async Task GetOddsLadder_ShouldPassContestKey_ToMarketMirrorClient(string market, string expectedContestKey)
    {
        // Arrange
        var fixture = new Fixture();
        var marketMirrorClient = fixture.Freeze<Mock<IMarketMirrorClient>>();
        marketMirrorClient
            .Setup(c => c.GetContests(It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(() => null);
        var scope = new TestScope(marketMirrorClient: marketMirrorClient.Object);
        var rewardBet = ExampleRewardBets.First();

        // Act
        rewardBet.Stages.First().Market = market;
        await scope.RewardStrategy.GetOddsLadder(rewardBet);


        // Assert
        marketMirrorClient.Verify(c => c.GetContests(new[] { expectedContestKey }), Times.Once);
    }

    private class TestScope
    {
        public TestScope(string oddsLadderOverride = null)
        {
            var fixture = new Fixture();
            var logger = fixture.Freeze<Mock<ILogger<UniBoostClaimStrategy>>>();
            var context = new RewardsDbContextMock(new DbContextOptionsBuilder<RewardsDbContextMock>()
                .UseInMemoryDatabase(databaseName: "RewardsDbContext")
                .Options);
            var mapper = new Mock<IMapper>();
            var marketMirrorClient = fixture.Freeze<Mock<IMarketMirrorClient>>();
            var oddsLadderLogic = new Mock<IOddsLadderClient>();

            oddsLadderLogic
                .Setup(c => c.GetOddsLadder(It.IsAny<string>()))
                .ReturnsAsync(OddsLadder(oddsLadderOverride));

            marketMirrorClient
                .Setup(c => c.GetContests(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(() => null);

            TestReward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Uniboost, null);
            RewardStrategy = new(context, mapper.Object, logger.Object, oddsLadderLogic.Object, marketMirrorClient.Object);
        }

        public TestScope(ILogger<UniBoostClaimStrategy> logger = default, RewardsDbContext contextFactory = default, IMapper mapper = default,
            IMarketMirrorClient marketMirrorClient = default, IOddsLadderClient oddsLadderLogic = default)
        {
            var fixture = new Fixture();

            logger ??= fixture.Freeze<Mock<ILogger<UniBoostClaimStrategy>>>().Object;

            contextFactory ??= new RewardsDbContextMock(new DbContextOptionsBuilder<RewardsDbContextMock>()
                .UseInMemoryDatabase(databaseName: "RewardsDbContext")
                .Options); ;

            mapper ??= new Mock<IMapper>().Object;

            if (marketMirrorClient is null)
            {
                var marketMirrorClientMock = fixture.Freeze<Mock<IMarketMirrorClient>>();
                marketMirrorClientMock
                    .Setup(c => c.GetContests(It.IsAny<IEnumerable<string>>()))
                    .ReturnsAsync(new GetContestsResponse());

                marketMirrorClient = marketMirrorClientMock.Object;
            }

            if (oddsLadderLogic is null)
            {
                var oddsLadderLogicMock = new Mock<IOddsLadderClient>();
                oddsLadderLogicMock
                    .Setup(c => c.GetOddsLadder(It.IsAny<string>()))
                    .ReturnsAsync(OddsLadder());

                oddsLadderLogic = oddsLadderLogicMock.Object;
            }

            TestReward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Uniboost, null);
            RewardStrategy = new(contextFactory, mapper, logger, oddsLadderLogic, marketMirrorClient);
        }

        public UniBoostClaimStrategy RewardStrategy { get; }

        public RewardDomainModel TestReward { get; }
    }

}
