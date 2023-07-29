using System.Collections;

using FluentAssertions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardClaims;
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
public class ReloadFeatureTests : TestBase
{
    private static IEnumerable ReloadTestDataFactory
    {
        get
        {
            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Winning),
                                new(BetOutcome.Losing)
                        },
                    3,
                    null,
                    3)
                .SetName("Original3ReloadUnlimitedStop3Lost1Win1Pending0Remaining2")
                .Returns(2);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(null)
                        },
                    2,
                    1,
                    1)
                .SetName("Original2Reload1Stop1Lost0Win0Pending1Remaining1")
                .Returns(1);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing)
                        },
                    2,
                    1,
                    1)
                .SetName("Original2Reload1Stop1Lost1Win0Pending0Remaining2")
                .Returns(2);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(null),
                                new(null)
                        },
                    2,
                    1,
                    1)
                .SetName("Original2Reload1Stop1Lost0Win0Pending2Remaining0")
                .Returns(null);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(null)
                        },
                    2,
                    1,
                    1)
                .SetName("Original2Reload1Stop1Lost1Win0Pending1Remaining1")
                .Returns(1);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing)
                        },
                    2,
                    1,
                    1)
                .SetName("Original2Reload1Stop1Lost2Win0Pending0Remaining1")
                .Returns(1);

            yield return new TestCaseData(
                    new List<TestClaimUsage>(),
                    2,
                    1,
                    1)
                .SetName("Original2Reload1Stop1Lost0Win0Pending0Remaining2")
                .Returns(2);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(null),
                                new(null)
                        },
                    2,
                    1,
                    1)
                .SetName("Original2Reload1Stop1Lost0Win0Pending2Remaining0")
                .Returns(null);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(null)
                        },
                    2,
                    2,
                    1)
                .SetName("Original2Reload2Stop1Lost0Win0Pending1Remaining1")
                .Returns(1);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(null),
                                new(null)
                        },
                    2,
                    2,
                    1)
                .SetName("Original2Reload2Stop1Lost0Win0Pending2Remaining0")
                .Returns(null);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(null)
                        },
                    2,
                    2,
                    1)
                .SetName("Original2Reload2Stop1Lost1Win0Pending1Remaining1")
                .Returns(1);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing)
                        },
                    2,
                    2,
                    1)
                .SetName("Original2Reload2Stop1Lost2Win0Pending0Remaining2")
                .Returns(2);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing),
                                new(null)
                        },
                    2,
                    2,
                    1)
                .SetName("Original2Reload2Stop1Lost2Win0Pending1Remaining1")
                .Returns(1);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing),
                                new(null),
                                new(null)
                        },
                    2,
                    2,
                    1)
                .SetName("Original2Reload2Stop1Lost2Win0Pending2Remaining0")
                .Returns(null);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing),
                                new(null)
                        },
                    2,
                    2,
                    1)
                .SetName("Original2Reload2Stop1Lost3Win0Pending1Remaining0")
                .Returns(null);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing)
                        },
                    2,
                    2,
                    1)
                .SetName("Original2Reload2Stop1Lost4Win0Pending0Remaining0")
                .Returns(null);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing)
                        },
                    2,
                    2,
                    1)
                .SetName("Original2Reload2Stop1Lost4Win0Pending0Remaining0")
                .Returns(null);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing),
                                new(BetOutcome.Winning)
                        },
                    2,
                    2,
                    1)
                .SetName("Original2Reload2Stop1Lost2Win1Pending0Remaining0")
                .Returns(null);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(BetOutcome.Losing),
                                new(BetOutcome.Winning)
                        },
                    2,
                    2,
                    2)
                .SetName("Original2Reload2Stop2Lost2Win1Pending0Remaining1")
                .Returns(1);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(BetOutcome.Winning)
                        },
                    2,
                    2,
                    2)
                .SetName("Original2Reload2Stop2Lost1Win1Pending0Remaining1")
                .Returns(1);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Winning),
                                new(BetOutcome.Winning)
                        },
                    2,
                    2,
                    2)
                .SetName("Original2Reload2Stop2Lost0Win2Pending0Remaining0")
                .Returns(null);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(BetOutcome.FullRefund),
                                new(BetOutcome.Losing),
                                new(BetOutcome.FullRefund)
                        },
                    2,
                    null,
                    2)
                .SetName("Original2ReloadUnlimitedStop2Lost4Win0Pending0Remaining2")
                .Returns(2);

            yield return new TestCaseData(
                    new List<TestClaimUsage>
                        {
                                new(BetOutcome.Losing),
                                new(BetOutcome.FullRefund),
                                new(BetOutcome.Losing),
                                new(BetOutcome.FullRefund),
                                new(null),
                                new(null)
                        },
                    2,
                    null,
                    2)
                .SetName("Original2ReloadUnlimitedStop2Lost4Win0Pending2Remaining0")
                .Returns(null);
        }
    }

    [Test]
    public void ShouldThrowReloadNotSupported()
    {
        // arrange
        var scope = new TestScope();
        scope.Feature.Enabled = false;

        // act
        var result = Assert.Throws<RewardsValidationException>(() => scope.Feature.ApplyCreationRules(scope.Reward));

        // assert
        result.Message.Should().Contain($"{FeatureType.Reload} is not supported");
    }

    [Test]
    public void ReloadIsNotRequiredIfClaimsPerIntervalIsNull()
    {
        // arrange
        var scope = new TestScope();
        scope.Reward.Terms.Restrictions.ClaimsPerInterval = null;

        // act
        var result = Assert.Throws<RewardsValidationException>(() => scope.Feature.ApplyCreationRules(scope.Reward));

        // assert
        result.Message.Should().Contain($"Reload configuration is not required if {nameof(scope.Reward.Terms.Restrictions.ClaimsPerInterval)} is null");
    }

    [Test]
    public void ReloadIsRequiredIfClaimsPerIntervalIsNotNull()
    {
        // arrange
        var scope = new TestScope();
        scope.Reward.Terms.Restrictions.Reload = null;

        // act
        var result = Assert.Throws<RewardsValidationException>(() => scope.Feature.ApplyCreationRules(scope.Reward));

        // assert
        result.Message.Should().Contain($"Reload configuration is required if {nameof(scope.Reward.Terms.Restrictions.ClaimsPerInterval)} is not null");
    }

    [TestCase(null)]
    [TestCase(5)]
    public void MaxReloadCanBeNullOrGreaterThanZero(int? maxReload)
    {
        // arrange
        var scope = new TestScope();
        scope.Reward.Terms.Restrictions.Reload.MaxReload = maxReload;

        // act & assert
        Assert.DoesNotThrow(() => scope.Feature.ApplyCreationRules(scope.Reward));
    }

    [TestCase(0)]
    public void MaxReloadCannotBeZero(int maxReload)
    {
        // arrange
        var scope = new TestScope();
        scope.Reward.Terms.Restrictions.Reload.MaxReload = maxReload;

        // act
        var result = Assert.Throws<RewardsValidationException>(() => scope.Feature.ApplyCreationRules(scope.Reward));

        // assert
        result.Message.Should().Contain($"{nameof(scope.Reward.Terms.Restrictions.Reload.MaxReload)} should be null or greater than 0");
    }

    [TestCase(0)]
    public void StopOnMinWinBetCannotBeZero(int minWinBet)
    {
        // arrange
        var scope = new TestScope();
        scope.Reward.Terms.Restrictions.Reload.StopOnMinimumWinBets = minWinBet;

        // act
        var result = Assert.Throws<RewardsValidationException>(() => scope.Feature.ApplyCreationRules(scope.Reward));

        // assert
        result.Message.Should().Contain($"{nameof(scope.Reward.Terms.Restrictions.Reload.StopOnMinimumWinBets)} should be greater than 0");
    }

    [Test]
    public void ReturnNullIfOriginalAndReloadIsNull()
    {
        // arrange
        var scope = new TestScope();
        scope.Usage.ClaimRemaining = 0;
        scope.Reward.Terms.Restrictions.Reload = null;

        // act
        var remaining = scope.Feature.GetRemainingClaims(scope.Reward, scope.Usage);

        // assert
        remaining.Should().BeNull();
    }

    [Test]
    public void StopReloadIfMinimumWinBetLimitIsReached()
    {
        // arrange
        var scope = new TestScope();
        scope.Usage.ClaimRemaining = 0;
        scope.Usage.BetOutcomeStatuses = new List<BetOutcome?>
        {
                                                                 BetOutcome.Winning,
                                                                 BetOutcome.WinningAndPartialRefund
                                                             };
        scope.Reward.Terms.Restrictions.Reload.StopOnMinimumWinBets = 2;

        // act
        var remaining = scope.Feature.GetRemainingClaims(scope.Reward, scope.Usage);

        // assert
        remaining.Should().BeNull();
        scope.Logger.Verify(v => v.Log(It.Is<LogLevel>(a1 => a1 == LogLevel.Information), It.Is<EventId>(e => e == 0), It.Is<It.IsAnyType>((s, _) => s.ToString().Contains("Stop reload. Winning count is reached.")), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), "Info not logged");
    }

    [Test]
    public void ShouldWaitForAllPendingBets()
    {
        // arrange
        var scope = new TestScope();
        scope.Reward.Terms.Restrictions.ClaimsPerInterval = 2;
        scope.Usage.ActiveUsagesCount = 2;
        scope.Usage.BetOutcomeStatuses = new List<BetOutcome?> { null, null };
        scope.Usage.ClaimRemaining = scope.Reward.Terms.Restrictions.ClaimsPerInterval - scope.Usage.ActiveUsagesCount;

        // act
        var remaining = scope.Feature.GetRemainingClaims(scope.Reward, scope.Usage);

        // assert
        remaining.Should().BeNull();
        scope.Logger.Verify(v => v.Log(It.Is<LogLevel>(a1 => a1 == LogLevel.Information), It.Is<EventId>(e => e == 0), It.Is<It.IsAnyType>((s, _) => s.ToString().Contains("Waiting for all claims bet outcome.")), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), "Info not logged");
    }

    [TestCaseSource(typeof(ReloadFeatureTests), nameof(ReloadTestDataFactory))]
    public int? ShouldOfferReload(List<TestClaimUsage> usages, int claimsPerInterval, int? maxReload, int stop)
    {
        var scope = new TestScope
        {
            Usage = new()
            {
                ClaimRemaining = claimsPerInterval - usages.Count,
                ActiveUsagesCount = usages.Count,
                BetOutcomeStatuses = new List<BetOutcome?>()
            }
        };

        foreach (var u in usages)
        {
            scope.Usage.BetOutcomeStatuses.Add(u.BetOutcome);
        }

        scope.Reward.Terms.Restrictions.ClaimsPerInterval = claimsPerInterval;
        scope.Reward.Terms.Restrictions.Reload.MaxReload = maxReload;
        scope.Reward.Terms.Restrictions.Reload.StopOnMinimumWinBets = stop;

        var remaining = scope.Feature.GetRemainingClaims(scope.Reward, scope.Usage);

        return remaining?.ClaimRemaining;
    }

    public class TestClaimUsage
    {
        public TestClaimUsage(BetOutcome? betOutcome)
        {
            BetOutcome = betOutcome;
        }

        public BetOutcome? BetOutcome { get; }
    }

    private class TestScope
    {
        public TestScope()
        {
            Logger = new();
            Feature = new ReloadFeature(Logger.Object) { Enabled = true };
            Reward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.UniboostReload, null);
            Usage = new()
            {
                ActiveUsagesCount = 1,
                BetOutcomeStatuses = new List<BetOutcome?> { BetOutcome.Losing },
                ClaimRemaining = 1
            };
        }

        public Mock<ILogger<ReloadFeature>> Logger { get; }

        public IFeature Feature { get; }

        public RewardDomainModel Reward { get; }

        public RewardClaimUsageDomainModel Usage { get; set; }
    }
}
