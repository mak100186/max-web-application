using FluentAssertions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Plugin.Claim.Models.Dto;
using Kindred.Rewards.Plugin.Claim.Models.Requests;
using Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim;
using Kindred.Rewards.Plugin.Claim.Models.Responses;
using Kindred.Rewards.Plugin.Claim.Models.Responses.SettleClaim;
using Kindred.Rewards.Plugin.Claim.Services;
using Kindred.Rewards.Plugin.Claim.Services.Strategies;
using Kindred.Rewards.Plugin.Claim.Services.Strategies.Base;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.UnitTests.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using SettlementCombinationStatus = Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim.Enums.SettlementCombinationStatus;
using SettlementSegmentStatus = Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim.Enums.SettlementSegmentStatus;

namespace Kindred.Rewards.Rewards.UnitTests.Plugins.Claim.Services;

internal class ClaimSettleServiceTests : TestBase
{
    private RewardsDbContextMock _context;
    private Mock<IRewardClaimStrategyFactory> _strategyFactory;
    private ClaimSettleService _service;

    private string _claimId;
    private string _customerId;
    private string _rewardId;

    [SetUp]
    public void SetUp()
    {
        _context = new(new DbContextOptionsBuilder<RewardsDbContextMock>()
            .UseInMemoryDatabase(databaseName: "RewardsDbContext")
            .Options);

        _strategyFactory = new();

        _service = new(_context,
            Mapper, new Mock<ILogger<ClaimSettleService>>().Object, _strategyFactory.Object);

        _claimId = Guid.NewGuid().ToString();
        _customerId = ObjectBuilder.CreateString();
        _rewardId = ObjectBuilder.CreateString();
    }

    [Test]
    [TestCase(RewardType.Uniboost, 10, 0)]
    [TestCase(RewardType.Freebet, 4, 0)]
    [TestCase(RewardType.Profitboost, 3, 0)]
    public async Task SettleClaimAsync_GivenRequestWithClaimThatExists_AndNoPreviousPayoffs_CalculatesPayoffAndReturnsIt(RewardType rewardType,
        decimal expectedCurrPayoff, decimal expectedPrevPayoff)
    {
        // Arrange
        var strategy = new Mock<IRewardStrategy>();
        strategy.Setup(x => x.ProcessClaimAsync(It.IsAny<SettleClaimParameterDto>()))
            .ReturnsAsync(new SettleClaimResultDto { Payoff = expectedCurrPayoff });

        _strategyFactory.Setup(x => x.GetRewardStrategy(It.Is<RewardType>(x => x == rewardType)))
            .Returns(strategy.Object);

        var rewardClaimRepo = _context.RewardClaims;
        rewardClaimRepo.Add(GenerateClaim(rewardType, new()));

        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SettleClaimAsync(GenerateRequest());

        // Assert
        result.Should().BeEquivalentTo(new SettleClaimResponse
        {
            PrevRewardClaimSettlement = new() { new() { Payoff = expectedPrevPayoff, Reward = null } },
            RewardClaimSettlement = new() { new() { Payoff = expectedCurrPayoff, Reward = null } }
        });
    }

    [Test]
    [TestCase(RewardType.Uniboost, 10, 1314)]
    [TestCase(RewardType.Freebet, 4, 62)]
    [TestCase(RewardType.Profitboost, 3, 21)]
    public async Task SettleClaimAsync_GivenRequestWithClaimThatExists_AndHasPreviousPayoffs_CalculatesPayoffAndReturnsItAndThePreviousPayoff(RewardType rewardType,
        decimal expectedCurrPayoff, decimal expectedPrevPayoff)
    {
        // Arrange
        var strategy = new Mock<IRewardStrategy>();
        strategy.Setup(x => x.ProcessClaimAsync(It.IsAny<SettleClaimParameterDto>()))
            .ReturnsAsync(new SettleClaimResultDto { Payoff = expectedCurrPayoff });

        _strategyFactory.Setup(x => x.GetRewardStrategy(It.Is<RewardType>(x => x == rewardType)))
            .Returns(strategy.Object);

        var rewardClaimRepo = _context.RewardClaims;
        rewardClaimRepo.Add(GenerateClaim(rewardType, new()
        {
            new()
            {
                CombinationPayoff = expectedPrevPayoff,
                BetRn = ObjectBuilder.CreateString(),
                CombinationRn = ObjectBuilder.CreateString()
            }
        }));

        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SettleClaimAsync(GenerateRequest());

        // Assert
        result.Should().BeEquivalentTo(new SettleClaimResponse
        {
            PrevRewardClaimSettlement = new() { new() { Payoff = expectedPrevPayoff, Reward = null } },
            RewardClaimSettlement = new() { new() { Payoff = expectedCurrPayoff, Reward = null } }
        });
    }

    private SettleClaimRequest GenerateRequest()
    {
        return new()
        {
            Settlement = new() { FinalPayoff = 1 },
            AcceptedCombinations = new List<CombinationPayload>
            {
                new()
                {
                    Rn = ObjectBuilder.CreateString(),
                    Settlement = new()
                    {
                        Segments = new List<SegmentPayload>
                        {
                            new() { Dividend = 1, Status = SettlementSegmentStatus.Won },
                        },
                        Status = SettlementCombinationStatus.Resolved,
                        Payoff = 13,
                    },
                    Selections = new List<SelectionPayload> { new() { Outcome = "testOutcome" } }
                },
            },
            CustomerId = _customerId,
            RewardClaims = new List<RewardClaimPayload> { new() { ClaimRn = _claimId, RewardRn = _rewardId } },
            Stages = new List<CompoundStagePayload>
            {
                new()
                {
                    Market = ObjectBuilder.CreateString(),
                    Odds = new() { Price = 10 },
                    Selection = new() { Outcome = "testOutcome" }
                }
            }
        };
    }

    private RewardClaim GenerateClaim(RewardType rewardType, List<CombinationRewardPayoffDomainModel> payoffs)
    {
        var claim = RewardClaimBuilder.CreateBonusClaim(rewardType, model =>
        {
            model.InstanceId = _claimId;
            model.Status = RewardClaimStatus.Claimed;
            model.PayoffMetadata = new()
            {
                Odds = new()
                {
                    new() { Boosted = 10, Original = 1, Outcome = "someOutcome" }
                }
            };
            model.RewardId = _rewardId;
            model.CustomerId = _customerId;
            model.CombinationRewardPayoffs = payoffs;
        });

        return Mapper.Map<RewardClaim>(claim);
    }
}
