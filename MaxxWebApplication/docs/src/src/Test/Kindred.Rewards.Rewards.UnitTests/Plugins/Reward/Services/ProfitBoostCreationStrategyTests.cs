using AutoFixture;

using FluentAssertions;

using Kindred.Infrastructure.Core.Extensions.Extensions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Plugin.Reward.Services;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.UnitTests.Common;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Plugins.Reward.Services;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[Category("Unit")]
public class ProfitBoostClaimStrategyTests : TestBase
{
    [Test]
    public void ValidateAndInitialise_ThrowError_WhenRequiredParameterIsMissing()
    {
        // Arrange
        var scope = new TestScope();
        scope.TestReward.Terms.RewardParameters = new Dictionary<string, string>();

        // Act
        var error = Assert.Throws<RewardsValidationException>(() => scope.RewardStrategy.ValidateAndInitialise(scope.TestReward));

        // Assert
        error.Message.Should().Contain($"Reward parameter(s) [{scope.RewardStrategy.RequiredParameterKeys.ToCsv()}] is required");
    }

    [Test]
    public void ValidateAndInitialise_ThrowsError_WhenInvalidParameterAreFound()
    {
        // Arrange
        var scope = new TestScope();
        scope.TestReward.Terms.RewardParameters.Add("aMaDeUpReWaRdKeY", "avalue");

        // Act
        var result = Assert.Throws<RewardsValidationException>(() => scope.RewardStrategy.ValidateAndInitialise(scope.TestReward));

        result.Message.Should().Be("Reward Parameter(s) [aMaDeUpReWaRdKeY] is not valid");
    }

    [TestCase(true, false, false)]
    [TestCase(false, true, false)]
    [TestCase(false, false, true)]
    [TestCase(false, false, false)]
    public void ValidateAndInitialise_ReturnsTrue_WhenEitherContestTypeOrContestRefsOrOutcomeRefsIsTrue(bool allowContestRef, bool allowedContestTypes, bool allowOutcomeRefs)
    {
        // Arrange
        var scope = new TestScope();
        scope.TestReward.Terms.Restrictions.AllowedContestRefs = allowContestRef ? new() { "1" } : new List<string>();
        scope.TestReward.Terms.Restrictions.AllowedContestTypes = allowedContestTypes ? new() { "baseball" } : new List<string>();
        scope.TestReward.Terms.Restrictions.AllowedOutcomes = allowOutcomeRefs ? new() { "1" } : new List<string>();

        // Act
        var result = scope.RewardStrategy.ValidateAndInitialise(scope.TestReward);

        // Assert
        result.Should().BeTrue();
    }

    [TestCase("blah")]
    [TestCase("0.00")]
    [TestCase("-0.1")]
    public void ValidateAndInitialise_ThrowsError_WhenMaxStakeAmountIsNotValid(string maxStakeAmount)
    {
        // Arrange
        var scope = new TestScope();
        scope.TestReward.Terms.RewardParameters[RewardParameterKey.MaxStakeAmount] = maxStakeAmount;

        // Act
        var error = Assert.Throws<RewardsValidationException>(() => scope.RewardStrategy.ValidateAndInitialise(scope.TestReward));

        // Assert
        error.Message.Should().Contain($"Reward parameter {RewardParameterKey.MaxStakeAmount} must contain a valid decimal greater than 0. Value found was: [{maxStakeAmount}]");
    }

    [Test]
    public void ValidateAndInitialise_ShouldReturnTrue_WhenAllValidationsPass()
    {
        // Arrange
        var scope = new TestScope();

        // Act
        var result = scope.RewardStrategy.ValidateAndInitialise(scope.TestReward);

        // Assert
        result.Should().BeTrue();
    }

    [TestCase("blah")]
    [TestCase("0.00")]
    [TestCase("-0.1")]
    public void ValidateAndInitialise_ThrowsError_WhenMaxExtraWinningsIsNotValid(string maxExtraWinnings)
    {
        // Arrange
        var scope = new TestScope();
        scope.TestReward.Terms.RewardParameters[RewardParameterKey.MaxExtraWinnings] = maxExtraWinnings;

        // Act
        var error = Assert.Throws<RewardsValidationException>(() => scope.RewardStrategy.ValidateAndInitialise(scope.TestReward));

        // Assert
        error.Message.Should().Contain($"Reward parameter {RewardParameterKey.MaxExtraWinnings} must contain a valid decimal greater than 0. Value found was: [{maxExtraWinnings}]");
    }

    private class TestScope
    {
        public TestScope()
        {
            var fixture = new Fixture();
            var logger = fixture.Freeze<Mock<ILogger<ProfitBoostCreationStrategy>>>();

            TestReward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Profitboost, null);

            RewardStrategy = new ProfitBoostCreationStrategy(logger.Object);
        }

        public RewardCreationStrategyBase RewardStrategy { get; }

        public RewardDomainModel TestReward { get; }
    }
}
