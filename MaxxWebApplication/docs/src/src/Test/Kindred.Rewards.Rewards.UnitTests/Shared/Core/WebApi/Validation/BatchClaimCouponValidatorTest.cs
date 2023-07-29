using System.ComponentModel.DataAnnotations;

using FluentAssertions;

using FluentValidation.TestHelper;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Models.Rn;
using Kindred.Rewards.Core.WebApi;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Validation;
using Kindred.Rewards.Core.WebApi.Validation.Enums;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.UnitTests.Common;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.WebApi.Validation;

[TestFixture]
[Category("Unit")]
public class BatchClaimCouponValidatorTest : TestBase
{
    private BatchClaimCouponValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new();
    }

    [Test]
    public void ShouldThrowErrorForRequiredFields()
    {
        // Arrange
        var request = new BatchClaimRequest
        {
            Claims = new()
            {
                new(),
                new()
                {
                    Bet = new(){ Rn = Guid.NewGuid().ToString() },
                    Hash = Guid.NewGuid().ToString()
                }
            }
        };

        // Act
        var testResult = _validator.TestValidate(request);
        var result = _validator.Validate(request);


        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(ClaimErrorCodes.CustomerIdIsRequired)));
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(ClaimErrorCodes.CouponRnIsRequired)));
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(ClaimErrorCodes.RewardRnIsRequired)));
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(ClaimErrorCodes.BetStageIsRequired)));
    }

    [Test]
    public void ShouldPassValidations()
    {
        // Arrange
        var bet = ObjectBuilder.CreateApiBet();
        var rewardRn = new RewardRn(Guid.Parse(ObjectBuilder.CreateString(useGuid: true)));

        var claims = new List<ClaimApiModel> { new() { Bet = bet, Rn = rewardRn.ToString(), Hash = ObjectBuilder.CreateString() } };

        var request = new BatchClaimRequest
        {
            Claims = claims,
            CouponRn = ObjectBuilder.CreateString(),
            CurrencyCode = ObjectBuilder.GetRandomCurrencyCode(),
            CustomerId = ObjectBuilder.CreateString()
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    [TestCase("AU")]
    [TestCase("AUST")]
    public void ShouldThrowErrorForInvalidCurrencyCode(string currencyCode)
    {
        // Arrange
        var bet = ObjectBuilder.CreateApiBet();

        var claims = new List<ClaimApiModel> { new() { Bet = bet, Rn = ObjectBuilder.CreateString(), Hash = ObjectBuilder.CreateString() } };

        var request = new BatchClaimRequest
        {
            Claims = claims,
            CouponRn = ObjectBuilder.CreateString(),
            CurrencyCode = currencyCode,
            CustomerId = ObjectBuilder.CreateString()
        };

        request.CurrencyCode = currencyCode;

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(ClaimErrorCodes.CurrencyCodeMustBeNullOrLengthOfThree)));
    }

    private static string GetExpectedErrorMessage(ClaimErrorCodes error)
    {
        return error.GetAttributeValue<DisplayAttribute>(ApiConstants.Description);
    }
}
