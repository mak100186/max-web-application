using AutoFixture;
using AutoFixture.AutoMoq;

using FluentAssertions;

using JetBrains.Annotations;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Plugin.Reward.Services;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.UnitTests.Common;

using Microsoft.Extensions.Logging;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Plugins.Reward.Services;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[Category("Unit")]
public class RewardCreationStrategyBaseTests : TestBase
{
    #region ValidateAndInitialise

    [Test]
    public void ValidateAndInitialise_DoNotThrowError_WhenTagsListIsEmpty()
    {
        // Arrange
        var scope = new TestScope
        {
            TestReward =
            {
                Tags = new List<string>()
            }
        };

        // Act
        var action = () => scope.RewardStrategy.ValidateAndInitialise(scope.TestReward);

        // Assert
        action.Should().NotThrow();
    }

    [Test]
    public void ValidateAndInitialise_ThrowError_WhenTagsContainEmptyString()
    {
        // Arrange
        var scope = new TestScope
        {
            TestReward =
            {
                Tags = new List<string>
                {
                    string.Empty
                }
            }
        };


        // Act
        var error = Assert.Throws<RewardsValidationException>(() => scope.RewardStrategy.ValidateAndInitialise(scope.TestReward));

        // Assert
        error.Message.Should().Contain("Tags can not have spaces and should be of one word only");
    }

    [Test]
    public void ValidateAndInitialise_ThrowError_WhenTagsContainAWhitespace()
    {
        // Arrange
        var scope = new TestScope
        {
            TestReward =
            {
                Tags = new List<string>
                {
                    "Multi Word Tag"
                }
            }
        };


        // Act
        var error = Assert.Throws<RewardsValidationException>(() => scope.RewardStrategy.ValidateAndInitialise(scope.TestReward));

        // Assert
        error.Message.Should().Contain("Tags can not have spaces and should be of one word only");
    }

    [TestCase("tag1,tag2,tag3")]
    [TestCase("tag1,tag2")]
    [TestCase("tag1")]
    [TestCase(null)]
    public void ValidateAndInitialise_ReturnsTrue_WhenTagsAreValid(string csvTags)
    {
        // Arrange
        var scope = new TestScope
        {
            TestReward =
            {
                Tags = csvTags?.Split(",")
            }
        };


        // Act
        var result = scope.RewardStrategy.ValidateAndInitialise(scope.TestReward);

        // Assert
        result.Should().BeTrue();
    }

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
        scope.TestReward.Terms.RewardParameters.Add("aMaDeUpReWaRdRn", "value");

        // Act
        var result = Assert.Throws<RewardsValidationException>(() => scope.RewardStrategy.ValidateAndInitialise(scope.TestReward));

        result.Message.Should().Be("Reward Parameter(s) [aMaDeUpReWaRdRn] is not valid");
    }

    [TestCase(true, true, true)]
    [TestCase(true, false, true)]
    [TestCase(true, true, false)]
    [TestCase(false, true, true)]
    public void ValidateAndInitialise_AcceptsAnyCombinationOfRestrictionsOnContestRefContestTypeAndOutcomes_WhenCalled(bool allowContestRef, bool allowedContestTypes, bool allowOutcomeRefs)
    {
        // Arrange
        var scope = new TestScope();
        scope.TestReward.Terms.Restrictions.AllowedContestRefs = allowContestRef ? new() { "1" } : new List<string>();
        scope.TestReward.Terms.Restrictions.AllowedContestTypes = allowedContestTypes ? new() { "baseball" } : new List<string>();
        scope.TestReward.Terms.Restrictions.AllowedOutcomes = allowOutcomeRefs ? new() { "1" } : new List<string>();

        // Act & Assert
        Assert.DoesNotThrow(() => scope.RewardStrategy.ValidateAndInitialise(scope.TestReward));
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

    #endregion

    private class TestScope
    {
        public TestScope()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());

            TestReward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, null);

            RewardStrategy = fixture.Freeze<TestRewardStrategy>();
        }

        public RewardCreationStrategyBase RewardStrategy { get; }

        public RewardDomainModel TestReward { get; }
    }

    [UsedImplicitly]
    private class TestRewardStrategy : RewardCreationStrategyBase
    {
        public TestRewardStrategy(ILogger logger, IRewardService bonusService) : base(logger)
        {
            RewardName = nameof(TestRewardStrategy);
        }

        public override IReadOnlyCollection<string> RequiredParameterKeys => new List<string>
        {
            RewardParameterKey.Amount,
            RewardParameterKey.MaxExtraWinnings
        };

        public override IReadOnlyCollection<string> OptionalParameterKeys => new List<string>
        {
            RewardParameterKey.AllowedFormulae,
            RewardParameterKey.MinStages,
            RewardParameterKey.MaxStages,
            RewardParameterKey.MinCombinations,
            RewardParameterKey.MaxCombinations
        };
    }
}
