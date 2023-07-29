using AutoFixture;

using AutoMapper;

using FluentAssertions;

using Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Mapping;
using Kindred.Rewards.Core.Mapping.Converters;
using Kindred.Rewards.Core.Models.Events;
using Kindred.Rewards.Core.Models.Rn;
using Kindred.Rewards.Rewards.UnitTests.Common;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using BetStatus = Kindred.Customer.WalletGateway.ExternalModels.Common.Enums.BetStatus;
using MinotaurSettlementCombinationStatus = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.Enums.SettlementCombinationStatus;
using MinotaurSettlementSegmentStatus = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.Enums.SettlementSegmentStatus;

namespace Kindred.Rewards.Rewards.UnitTests.Plugins.MessageConsumers.Mappings;

[TestFixture]
[Category("Unit")]
public class EventToDomainMappingTests : TestBase
{
    [OneTimeSetUp]
    public void SetupMapping()
    {
        var svcColl = new ServiceCollection();
        svcColl.AddAutoMapper(typeof(RewardEventToDomainMappings));

        svcColl.AddSingleton<IConverter<string, RewardRn>, RewardRnConverter>();
        svcColl.AddLogging();

        var serviceProvider = svcColl.BuildServiceProvider();

        Mapper = serviceProvider.GetService<IMapper>();
    }

    [TestCase(MinotaurSettlementCombinationStatus.Resolved, MinotaurSettlementSegmentStatus.Won, BetOutcome.Winning)]
    [TestCase(MinotaurSettlementCombinationStatus.Resolved, MinotaurSettlementSegmentStatus.Refunded, BetOutcome.FullRefund)]
    [TestCase(MinotaurSettlementCombinationStatus.Resolved, MinotaurSettlementSegmentStatus.PartWon, BetOutcome.WinningAndPartialRefund)]
    [TestCase(MinotaurSettlementCombinationStatus.Resolved, MinotaurSettlementSegmentStatus.PartRefunded, BetOutcome.WinningAndPartialRefund)]
    [TestCase(MinotaurSettlementCombinationStatus.Resolved, MinotaurSettlementSegmentStatus.Lost, BetOutcome.Losing)]
    [TestCase(MinotaurSettlementCombinationStatus.Unresolved, MinotaurSettlementSegmentStatus.Pending, null)]
    public void MinotaurBet_MappedToRewardBet_ShouldMapToExpectedBetOutcomeInRewardBet(MinotaurSettlementCombinationStatus settlementCombinationStatus, MinotaurSettlementSegmentStatus settlementSegmentStatus, BetOutcome? expectedBetOutcome)
    {
        // arrange
        var bet = Fixture.Create<Bet>();
        bet.AcceptedCombinations.ForAll(x => x.Settlement.Status = settlementCombinationStatus);
        bet.AcceptedCombinations.ForAll(x => x.Settlement.Segments.ForAll(y => y.Status = settlementSegmentStatus));

        // act
        var mapped = Mapper.Map<RewardBet>(bet);

        // assert
        mapped.BetOutcome.Should().Be(expectedBetOutcome);
    }

    [Test]
    public void MinotaurBet_MappedToRewardBet_ShouldMapToExpectedStakeAmountInRewardBet()
    {
        // arrange
        var bet = Fixture.Create<Bet>();
        bet.Stake = 20.0f;

        // act
        var mapped = Mapper.Map<RewardBet>(bet);

        // assert
        mapped.Stake.Should().Be(20.0m);
    }


    [TestCase(BetStatus.Cancelled)]
    [TestCase(BetStatus.Settled)]
    public void MinotaurBet_MappedToRewardBet_ShouldMapToExpectedStatusInRewardBet(BetStatus betStatus)
    {
        // arrange
        var stringEnum = betStatus.ToString();
        Enum.TryParse(stringEnum, true, out Kindred.Rewards.Core.Enums.BetStatus expectedBetStatus);

        var bet = Fixture.Create<Bet>();
        bet.Status = betStatus;

        // act
        var mapped = Mapper.Map<RewardBet>(bet);

        // assert
        mapped.Status.Should().Be(expectedBetStatus);
    }

    [TestCase(BetStatus.Pending)]
    [TestCase(BetStatus.Accepted)]
    [TestCase(BetStatus.Archived)]
    [TestCase(BetStatus.CounterOffered)]
    [TestCase(BetStatus.Intercepted)]
    [TestCase(BetStatus.Rejected)]
    [TestCase(BetStatus.Suggested)]
    [TestCase(BetStatus.Discarded)]
    public void MinotaurBet_MappedToRewardBet_ShouldMapToNullStatusInRewardBet(BetStatus betStatus)
    {
        // arrange
        var bet = Fixture.Create<Bet>();
        bet.Status = betStatus;

        // act
        var mapped = Mapper.Map<RewardBet>(bet);

        // assert
        mapped.Status.Should().BeNull();
    }
}
