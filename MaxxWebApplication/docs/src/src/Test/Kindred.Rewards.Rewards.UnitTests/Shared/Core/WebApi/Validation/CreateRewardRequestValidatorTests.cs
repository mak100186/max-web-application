using FluentAssertions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Validation;
using Kindred.Rewards.Core.WebApi.Validation.Enums;
using Kindred.Rewards.Plugin.Reward.Models;
using Kindred.Rewards.Rewards.Tests.Common;
using Kindred.Rewards.Rewards.UnitTests.Common;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.WebApi.Validation;

[TestFixture]
[Category("Unit")]
public class CreateBonusRequestValidatorTests : TestBase
{
    private ValidatorBase<RewardRequest> _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new RewardRequestValidator();
    }

    [Test]
    public void ValidRequestShouldPass()
    {
        // Arrange
        var request = new RewardRequest
        {
            CustomerId = new('1', 10),
            RewardType = RewardType.Freebet.ToString(),
            Restrictions = new()
            {
                StartDateTime = DateTime.UtcNow,
                ExpiryDateTime = DateTime.UtcNow.AddDays(1),
                ClaimInterval = "0 0 0 ? * *"
            },
            RewardParameters = new FreeBetParametersApiModel
            {
                Amount = 2355
            },
            Name = new('a', 100),
            Comments = new('a', 300),
            CountryCode = TestConstants.DefaultCountryCode
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.Should().BeEmpty();
    }

    [TestCase("blah")]
    public void CronExpressionShouldThrowValidationError(string cronExpression)
    {
        // Arrange
        var request = new RewardRequest
        {
            Restrictions = new()
            {
                ClaimInterval = cronExpression,
                ClaimAllowedPeriod = cronExpression,
                TimezoneId = "invalid"
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(
            e => e.ErrorMessage.Contains(
                GetExpectedErrorMessage(RewardErrorCodes.ClaimIntervalExpressionIsNotValid)));
        result.Errors.Should().Contain(
            e => e.ErrorMessage.Contains(
                GetExpectedErrorMessage(RewardErrorCodes.ClaimAllowedPeriodExpressionIsNotValid)));
        result.Errors.Should().Contain(
            e => e.ErrorMessage.Contains(
                GetExpectedErrorMessage(RewardErrorCodes.TimezoneIdShouldBeValid)));
    }

    [Test]
    public void ClaimAllowedPeriodIsRequiredWhenTimezoneIsGiven()
    {
        // Arrange
        var request = new RewardRequest
        {
            Restrictions = new()
            {
                TimezoneId = TimeZoneInfo.Utc.Id
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(
            e => e.ErrorMessage.Contains(
                GetExpectedErrorMessage(RewardErrorCodes.ClaimAllowedPeriodIsRequiredWhenTimezoneIsGiven)));
    }

    [Test]
    public void RequiredFieldsShouldThrowError()
    {
        // Arrange
        var request = new RewardRequest();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(RewardErrorCodes.RewardNameIsRequired)));
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(RewardErrorCodes.RewardTypeIsInvalid)));
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(RewardErrorCodes.RestrictionsIsRequired)));
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(RewardErrorCodes.RewardParametersIsRequired)));
    }

    [Test]
    public void MaxLengthFieldsShouldThrowError()
    {
        // Arrange
        var request = new RewardRequest
        {
            Name = new('a', 101),
            Comments = new('a', 301),
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(RewardErrorCodes.DescriptionMaxLengthIs300)));
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(RewardErrorCodes.NameMaxLengthIs100)));
    }

    [TestCase("2018-01-15", "2018-01-15", RewardErrorCodes.ExpiryDateShouldBeGreaterThanStartDate)]
    [TestCase("null", "1990-01-15", RewardErrorCodes.ExpiryDateShouldBeGreaterThanUtcNow)]
    [TestCase("2018-01-16", "2018-01-15", RewardErrorCodes.StartDateShouldBeLessThanExpiryDate)]
    public void RestrictionTermsShouldBeValid(string startDate, string expiryDate, RewardErrorCodes expectedErrorCode)
    {
        // Arrange
        var request = new RewardRequest
        {
            Restrictions = new()
        };

        if (DateTime.TryParse(startDate, out var startDateTime))
        {
            request.Restrictions.StartDateTime = startDateTime;
        }

        if (DateTime.TryParse(expiryDate, out var expiryDateTime))
        {
            request.Restrictions.ExpiryDateTime = expiryDateTime;
        }

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.Should().NotContain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(RewardErrorCodes.ClaimIntervalExpressionIsNotValid)));
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(expectedErrorCode)));
    }

    [Test]
    public void StartDateTimeMustBeLessThanMaxDate()
    {
        // Arrange
        var request = new RewardRequest
        {
            Restrictions = new() { StartDateTime = DateTime.MaxValue }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(RewardErrorCodes.StartDateShouldBeLessThanMaxDate)));
    }

    [Test]
    public void StartDateAndExpiryDateShouldBeOptional()
    {
        // Arrange
        string customerId = new('1', 10);
        var request = new RewardRequest
        {
            CustomerId = new('1', 10),
            RewardType = RewardType.Freebet.ToString(),
            Restrictions = new()
            {
                ClaimInterval = "0 0 0 ? * *"
            },
            RewardParameters = new FreeBetParametersApiModel
            {
                Amount = 111
            },
            CountryCode = TestConstants.DefaultCountryCode,
            Name = $"{customerId}TestName"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.Errors.Should().BeEmpty();
    }

    [TestCase("A_a0B-b5Z.z 9", true)]
    [TestCase("OddsBoost", true)]
    [TestCase("123", true)]
    [TestCase("&", false)]
    [TestCase("/", false)]
    public void NameShouldNotContainSpecialChar(string value, bool isValid)
    {
        // arrange
        var request = new RewardRequest { Name = value };

        // act
        var result = _validator.Validate(request);

        // assert
        if (isValid)
        {
            result.Errors.Should().NotContain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(RewardErrorCodes.RewardNameShouldNotContainInvalidChar)));
        }
        else
        {
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(RewardErrorCodes.RewardNameShouldNotContainInvalidChar)));
        }
    }

    [TestCase("OddsBoost.")]
    [TestCase(".123")]
    [TestCase(".123.")]
    public void NameShouldNotContainLeadingOrTrailingDot(string value)
    {
        // arrange
        var request = new RewardRequest { Name = value };

        // act
        var result = _validator.Validate(request);

        // assert
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(RewardErrorCodes.RewardNameShouldNotContainLeadingOrTrailingDot)));
    }
}
