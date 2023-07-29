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

//using Moq;

//using NUnit.Framework;
//using IClaimService = Kindred.Rewards.Plugin.MessageConsumers.Services.IClaimService;

//namespace Kindred.Rewards.Rewards.UnitTests.EventHandlers.Kafka;

//[TestFixture]
//[Category("Unit")]
//public class ResultsReversedMessageHandlerTests : BaseMapperTests<ResultsReversedMessageHandler>
//{
//    private Mock<IClaimService> rewardServiceMock;
//    private ResultsReversedMessageHandler subject;

//    [SetUp]
//    public void SetUp()
//    {
//        this.rewardServiceMock = new();

//        this.subject =
//            new(this.LoggerMock.Object, this.Mapper, this.rewardServiceMock.Object);
//    }

//    [TearDown]
//    public void TearDown()
//    {
//        this.LoggerMock.Reset();
//    }

//    [Test]
//    public async Task CombinationSettledHandler_UnsettlesClaim_WhenCalled()
//    {
//        //arrange
//        var bet = this.Fixture.Build<Bet>()
//            .With(x => x.Status, BetStatus.Settled)
//            .Create();
//        var action = this.Fixture.Create<SettlementAction>();

//        bet.AcceptedCombinations.ForAll(c => c.Settlement.Status = SettlementCombinationStatus.Resolved);
//        bet.AcceptedCombinations.ForAll(c =>
//            c.Settlement.Segments.ForAll(x => x.Status = SettlementSegmentStatus.Won));

//        //act
//        await this.subject.Handle(bet, action, TestConstants.CorrelationId);

//        //assert
//        this.LoggerMock.VerifyLog(LogLevel.Information, "Received ResultsReversedMessage", 1);

//        this.rewardServiceMock.Verify(x =>
//            x.UnsettleClaimAsync(It.IsAny<RewardBet>(), It.Is<string>(x => x == TestConstants.CorrelationId)),
//            Times.Exactly(bet.RewardClaims.Count()));
//    }

//    [Test]
//    public async Task CombinationSettledHandler_ThrowsLogsAndContinues_WhenArgumentNullExceptionOccurs()
//    {
//        //arrange
//        var bet = this.Fixture.Build<Bet>()
//            .With(x => x.Status, BetStatus.Settled)
//            .Create();
//        var action = this.Fixture.Create<SettlementAction>();

//        bet.AcceptedCombinations.ForAll(c => c.Settlement.Status = SettlementCombinationStatus.Resolved);
//        bet.AcceptedCombinations.ForAll(c =>
//            c.Settlement.Segments.ForAll(x => x.Status = SettlementSegmentStatus.Refunded));

//        this.rewardServiceMock.Setup(x => x.UnsettleClaimAsync(It.IsAny<RewardBet>(), It.IsAny<string>()))
//            .ThrowsAsync(new ArgumentNullException());

//        //act
//        await this.subject.Handle(bet, action, TestConstants.CorrelationId);

//        //assert
//        this.LoggerMock.VerifyLog(LogLevel.Error, "Reward could not be processed on ResultsReversedMessage", 3);
//    }

//    [Test]
//    public void CombinationSettledHandler_ThrowsAndBreaks_WhenExceptionOccurs()
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

//        this.rewardServiceMock.Setup(x => x.UnsettleClaimAsync(It.IsAny<RewardBet>(), It.IsAny<string>()))
//            .ThrowsAsync(new());

//        //act
//        Assert.ThrowsAsync<Exception>(async () =>
//        {
//            await this.subject.Handle(bet, action, TestConstants.CorrelationId);
//        });

//        //assert
//        this.LoggerMock.VerifyLog(LogLevel.Error, "Reward could not be processed on ResultsReversedMessage", 1);
//    }
//}
