/* TODO: Implement. A lot of tests will need to be re-written to use in-memory db context instead of IDataAccessManager
using FluentAssertions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Infrastructure.Data.DataModels;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Logic.BAL;

/// <summary>
/// The reward claim service tests.
/// </summary>
public partial class RewardServiceTests
{
    [Test]
    public async Task ReactivateRewardShouldPublishKafkaMessage()
    {
        // Arrange
        var reward = RewardClaimBuilder.CreateBonusClaim();
        reward.Status = RewardClaimStatus.Claimed;

        var correlationId = ObjectBuilder.CreateString();

        this.SetUpClaimRepository(reward);

        // Act
        await this.target.ReActivateClaimAsync(reward.InstanceId, correlationId);

        // Assert
        this.producer.Verify(
            x => x.ProduceKafkaAsync(
                It.IsAny<string>(),
                It.Is<RewardClaim>(r => string.Equals(r.Status, nameof(RewardClaimStatus.Revoked)) && r.InstanceId == reward.InstanceId),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()),
            Times.Once);

        //this.cache.Verify(c => c.UpdateAsync(reward.CustomerId), Times.Once);
    }

    [Test]
    [TestCase(RewardClaimStatus.Revoked)]
    [TestCase(RewardClaimStatus.Settled)]
    public async Task ReactivateRewardShouldFailIfStatusIsNotClaimed(RewardClaimStatus status)
    {
        // Arrange
        var reward = RewardClaimBuilder.CreateBonusClaim();
        reward.Status = status;

        this.SetUpClaimRepository(reward);

        // Act
        await this.target.ReActivateClaimAsync(reward.InstanceId, It.IsAny<string>());

        // Assert
        this.producer.Verify(
            x => x.ProduceKafkaAsync(
                It.IsAny<string>(),
                It.Is<RewardClaim>(r => string.Equals(r.Status, nameof(RewardClaimStatus.Revoked))),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()),
            Times.Never);

        this.cache.Verify(c => c.UpdateAsync(reward.CustomerId), Times.Never);
    }

    [Test]
    public void ReactivateRewardShouldThrowArgumentNullException()
    {
        // Act
        var error = Assert.ThrowsAsync<ArgumentNullException>(
            async () => await this.target.ReActivateClaimAsync(string.Empty, It.IsAny<string>()));

        // Assert
        error.Message.Should().Contain("claimInstanceId");

        this.producer.Verify(
            x => x.ProduceKafkaAsync(
                It.IsAny<string>(),
                It.Is<RewardClaim>(r => r.Status == nameof(RewardClaimStatus.Revoked).ToLowerInvariant()),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()),
            Times.Never);
    }

    [Test]
    public async Task ReactivateRewardShouldFailIfRewardNotFound()
    {
        // Arrange
        var instanceId = ObjectBuilder.CreateString();
        this.dataAccessManager.Setup(d => d.RewardClaims.GetClaimedOrSettledAsync(It.IsAny<string>())).Returns(Task.FromResult<RewardClaimDomainModel>(null));

        // Act
        await this.target.ReActivateClaimAsync(instanceId, It.IsAny<string>());

        // Assert

        this.logger.Verify(v => v.Log<It.IsAnyType>(It.Is<LogLevel>(a1 => a1 == LogLevel.Warning), It.Is<EventId>(e => e == 0), It.Is<It.IsAnyType>((s, _) => s.ToString().Equals($"ReActivateClaimAsync:Could not find reward claim with instance Id {instanceId}")), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), "Error not logged");
        this.dataAccessManager.Verify(r => r.AddAsync(It.IsAny<RewardClaimDomainModel>()), Times.Never);
    }

    [Test]
    public void ReactivateRewardShouldThrowUnhandledException()
    {
        // Arrange
        var reward = RewardClaimBuilder.CreateBonusClaim();

        this.SetUpClaimRepository(reward);

        var correlationId = ObjectBuilder.CreateString();

        this.producer
            .Setup(x =>
                x.ProduceKafkaAsync(
                    reward.InstanceId,
                    It.IsAny<RewardClaim>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
            .Throws<Exception>();

        // Act
        Assert.ThrowsAsync<Exception>(() => this.target.ReActivateClaimAsync(reward.InstanceId, correlationId));

        // Assert
        this.logger.Verify(v => v.Log<It.IsAnyType>(It.Is<LogLevel>(a1 => a1 == LogLevel.Information), It.Is<EventId>(e => e == 0), It.Is<It.IsAnyType>((s, _) => s.ToString().Equals("Published payout reward")), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Never);
    }
}
*/
