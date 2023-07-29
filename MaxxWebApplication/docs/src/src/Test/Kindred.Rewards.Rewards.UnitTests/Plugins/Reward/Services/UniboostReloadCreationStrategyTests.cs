using AutoFixture;

using FluentAssertions;

using Kindred.Rewards.Core.Client;
using Kindred.Rewards.Core.Enums;
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
public class UniboostReloadCreationStrategyTests : TestBase
{
    [Test]
    public void ShouldEnableReloadFeature()
    {
        var scope = new TestScope();
        scope.RewardStrategy.Features.Should().NotBeEmpty();
        scope.RewardStrategy.Features[FeatureType.Reload].Enabled.Should().BeTrue();
    }

    [Test]
    public void ValidateAndInitialise_DoesNotThrowError_WhenOddsLadderTypeIsMissing()
    {
        // Arrange
        var scope = new TestScope();
        var requiredKeys = scope.RewardStrategy.RequiredParameterKeys;

        foreach (var keyToAdd in requiredKeys)
        {
            scope.TestReward.Terms.RewardParameters.TryAdd(keyToAdd, "123456");
        }

        // Act
        scope.TestReward.Terms.RewardParameters.Remove(RewardParameterKey.OddsLadderType);

        var action = () => scope.RewardStrategy.ValidateAndInitialise(scope.TestReward);

        // Assert
        action.Should().NotThrow();
    }

    private class TestScope
    {
        public TestScope()
        {
            var fixture = new Fixture();
            var logger = fixture.Freeze<Mock<ILogger<UniBoostReloadCreationStrategy>>>();
            var oddsLadderLogic = fixture.Freeze<Mock<IOddsLadderClient>>();

            oddsLadderLogic.Setup(c => c.GetOddsLadder(It.IsAny<string>())).ReturnsAsync(OddsLadder());

            TestReward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.UniboostReload, null);
            RewardStrategy = new UniBoostReloadCreationStrategy(logger.Object);
        }

        public RewardCreationStrategyBase RewardStrategy { get; }
        public RewardDomainModel TestReward { get; }
    }
}
