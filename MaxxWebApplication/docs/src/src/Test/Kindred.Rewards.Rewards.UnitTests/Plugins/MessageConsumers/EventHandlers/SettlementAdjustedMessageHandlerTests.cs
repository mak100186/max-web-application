//using AutoFixture;

//using Kindred.Customer.WalletGateway.ExternalModels.Common.Enums;
//using Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels;
//using Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.Enums;
//using Kindred.Rewards.Core.Extensions;
//using Kindred.Rewards.Core.Models.Events;
//using Kindred.Rewards.Plugin.MessageConsumers.EventHandlers;
//using Kindred.Rewards.Rewards.Tests.Common;
//using Kindred.Rewards.Rewards.UnitTests.TestUtils;

//using Microsoft.Extensions.Logging;

//using IClaimService = Kindred.Rewards.Plugin.MessageConsumers.Services.IClaimService;

//using Moq;

//using NUnit.Framework;

//namespace Kindred.Rewards.Rewards.UnitTests.EventHandlers.Kafka;

//[TestFixture]
//[Category("Unit")]
//public class SettlementAdjustedMessageHandlerTests : BaseMapperTests<SettlementAdjustedMessageHandler>
//{
//    [SetUp]
//    public void SetUp()
//    {
//        this.rewardServiceMock = new();

//        this.subject = new(this.LoggerMock.Object, this.Mapper, this.rewardServiceMock.Object);
//    }

//    [TearDown]
//    public void TearDown()
//    {
//        this.LoggerMock.Reset();
//    }

//    private Mock<IClaimService> rewardServiceMock;
//    private SettlementAdjustedMessageHandler subject;

//    [TestCase(BetStatus.Settled, SettlementSegmentStatus.Lost)]
//    [TestCase(BetStatus.Settled, SettlementSegmentStatus.Won)]
//    [TestCase(BetStatus.Settled, SettlementSegmentStatus.PartRefunded)]
//    public async Task SettlementAdjustedHandler_ShouldSettleClaimForEachClaimInBet_WhenActionIsDeterminedToBeSettle(
//        BetStatus betStatus, SettlementSegmentStatus settlementSegmentStatus)
//    {
//        //arrange
//        var bet = this.Fixture.Build<Bet>()
//            .With(x => x.Status, betStatus)
//            .Create();
//        var action = this.Fixture.Create<SettlementAction>();

//        bet.AcceptedCombinations.ForAll(c => c.Settlement.Status = SettlementCombinationStatus.Resolved);
//        bet.AcceptedCombinations.ForAll(c =>
//            c.Settlement.Segments.ForAll(x => x.Status = settlementSegmentStatus));

//        //act
//        await this.subject.Handle(bet, action, TestConstants.CorrelationId);

//        //assert
//        this.LoggerMock.VerifyLog(LogLevel.Information, "Received SettlementAdjustedMessage", 1);

//        this.rewardServiceMock.Verify(x =>
//                x.SettleClaimAsync(
//                    It.Is<RewardBet>(b => b.Status.Value.ToString() == bet.Status.ToString()),
//                    It.Is<string>(cid => cid == TestConstants.CorrelationId)),
//            Times.Exactly(bet.RewardClaims.Count()));
//    }

//    [TestCase(BetStatus.Settled)]
//    [TestCase(BetStatus.Cancelled)]
//    public async Task SettlementAdjustedHandler_ShouldSettleClaimForEachClaimInBet_WhenActionIsDeterminedToBeReactivate(
//        BetStatus betStatus)
//    {
//        var settlementSegmentStatus = SettlementSegmentStatus.Refunded;

//        //arrange
//        var bet = this.Fixture.Build<Bet>()
//            .With(x => x.Status, betStatus)
//            .Create();
//        var action = this.Fixture.Create<SettlementAction>();

//        bet.AcceptedCombinations.ForAll(c => c.Settlement.Status = SettlementCombinationStatus.Resolved);
//        bet.AcceptedCombinations.ForAll(c =>
//            c.Settlement.Segments.ForAll(x => x.Status = settlementSegmentStatus));

