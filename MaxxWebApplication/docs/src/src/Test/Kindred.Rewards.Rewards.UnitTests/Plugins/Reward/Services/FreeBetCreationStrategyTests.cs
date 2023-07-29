using System.Globalization;

using AutoFixture;
using AutoFixture.AutoMoq;

using FluentAssertions;

using Kindred.Infrastructure.Core.Extensions.Extensions;

using Kindred.Rewards.Core;
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
public class FreeBetCreationStrategyTests : TestBase
{
    [Test]
    public void ThrowErrorIfRequiredParameterIsMissing()
    {
        // Arrange
        var scope = new TestScope();
        scope.TestReward.Terms.RewardParameters = new Dictionary<string, string> { { "currency", "aud" } };

        // Act
        var error = Assert.Throws<RewardsValidationException>(() => scope.Strategy.ValidateAndInitialise(scope.TestReward));

        // Assert
        var expectedError = $"Reward parameter(s) [{scope.Strategy.RequiredParameterKeys.ToCsv()}] is required";
        error.Message.Should().Contain(expectedError);
    }

    [TestCase("a")]
    public void ThrowErrorIfAmountIsNotDecimal(string amount)
    {
        // Arrange
        var scope = new TestScope();
        scope.TestReward.Terms.RewardParameters = new Dictionary<string, string>
        {
            {RewardParameterKey.Amount, amount},
            {RewardParameterKey.MinStages, "2"},
            {RewardParameterKey.MaxExtraWinnings, "100"}
        };

        // Act
        var error = Assert.Throws<RewardsValidationException>(() => scope.Strategy.ValidateAndInitialise(scope.TestReward));

        // Assert
        error.Message.Should().Contain("Amount parameter must be a valid decimal value");
    }

    [TestCase("singles,doubles", "2", "20", true)]
    [TestCase("canadian,singles,doubles", "2", "10", true)]
    [TestCase("singles,doubles", "2", "10", true)]
    [TestCase("doubles,canadian", "2", "8", true)]
    [TestCase("singles", "1", "1", true)]
    [TestCase("canadian", "2", "15", true)]
    [TestCase("singles,doubles", "2", "12", true)]
    [TestCase("singles", "2", "15", true)]
    [TestCase("singles,canadian", "1", "1", false)]
    [TestCase("singles,doubles,canadian", "1", "1", false)]
    [TestCase("singles,doubles,canadian", "1", "21", false)]
    [TestCase("singles,doubles,canadian", "A", "B", false)]
    public void ThrowIfMultiConfigsAreInvalid(string allowedFormulae, string minNumberOfLegs, string maxNumberOfLegs, bool shouldPass)
    {
        // Arrange
        var scope = new TestScope();
        scope.TestReward.Terms.RewardParameters = new Dictionary<string, string>
        {
            {RewardParameterKey.Amount, 5m.ToString(CultureInfo.InvariantCulture)},
            {RewardParameterKey.MinStages, minNumberOfLegs},
            {RewardParameterKey.MaxStages, maxNumberOfLegs},
            {RewardParameterKey.AllowedFormulae, allowedFormulae},
            {RewardParameterKey.MaxExtraWinnings, "100"}
        };

        // Act & Assert
        if (shouldPass)
        {
            Assert.DoesNotThrow(() =>
            {
                scope.Strategy.ValidateAndInitialise(scope.TestReward);
            });
        }
        else
        {
            Assert.Throws<RewardsValidationException>(() =>
            {
                scope.Strategy.ValidateAndInitialise(scope.TestReward);
            });
        }
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
        var error = Assert.Throws<RewardsValidationException>(() => scope.Strategy.ValidateAndInitialise(scope.TestReward));

        // Assert
        error.Message.Should().Contain($"Reward parameter {RewardParameterKey.MaxExtraWinnings} must contain a valid decimal greater than 0. Value found was: [{maxExtraWinnings}]");
    }

    [Test]
    public void ValidateTermsShouldPass()
    {
        // Arrange
        var scope = new TestScope();
        scope.TestReward.Terms.RewardParameters = new Dictionary<string, string>
        {
            {RewardParameterKey.Amount, "5.39"},
            {RewardParameterKey.MaxExtraWinnings, "100"}
        };
        scope.TestReward.Terms.Restrictions = new()
        {
            ClaimInterval = DomainConstants.InfiniteCronInterval,
            ClaimsPerInterval = 1
        };

        // Act
        var result = scope.Strategy.ValidateAndInitialise(scope.TestReward);

        // Assert
        result.Should().BeTrue();
    }

    private class TestScope
    {
        public TestScope()
        {
            Fixture = new Fixture();
            Fixture.Customize(new AutoMoqCustomization());
            var logger = Fixture.Freeze<Mock<ILogger<FreeBetCreationStrategy>>>();

            TestReward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, null);

            Strategy = new FreeBetCreationStrategy(logger.Object);
        }

        public RewardCreationStrategyBase Strategy { get; }

        public RewardDomainModel TestReward { get; }

        public IFixture Fixture { get; }
    }
}
