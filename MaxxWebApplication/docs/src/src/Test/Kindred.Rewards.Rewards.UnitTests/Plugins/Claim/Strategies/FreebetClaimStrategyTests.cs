using System.Collections;

using AutoFixture;
using AutoFixture.AutoMoq;

using AutoMapper;

using FluentAssertions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Events;
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

internal class FreebetClaimStrategyTests : TestBase
{
    private string _claimInstanceId;
    private FreeBetClaimStrategy _atTest;

    public static IEnumerable RewardPayoutResultFactory
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
                                Selection = new() { Outcome = "4-342-34-2-342" }
                            }
                        },
                        Terms = RewardBuilder.FreebetTerms
                    })
                .SetName("Reward payout with no configured minimum odds.")
                .Returns(7.5m);

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
                        Terms = RewardBuilder.FreebetTerms.WithMinimumOdds(1.6m)
                    })
                .SetName("RewardPayout with one stage and it falls below the configured minimum odds.")
                .Returns(null);


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
                        Terms = RewardBuilder.FreebetTerms.WithMinimumOdds(1.0m)
                    })
                .SetName("RewardPayout with one stage does not apply reward because bet did not win.")
                .Returns(null);

            yield return new TestCaseData(
                    new RewardBet
                    {
                        Stake = 6,
                        BetOutcome = BetOutcome.Winning,
                        Formula = "doubles",
                        Stages = new List<CompoundStage>
                        {
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 10m,
                                Selection = new() { Outcome = "4-342-34-2-342" }
                            },
                            new()
                            {
                                ContestType = TestConstants.DefaultContestType,
                                Market = TestConstants.DefaultMarketRn,
                                OriginalOdds = 10m,
                                Selection = new() { Outcome = "4-342-34-2-342" }
                            }
                        },
                        Terms = RewardBuilder.FreebetTerms
                            .WithMaxExtraWinnings(2)
                    })
                .SetName("RewardPayout with 2 stages and has max extra winnings configured.")
                .Returns(2);
        }
    }


    [SetUp]
    public void SetUp()
    {
        var context = new RewardsDbContextMock(new DbContextOptionsBuilder<RewardsDbContextMock>()
            .UseInMemoryDatabase(databaseName: "RewardsDbContext")
            .Options);

        _claimInstanceId = Guid.NewGuid().ToString();
        var claim = Mapper.Map<RewardClaim>(RewardClaimBuilder.CreateBonusClaim(RewardType.Freebet, model =>
        {
            model.InstanceId = _claimInstanceId;
        }));

        context.RewardClaims.Add(claim);
        context.SaveChanges();

        _atTest = new(context, Mapper, new Mock<ILogger<FreeBetClaimStrategy>>().Object);
    }

    [Test]
    public async Task CalculateRewardPaymentAmountAsync_WhenBetPayoffIsZero_ReturnsZero()
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
            CustomerId = customerId,
            Formula = "Singles",
            RewardClaim = new()
            {
                Id = 1,
                Terms = new()
                {
                    RewardParameters = new Dictionary<string, string>
                    {
                        { RewardParameterKey.Amount, "10" }
                    }
                },
                InstanceId = _claimInstanceId,
                BetRn = betRn,
                CouponRn = Guid.NewGuid().ToString(),
                CustomerId = customerId,
                RewardId = Guid.NewGuid().ToString(),
                PromotionName = "Test"
            },
            BetStake = 16,
            CombinationStake = 5
        });

        // Assert
        result.Payoff.Should().Be(0);
    }

    [Test]
    public async Task CalculateRewardPaymentAmountAsync_WhenBetPayoffIsGreaterThanFreeBetAmount_ReturnsNegativeStake()
    {
        // Arrange
        var betRn = Guid.NewGuid().ToString();
        var customerId = "123456";

        // Act
        var result = await _atTest.ProcessClaimAsync(new SettleClaimParameterDto
        {
            BetPayoff = 50,
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
                        { RewardParameterKey.Amount, "10" }
                    }
                },
                InstanceId = _claimInstanceId,
                BetRn = betRn,
                CouponRn = Guid.NewGuid().ToString(),
                CustomerId = customerId,
                RewardId = Guid.NewGuid().ToString(),
                PromotionName = "Test"
            },
            BetStake = 16,
            CombinationStake = 5
        });

        // Assert
        result.Payoff.Should().Be(-5);
    }

    [Test]
    public async Task CalculateRewardPaymentAmountAsync_WhenBetPayoffIsLessThanFreeBetAmount_ReturnsNegativeBetPayoff()
    {
        // Arrange
        var betRn = Guid.NewGuid().ToString();
        var customerId = "123456";

        // Act
        var result = await _atTest.ProcessClaimAsync(new SettleClaimParameterDto
        {
            BetPayoff = 5,
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
                        { RewardParameterKey.Amount, "10" }
                    }
                },
                InstanceId = _claimInstanceId,
                BetRn = betRn,
                CouponRn = Guid.NewGuid().ToString(),
                CustomerId = customerId,
                RewardId = Guid.NewGuid().ToString(),
                PromotionName = "Test"
            },
            BetStake = 16,
            CombinationStake = 5
        });

        // Assert
        result.Payoff.Should().Be(-5);
    }

    [Test]
    [TestCaseSource(typeof(FreebetClaimStrategyTests), nameof(RewardPayoutResultFactory))]
    public async Task<dynamic> CalculateRewardPaymentAmountAsync_ShouldReturnCorrectRewardPaymentAmount(RewardBet bet)
    {
        // Arrange
        var scope = new TestScope();

        // Act
        await scope.Strategy.ProcessClaimAsync(bet);

        // Assert
        return bet.RewardPaymentAmount;
    }

    [Test]
    public async Task CalculateShouldReturnZero()
    {
        // Arrange
        var scope = new TestScope();
        var bet = new RewardBet { RewardPaymentAmount = 10 };

        // Act
        await scope.Strategy.ProcessClaimAsync(bet);

        // Assert
        bet.RewardPaymentAmount.Should().BeNull();
    }
    [Test]
    public async Task CalculateShouldAllowMultiBet()
    {
        // Arrange
        var scope = new TestScope();
        var bet = scope.Fixture.Create<RewardBet>();

        // Act
        await scope.Strategy.ProcessClaimAsync(bet);

        // Assert
        bet.RewardPaymentAmount.Should().BeNull();
    }


    private class TestScope
    {
        public TestScope()
        {
            Fixture = new Fixture();
            Fixture.Customize(new AutoMoqCustomization());
            var logger = Fixture.Freeze<Mock<ILogger<FreeBetClaimStrategy>>>();
            var context = new RewardsDbContextMock(new DbContextOptionsBuilder<RewardsDbContextMock>()
                .UseInMemoryDatabase(databaseName: "RewardsDbContext")
                .Options);
            var mapper = new Mock<IMapper>();


            TestReward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, null);

            Strategy = new FreeBetClaimStrategy(context, mapper.Object, logger.Object);
        }

        public RewardClaimStrategyBase Strategy { get; }

        public RewardDomainModel TestReward { get; }

        public IFixture Fixture { get; }
    }
}
