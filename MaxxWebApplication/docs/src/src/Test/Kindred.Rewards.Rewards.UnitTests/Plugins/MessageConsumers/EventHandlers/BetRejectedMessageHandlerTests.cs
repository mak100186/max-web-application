using System.Text;

using AutoFixture;

using Confluent.Kafka;

using FluentValidation;
using FluentValidation.Results;

using Kindred.Customer.WalletGateway.ExternalModels.ChimeraModels;
using Kindred.Customer.WalletGateway.ExternalModels.Common.Enums;
using Kindred.Rewards.Plugin.MessageConsumers.EventHandlers;
using Kindred.Rewards.Plugin.MessageConsumers.Services;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using BetRejected = Kindred.Rewards.Plugin.MessageConsumers.EventHandlers.BetRejected;

namespace Kindred.Rewards.Rewards.UnitTests.Plugins.MessageConsumers.EventHandlers;

[TestFixture]
[Category("Unit")]
public class BetRejectedMessageHandlerTests
{
    private BetRejectedMessageHandler _subject;
    private Mock<IClaimService> _claimService;

    [SetUp]
    public void SetUp()
    {
        var mockLogger = new Mock<ILogger<BetRejectedMessageHandler>>();
        var mockValidator = new Mock<IValidator<BetMessage>>();
        _claimService = new();


        mockValidator.Setup(x => x.ValidateAsync(It.IsAny<BetMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _subject = new(mockValidator.Object, _claimService.Object, mockLogger.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _claimService.Reset();
    }

    [Test]
    public void BetMessageHandler_DelegatesControlToRelevantService_WhenBetIsRejected()
    {
        //arrange
        Fixture f = new();
        var betMessage = f.Create<BetRejected>();
        betMessage.Resources.First().Status = BetStatus.Rejected;

        //act
        _subject.ProcessMessage(new Message<string, BetRejected>
        {
            Headers = new()
            {
                { "correlationId", Encoding.ASCII.GetBytes("testCorrelationId") }
            },
            Value = betMessage,
            Key = betMessage.Resources.First().Rn,
            Timestamp = Confluent.Kafka.Timestamp.Default
        });

        //assert
        _claimService.Verify(x => x.ReActivateClaimAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void BetMessageHandler_DelegatesControlToRelevantService_WhenBetIsNotRejected()
    {
        //arrange
        Fixture f = new();
        var betMessage = f.Create<BetRejected>();
        betMessage.Resources.First().Status = f.Create<Generator<BetStatus>>().First(s => BetStatus.Rejected != s);

        //act
        _subject.ProcessMessage(new Message<string, BetRejected>
        {
            Headers = new()
            {
                { "correlationId", Encoding.ASCII.GetBytes("testCorrelationId") }
            },
            Value = betMessage,
            Key = betMessage.Resources.First().Rn,
            Timestamp = Confluent.Kafka.Timestamp.Default
        });

        //assert
        _claimService.Verify(x => x.ReActivateClaimAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()), Times.Never);
    }
}
