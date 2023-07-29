using FluentAssertions;

using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Helpers;
using Kindred.Rewards.Rewards.UnitTests.Common;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Helpers;

[TestFixture]
[Category("Unit")]
public class CronServiceTests : TestBase
{
    [Test]
    public void GetNextIntervalReturnsValidIntervalId()
    {
        // Arrange
        const string expression = DomainConstants.InfiniteCronInterval;

        // Act
        var interval = CronService.GetNextInterval(expression);

        // Assert
        interval.Should().Be(662379552000000000);
    }

    [Test]
    public void HandleNullCronAsInfiniteInterval()
    {
        // Arrange
        const string expression = null;

        // Act
        var interval = CronService.GetNextInterval(expression);

        // Assert
        interval.Should().Be(662379552000000000);
    }

    [Test]
    public void HandleEmptyCronAsInfiniteInterval()
    {
        // Arrange
        const string expression = "";

        // Act
        var interval = CronService.GetNextInterval(expression);

        // Assert
        interval.Should().Be(662379552000000000);
    }

    [Test]
    public void ValidateNullCron()
    {
        // Arrange
        const string expression = null;

        // Act
        var isValid = CronService.IsValidCron(expression);

        // Assert
        isValid.Should().BeTrue();
    }

    [Test]
    public void ValidateEmptyCron()
    {
        // Arrange
        const string expression = "";

        // Act
        var isValid = CronService.IsValidCron(expression);

        // Assert
        isValid.Should().BeTrue();
    }

    [Test]
    public void ValidateInfiniteCron()
    {
        // Arrange
        const string expression = DomainConstants.InfiniteCronInterval;

        // Act
        var isValid = CronService.IsValidCron(expression);

        // Assert
        isValid.Should().BeTrue();
    }

    [TestCase("* * * 20-30 * ? *")]
    [TestCase("0 0 0/1 1-4 * ? *")]
    [TestCase("0 0 0/1 ? * * *")]
    [TestCase("0 0 0/1 1,4,8 * ? *")]
    public void ValidateSpecialCharactersSupport(string expression)
    {
        CronService.IsValidCron(expression).Should().BeTrue();
    }

    #region GetNextIntervalInAllowedPeriod tests

