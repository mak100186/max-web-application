//using AutoFixture;

//using FluentAssertions;

//using Kindred.Customer.WalletGateway.ExternalModels.ChimeraModels;
//using Kindred.Rewards.Plugin.MessageConsumers.EventHandlers;
//using Kindred.Rewards.Plugin.MessageConsumers.Services;
//using Kindred.Rewards.Rewards.UnitTests.TestUtils;

//using Microsoft.Extensions.Logging;

//using Moq;

//using NUnit.Framework;

//namespace Kindred.Rewards.Rewards.UnitTests.Consumers;

//[TestFixture]
//[Category("Unit")]
//public class CouponFailedMessageHandlerTests : BaseMessageHandlerTests<CouponFailedMessageHandler>
//{
//    private Coupon coupon;

//    private Mock<IClaimService> rewardService;
//    private CouponFailedMessageHandler subject;

//    [SetUp]
//    public void Setup()
//    {
//        this.rewardService = new();
//        this.subject = new(this.LoggerMock.Object, this.rewardService.Object);
//        this.coupon = this.Fixture.Create<Coupon>();
//    }

//    [TearDown]
//    public void TearDown()
//    {
//        this.LoggerMock.Reset();
//    }

//    [Test]
//    public async Task HandleCouponFailedShouldCallReActivateClaimAsync()
//    {
//        //act
//        await this.subject.Handle(this.coupon, CorrelationId);

//        //assert
//        var rewardClaims = this.coupon.Bets.SelectMany(b => b.RewardClaims).Select(rc => rc.InstanceId).Distinct();
//        this.rewardService.Verify(
//                    x => x.ReActivateClaimAsync(rewardClaims, CorrelationId),
//                    Times.Once);
//    }

//    [Test]
//    public void HandleCouponFailedLogsErrorOnException()
//    {
//        //arrange
//        this.rewardService
//            .Setup(x => x.ReActivateClaimAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
//            .Throws(new Exception("Some message"));

//        var action = () => this.subject.Handle(this.coupon, CorrelationId).GetAwaiter().GetResult();

//        //assert
//        action.Should().Throw<Exception>();
//        this.LoggerMock.VerifyLog(LogLevel.Error, 1);
//    }
//}
