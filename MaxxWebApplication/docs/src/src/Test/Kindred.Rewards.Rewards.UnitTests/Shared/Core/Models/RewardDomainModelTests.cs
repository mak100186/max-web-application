using FluentAssertions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Models;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[Category("Unit")]
public class RewardDomainModelTests
{
    [Test]
    public void HasRewardStarted_ShouldReturnTrue_WhenStartDateTime_LessThanTimeNow()
    {
        // Arrange
        var model = BonusDomainModelBuilder.Create<RewardDomainModel>();
        var dateTwoDaysAgo = DateTime.Now.AddDays(-2);

        // Act
        model.Terms.Restrictions.StartDateTime = dateTwoDaysAgo;
        var hasRewardStarted = model.HasRewardStarted();

        // Assert
        hasRewardStarted.Should().BeTrue();
    }

    [Test]
    public void HasRewardStarted_ShouldReturnFalse_WhenStartDateTime_MoreThanTimeNow()
    {
        // Arrange
        var model = BonusDomainModelBuilder.Create<RewardDomainModel>();
        var dateTwoDaysInTheFuture = DateTime.Now.AddDays(2);

        // Act
        model.Terms.Restrictions.StartDateTime = dateTwoDaysInTheFuture;
        var hasRewardStarted = model.HasRewardStarted();

        hasRewardStarted.Should().BeFalse();
    }

    [Test]
    public void GetCurrentStatus_ShouldReturnCancelled_WhenRewardIsCancelledIsTrue()
    {
        // Arrange
        var model = BonusDomainModelBuilder.Create<RewardDomainModel>();

        // Act
        model.IsCancelled = true;
        var actual = model.GetCurrentStatus();
        var expected = RewardStatus.Cancelled;

        // Assert
        actual.Should().Be(expected);
    }

    [Test]
    public void GetCurrentStatus_ShouldReturnExpired_WhenRewardStartDateTime_LessThanTimeNow()
    {
        // Arrange
        var model = BonusDomainModelBuilder.Create<RewardDomainModel>();
        var dateTwoDaysAgo = DateTime.Now.AddDays(-2);

        // Act
        model.Terms.Restrictions.ExpiryDateTime = dateTwoDaysAgo;
        var actual = model.GetCurrentStatus();
        var expected = RewardStatus.Expired;

        // Assert
        actual.Should().Be(expected);
    }

    [Test]
    public void GetCurrentStatus_ShouldReturnActive_WhenRewardStartDateTime_MoreThanTimeNow()
    {
        // Arrange
        var model = BonusDomainModelBuilder.Create<RewardDomainModel>();
        var dateTwoDaysInTheFuture = DateTime.Now.AddDays(2);

        // Act
        model.Terms.Restrictions.ExpiryDateTime = dateTwoDaysInTheFuture;
        var actual = model.GetCurrentStatus();
        var expected = RewardStatus.Active;

        // Assert
        actual.Should().Be(expected);
    }
}
