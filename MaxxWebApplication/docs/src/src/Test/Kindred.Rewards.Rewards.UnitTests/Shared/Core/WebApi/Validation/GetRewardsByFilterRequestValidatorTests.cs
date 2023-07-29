using FluentAssertions;

using Kindred.Rewards.Core.WebApi;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Validation;
using Kindred.Rewards.Rewards.UnitTests.Common;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.WebApi.Validation;

[TestFixture]
[Category("Unit")]
public class GetRewardsByFilterRequestValidatorTests : TestBase
{
    private GetRewardsByFilterRequestValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new();
    }

    [Test]
    public void WhenSpecifyingNothingShouldValidateTrue()
    {
        // Arrange
        var request = new GetRewardsByFilterRequest();

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Count.Should().Be(0);
    }

    [Test]
    public void WhenSpecifyingUpdatedDateToUtcOnlyShouldValidateTrueIfAValidDateTime()
    {
        // arrange
        var request = new GetRewardsByFilterRequest
        {
            UpdatedDateToUtc = new DateTime(1955, 11, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // act
        var result = _validator.Validate(request);

        // assert
        result.IsValid.Should().BeTrue();
        result.Errors.Count.Should().Be(0);
    }

    [Test]
    public void WhenSpecifyingUpdatedDateUtcToOnlyShouldValidateFalseIfNotAValidDateTime()
    {
        // arrange
        var request = new GetRewardsByFilterRequest
        {
            UpdatedDateToUtc = DateTimeOffset.MinValue
        };

        // act
        var result = _validator.Validate(request);

        // assert
        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorMessage.Should().Contain(ApiConstants.ErrorMsgInvalidUpdateDateToUtc);
    }

    [Test]
    public void WhenSpecifyingUpdatedDateFromUtcOnlyShouldValidateTrueIfAValidDateTime()
    {
        // arrange
        var request = new GetRewardsByFilterRequest
        {
            UpdatedDateFromUtc = new DateTime(1955, 11, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // act
        var result = _validator.Validate(request);

        // assert
        result.IsValid.Should().BeTrue();
        result.Errors.Count.Should().Be(0);
    }

    [Test]
    public void WhenSpecifyingUpdatedDateFromUtcOnlyShouldValidateFalseIfNotAValidDateTime()
    {
        // arrange
        var request = new GetRewardsByFilterRequest
        {
            UpdatedDateFromUtc = DateTimeOffset.MinValue
        };

        // act
        var result = _validator.Validate(request);

        // assert
        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorMessage.Should().Contain(ApiConstants.ErrorMsgInvalidUpdateDateFromUtc);
    }

    [Test]
    public void WhenSpecifyingUpdatedDateFromAndToUtcShouldValidateFalseIfDateFromLaterThanDateTo()
    {
        // arrange
        var request = new GetRewardsByFilterRequest
        {
            UpdatedDateFromUtc = new DateTime(1955, 11, 10, 0, 0, 0, DateTimeKind.Utc),
            UpdatedDateToUtc = new DateTime(1955, 11, 5, 0, 0, 0, DateTimeKind.Utc)
        };

        // act
        var result = _validator.Validate(request);

        // assert
        result.IsValid.Should().BeFalse();
        result.Errors.Count.Should().Be(1);
        result.Errors[0].ErrorMessage.Should().Contain(ApiConstants.ErrorMsgInvalidUpdateDateToUtcDateFromUtcDependency);
    }
}