//        //act
//        await this.subject.Handle(bet, action, TestConstants.CorrelationId);

//        //assert
//        this.LoggerMock.VerifyLog(LogLevel.Information, "Received SettlementAdjustedMessage", 1);

//        this.rewardServiceMock.Verify(x =>
//                x.ReActivateClaimAsync(It.IsAny<string>(), It.Is<string>(cid => cid == TestConstants.CorrelationId)),
//            Times.Exactly(bet.RewardClaims.Count()));
//    }

//    [Test]
//    public async Task SettlementAdjustedHandler_ShouldLogWarningClaimInBet_WhenActionIsNotDetermined()
//    {
//        //arrange
//        var bet = this.Fixture.Build<Bet>()
//            .With(x => x.Status, BetStatus.Pending)
//            .Create();
//        var action = this.Fixture.Create<SettlementAction>();

//        //act
//        await this.subject.Handle(bet, action, TestConstants.CorrelationId);

//        //assert
//        this.LoggerMock.VerifyLog(LogLevel.Debug, "Reward action is Undetermined", bet.RewardClaims.Count());

//        this.rewardServiceMock.Verify(x =>
//                x.ReActivateClaimAsync(It.IsAny<string>(), It.IsAny<string>()),
//            Times.Never);
//        this.rewardServiceMock.Verify(x =>
//                x.SettleClaimAsync(It.IsAny<RewardBet>(), It.IsAny<string>()),
//            Times.Never);
//    }

//    [Test]
//    public async Task SettlementAdjustedHandler_ThrowsLogsAndContinues_WhenArgumentNullExceptionOccurs()
//    {
//        var settlementSegmentStatus = SettlementSegmentStatus.Refunded;

//        //arrange
//        var bet = this.Fixture.Build<Bet>()
//            .With(x => x.Status, BetStatus.Settled)
//            .Create();
//        var action = this.Fixture.Create<SettlementAction>();

//        bet.AcceptedCombinations.ForAll(c => c.Settlement.Status = SettlementCombinationStatus.Resolved);
//        bet.AcceptedCombinations.ForAll(c =>
//            c.Settlement.Segments.ForAll(x => x.Status = settlementSegmentStatus));

//        this.rewardServiceMock.Setup(x => x.ReActivateClaimAsync(It.IsAny<string>(), It.IsAny<string>()))
//            .ThrowsAsync(new ArgumentNullException());

//        //act
//        await this.subject.Handle(bet, action, TestConstants.CorrelationId);

//        //assert
//        this.LoggerMock.VerifyLog(LogLevel.Warning, "Reward could not be processed on SettlementAdjustedMessage", 3);
//    }

//    [Test]
//    public void SettlementAdjustedHandler_ThrowsAndBreaks_WhenExceptionOccurs()
//    {
//        var settlementSegmentStatus = SettlementSegmentStatus.Refunded;

//        //arrange
//        var bet = this.Fixture.Build<Bet>()
//            .With(x => x.Status, BetStatus.Settled)
//            .Create();
//        var action = this.Fixture.Create<SettlementAction>();

//        bet.AcceptedCombinations.ForAll(c => c.Settlement.Status = SettlementCombinationStatus.Resolved);
//        bet.AcceptedCombinations.ForAll(c =>
//            c.Settlement.Segments.ForAll(x => x.Status = settlementSegmentStatus));

//        this.rewardServiceMock.Setup(x => x.ReActivateClaimAsync(It.IsAny<string>(), It.IsAny<string>()))
//            .ThrowsAsync(new());

//        //act
//        Assert.ThrowsAsync<Exception>(async () => { await this.subject.Handle(bet, action, TestConstants.CorrelationId); });

//        //assert
//        this.LoggerMock.VerifyLog(LogLevel.Error, "Reward could not be processed on SettlementAdjustedMessage", 1);
//    }
//}
