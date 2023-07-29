//using System.Text;

//using AutoFixture;

//using Couchbase.Core.Exceptions;

//using KafkaFlow;

//using Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels;
//using Kindred.Rewards.Plugin.MessageConsumers.EventHandlers;
//using Kindred.Rewards.Plugin.MessageConsumers.Validators;
//using Kindred.Rewards.Rewards.Tests.Common;
//using Kindred.Rewards.Rewards.UnitTests.TestUtils;

//using Moq;

//using NUnit.Framework;

//namespace Kindred.Rewards.Rewards.UnitTests.EventHandlers.Kafka;

//[TestFixture]
//[Category("Unit")]
//public class BetUpdateConsumerTests : BaseMapperTests<BetUpdateConsumer>
//{
//    private SettlementMessageEventValidator settlementMessageEventValidator;
//    private CouponMessageEventValidator couponMessageEventValidator;

//    private Mock<ICouponDeclinedMessageHandler> couponDeclinedMessageHandlerMock;
//    private Mock<ICouponVerifiedMessageHandler> couponVerifiedMessageHandlerMock;
//    private Mock<ICouponFailedMessageHandler> couponFailedMessageHandlerMock;

//    private Mock<ICombinationSettledMessageHandler> combinationSettledMessageHandlerMock;
//    private Mock<IResultsReversedMessageHandler> resultsReversedMessageHandlerMock;
//    private Mock<ISettlementAdjustedMessageHandler> settlementAdjustedMessageHandlerMock;

//    private BetUpdateConsumer subject;
//    private Mock<IMessageContext> context;

//    [SetUp]
//    public void SetUp()
//    {
//        this.context = new();
//        var messageHeaders = new MessageHeaders { { "correlationId", Encoding.ASCII.GetBytes(TestConstants.CorrelationId) } };
//        this.context.Setup(x => x.Headers).Returns(messageHeaders);

//        this.settlementMessageEventValidator = new();
//        this.couponMessageEventValidator = new();


//        this.couponFailedMessageHandlerMock = new();
//        this.couponVerifiedMessageHandlerMock = new();
//        this.couponDeclinedMessageHandlerMock = new();

//        this.combinationSettledMessageHandlerMock = new();
//        this.resultsReversedMessageHandlerMock = new();
//        this.settlementAdjustedMessageHandlerMock = new();

//        this.subject = new(
//            this.LoggerMock.Object,
//            this.couponVerifiedMessageHandlerMock.Object,
//            this.couponDeclinedMessageHandlerMock.Object,
//            this.couponFailedMessageHandlerMock.Object,
//            this.couponMessageEventValidator,
//            this.combinationSettledMessageHandlerMock.Object,
//            this.resultsReversedMessageHandlerMock.Object,
//            this.settlementAdjustedMessageHandlerMock.Object,
//            this.settlementMessageEventValidator);
//    }

//    [Test]
//    public async Task SettlementMessageHandler_DelegatesControlToRelevantService_WhenSettlementActionIsCombinationSettled()
//    {
//        //arrange
//        var settlementMessage = new SettlementMessage
//        {
//            Resources = new List<Bet>
//            {
//                this.Fixture.Create<Bet>()
//            },
//            Actions = new List<SettlementAction>
//            {
//                this.Fixture.Create<CombinationSettled>()
//            }
//        };

//        //act
//        await this.subject.Handle(this.context.Object, settlementMessage);

//        //assert
//        this.combinationSettledMessageHandlerMock.Verify(x =>
//                x.Handle(It.Is<Bet>(b => b.Rn == settlementMessage.Resources.First().Rn),
//                    It.Is<SettlementAction>(a => a.OperatorId == settlementMessage.Actions.First().OperatorId),
//                    It.Is<string>(x => x.Equals(TestConstants.CorrelationId))),
//            Times.Once);
//    }

//    [Test]
//    public async Task SettlementMessageHandler_DelegatesControlToRelevantService_WhenSettlementActionIsResultsReversed()
//    {
//        //arrange
//        var settlementMessage = new SettlementMessage
//        {
//            Resources = new List<Bet>
//            {
//                this.Fixture.Create<Bet>()
//            },
//            Actions = new List<SettlementAction>
//            {
//                this.Fixture.Create<ResultsReversed>()
//            }
//        };

//        //act
//        await this.subject.Handle(this.context.Object, settlementMessage);

//        //assert
//        this.resultsReversedMessageHandlerMock.Verify(x =>
//                x.Handle(It.Is<Bet>(b => b.Rn == settlementMessage.Resources.First().Rn),
//                    It.Is<SettlementAction>(a => a.OperatorId == settlementMessage.Actions.First().OperatorId),
//                    It.Is<string>(x => x.Equals(TestConstants.CorrelationId))),
//            Times.Once);
//    }

//    [Test]
//    public async Task SettlementMessageHandler_DelegatesControlToRelevantService_WhenSettlementActionIsSettlementAdjustment()
//    {
//        //arrange
//        var settlementMessage = new SettlementMessage
//        {
//            Resources = new List<Bet>
//            {
//                this.Fixture.Create<Bet>()
//            },
//            Actions = new List<SettlementAction>
//            {
//                this.Fixture.Create<SettlementAdjusted>()
//            }
//        };

//        //act
//        await this.subject.Handle(this.context.Object, settlementMessage);

//        //assert
//        this.settlementAdjustedMessageHandlerMock.Verify(x =>
//                x.Handle(It.Is<Bet>(b => b.Rn == settlementMessage.Resources.First().Rn),
//                    It.Is<SettlementAction>(a => a.OperatorId == settlementMessage.Actions.First().OperatorId),
//                    It.Is<string>(x => x.Equals(TestConstants.CorrelationId))),
//            Times.Once);
//    }

//    [Test]
//    public void SettlementMessageHandler_ThrowsInvalidArgumentException_WhenSettlementActionIsNotDetermined()
//    {
//        //arrange
//        var settlementMessage = new SettlementMessage
//        {
//            Resources = new List<Bet>
//            {
//                this.Fixture.Create<Bet>()
//            },
//            Actions = new List<SettlementAction>
//            {
//                this.Fixture.Create<TestSettlementAction>()
//            }
//        };

//        //act
//        Assert.ThrowsAsync<InvalidArgumentException>(async () =>
//        {
//            await this.subject.Handle(this.context.Object, settlementMessage);
//        });


//        //assert
//        this.settlementAdjustedMessageHandlerMock.Verify(x =>
//                x.Handle(It.IsAny<Bet>(), It.IsAny<SettlementAction>(), It.IsAny<string>()), Times.Never());
//        this.combinationSettledMessageHandlerMock.Verify(x =>
//            x.Handle(It.IsAny<Bet>(), It.IsAny<SettlementAction>(), It.IsAny<string>()), Times.Never());
//        this.resultsReversedMessageHandlerMock.Verify(x =>
//            x.Handle(It.IsAny<Bet>(), It.IsAny<SettlementAction>(), It.IsAny<string>()), Times.Never());
//    }

//    private class TestSettlementAction : SettlementAction
//    { }
//}