    [Test]
    public void GetNextIntervalInAllowedPeriod_WhenEmptyAllowedPeriod_ReturnsValidInterval()
    {
        // Arrange
        var claimInterval = "0 0 0/1 ? * * *";//every hour
        string claimAllowedPeriod = null;//null means always

        var from = new DateTime(2020, 05, 25, 14, 3, 0, DateTimeKind.Utc);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, "", from));

        var expected = new DateTime(2020, 05, 25, 15, 0, 0, DateTimeKind.Utc);

        // Assert
        interval.Should().Be(expected);
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_WhenEmptyAllowedPeriodAndInvalidIntervalExpr_ReturnsInfinite()
    {
        // Arrange
        string claimInterval = null;//every hour
        string claimAllowedPeriod = null;//null means always

        var from = new DateTime(2020, 05, 25, 14, 3, 0, DateTimeKind.Utc);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, "", from));

        var expected = new DateTime(2099, 12, 31, 0, 0, 0, DateTimeKind.Utc);

        // Assert
        interval.Should().Be(expected);
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_NextIntOnEndOfCurrentAllowedPeriod_ReturnsNextStartOfAllowedPeriod()
    {
        // Arrange
        var claimInterval = "0 0/15 * ? * * *";//every hour
        var claimAllowedPeriod = "* * 12 ? * * *";//null means always

        var from = new DateTime(2020, 06, 01, 12, 48, 0, DateTimeKind.Local);
        var start = new DateTime(2020, 05, 31, 14, 0, 0, DateTimeKind.Local);
        var exp = new DateTime(2020, 06, 07, 13, 59, 0, DateTimeKind.Local);


        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, TimeZoneInfo.Local.Id, from, start, exp));

        var expected = new DateTime(2020, 06, 02, 12, 0, 0, DateTimeKind.Local);

        // Assert
        interval.Should().Be(expected.ToUniversalTime());
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_WhenEmptyAllowedPeriodAndStartTimeInFuture_ReturnsCorrectInterval()
    {
        // Arrange
        var claimInterval = "0 0 0/1 ? * * *";//every hour
        string claimAllowedPeriod = null;//null means always

        var from = new DateTime(2020, 05, 25, 14, 3, 0, DateTimeKind.Utc);
        var startTime = new DateTime(2020, 05, 26, 12, 7, 0, DateTimeKind.Utc);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, "", from, startTime));

        var expected = new DateTime(2020, 05, 26, 13, 0, 0, DateTimeKind.Utc);

        // Assert
        interval.Should().Be(expected);
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_WhenEmptyAllowedPeriodAndExpTimeInPast_ReturnsZero()
    {
        // Arrange
        var claimInterval = "0 0 0/1 ? * * *";//every hour
        string claimAllowedPeriod = null;//null means always

        var from = new DateTime(2020, 05, 25, 14, 3, 0, DateTimeKind.Utc);
        var expTime = new DateTime(2020, 05, 20, 12, 7, 0, DateTimeKind.Utc);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, "", from, null, expTime));

        var expected = DateTime.MinValue;

        // Assert
        interval.Should().Be(expected);
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_StartTimeInFuture_ReturnsCorrectInterval()
    {
        // Arrange
        var claimInterval = "0 0 0/1 ? * * *";//every hour
        var claimAllowedPeriod = "* * * ? * MON-WED *";//on MON to WED only

        var from = new DateTime(2020, 05, 25, 14, 3, 0, DateTimeKind.Utc);
        var startTime = new DateTime(2020, 05, 26, 12, 7, 0, DateTimeKind.Utc);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, "", from, startTime));

        var expected = new DateTime(2020, 05, 26, 13, 0, 0, DateTimeKind.Utc);

        // Assert
        interval.Should().Be(expected);
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_StartTimeInFutureAndBeforeAllowedPrd_ReturnsCorrectInterval()
    {
        // Arrange
        var claimInterval = "0 0 0/1 ? * * *";//every hour
        var claimAllowedPeriod = "* * * ? * WED-FRI *";//on MON to WED only

        var from = new DateTime(2020, 05, 25, 14, 3, 0, DateTimeKind.Utc);
        var startTime = new DateTime(2020, 05, 26, 12, 7, 0, DateTimeKind.Utc);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, "", from, startTime));

        var expected = new DateTime(2020, 05, 27, 0, 0, 0, DateTimeKind.Utc);

        // Assert
        interval.Should().Be(expected);
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_StartTimeInFutureAndAfterAllowedPrd_ReturnsCorrectInterval()
    {
        // Arrange
        var claimInterval = "0 0 0/1 ? * * *";//every hour
        var claimAllowedPeriod = "* * * ? * WED-FRI *";//on MON to WED only

        var from = new DateTime(2020, 05, 25, 14, 3, 0, DateTimeKind.Utc);
        var startTime = new DateTime(2020, 06, 1, 12, 7, 0, DateTimeKind.Utc);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, "", from, startTime));

        var expected = new DateTime(2020, 06, 3, 0, 0, 0, DateTimeKind.Utc);

        // Assert
        interval.Should().Be(expected);
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_ExpTimeInPast_ReturnsZero()
    {
        // Arrange
        var claimInterval = "0 0 0/1 ? * * *";//every hour
        var claimAllowedPeriod = "* * * ? * MON-WED *";//on MON to WED only

        var from = new DateTime(2020, 05, 25, 14, 3, 0, DateTimeKind.Utc);
        var expTime = new DateTime(2020, 05, 20, 12, 7, 0, DateTimeKind.Utc);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, "", from, null, expTime));

        var expected = DateTime.MinValue;

        // Assert
        interval.Should().Be(expected);
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_ExpTimeInFutureAndBeforeAllowedPrd_ReturnsZero()
    {
        // Arrange
        var claimInterval = "0 0 0/1 ? * * *";//every hour
        var claimAllowedPeriod = "* * * ? * WED-FRI *";//on WED to FRI only

        var from = new DateTime(2020, 05, 25, 14, 3, 0, DateTimeKind.Utc);
        var expTime = new DateTime(2020, 05, 26, 12, 7, 0, DateTimeKind.Utc);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, "", from, null, expTime));

        var expected = DateTime.MinValue;

        // Assert
        interval.Should().Be(expected);
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_ExpTimeInFutureAndAfterAllowedPrd_ReturnsCorrectInterval()
    {
        // Arrange
        var claimInterval = "0 0 0/1 ? * * *";//every hour
        var claimAllowedPeriod = "* * * ? * WED-FRI *";//on WED to FRI only

        var from = new DateTime(2020, 05, 25, 14, 3, 0, DateTimeKind.Utc);
        var expTime = new DateTime(2020, 05, 27, 1, 0, 0, DateTimeKind.Utc);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, "", from, null, expTime));

        var expected = new DateTime(2020, 05, 27, 0, 0, 0, DateTimeKind.Utc);

        // Assert
        interval.Should().Be(expected);
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_NextIntervalNotInAllowedPeriod_ReturnsAllowedPrdStartTime()
    {
        // Arrange
        var claimInterval = "0 0 3/1 ? * * *";//every hour after 3am
        var claimAllowedPeriod = "* * * ? * WED-FRI *";//on WED to FRI only

        var from = new DateTime(2020, 05, 25, 14, 3, 0, DateTimeKind.Utc);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, "", from));

        var expected = new DateTime(2020, 05, 27, 0, 0, 0, DateTimeKind.Utc);//and not 2020-05-27-03:00:00

        // Assert
        interval.Should().Be(expected);
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_NextIntervalInAllowedPeriod_ReturnsValidInterval()
    {
        // Arrange
        var claimInterval = "0 0 3/1 ? * * *";//every hour after 3am
        var claimAllowedPeriod = "* * * ? * WED-FRI *";//on WED to FRI only

        var from = new DateTime(2020, 05, 27, 14, 3, 0, DateTimeKind.Utc);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, "", from));

        var expected = new DateTime(2020, 05, 27, 15, 0, 0, DateTimeKind.Utc);

        // Assert
        interval.Should().Be(expected);
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_WithTimezone_ReturnsValidInterval()
    {
        // Arrange
        const string claimInterval = "0 0 0/1 ? * * *";//every hour 
        const string claimAllowedPeriod = "* * * ? * WED *";//on Wed only

        var from = new DateTime(2020, 05, 25, 14, 3, 0, DateTimeKind.Local);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, TimeZoneInfo.Local.Id, from));

        var expected = new DateTime(2020, 05, 27, 0, 0, 0, DateTimeKind.Local);

        // Assert
        interval.Should().Be(expected.ToUniversalTime());
    }

    [Test]
    public void GetNextIntervalInAllowedPeriod_WithTimezone_ReturnsValidInterval1()
    {
        // Arrange
        const string claimInterval = "0 0 0/1 ? * * *";//every hour 
        const string claimAllowedPeriod = "* * * ? * WED *";//on Wed only

        var from = new DateTime(2020, 05, 27, 14, 3, 0, DateTimeKind.Local);

        // Act
        var interval = new DateTime(CronService.GetNextIntervalInAllowedPeriod(claimInterval, claimAllowedPeriod, TimeZoneInfo.Local.Id, from));

        var expected = new DateTime(2020, 05, 27, 15, 0, 0, DateTimeKind.Local);

        // Assert
        interval.Should().Be(expected.ToUniversalTime());
    }

    #endregion

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void IsSatisfiedByShouldAllowEmptyCron(string cron)
    {
        CronService.IsSatisfiedBy(cron, null).Should().BeTrue();
    }

    [TestCase("* * {0}-{1} ? * * *", true)]
    [TestCase("* * {0}-{1} ? * {2},{3} *", true)]
    [TestCase("blah", false)]
    public void IsSatisfiedByShouldValidateCron(string cron, bool isValid)
    {
        if (isValid)
        {
            var expression = string.Format(
                cron,
                DateTime.UtcNow.Hour,
                DateTime.UtcNow.AddHours(1).Hour,
                (int)DateTime.UtcNow.DayOfWeek + 1,
                (int)DateTime.UtcNow.AddDays(1).DayOfWeek + 1);

            CronService.IsSatisfiedBy(expression, null).Should().BeTrue();
        }
        else
        {
            Assert.Throws<RewardsValidationException>(() => CronService.IsSatisfiedBy(cron, null));
        }
    }

    [Test]
    public void IsSatisfiedByShouldPassForGivenTimezoneId()
    {
        // arrange
        var claimAllowedPeriod = $"* * {DateTime.Now.Hour} ? * * *";

        // act
        var result = CronService.IsSatisfiedBy(claimAllowedPeriod, TimeZoneInfo.Local.Id);

        // assert
        result.Should().BeTrue();
    }
}
