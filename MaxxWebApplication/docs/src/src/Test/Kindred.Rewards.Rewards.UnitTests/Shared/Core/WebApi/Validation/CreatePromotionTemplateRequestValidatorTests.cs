using FluentAssertions;

using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Validation;
using Kindred.Rewards.Core.WebApi.Validation.Enums;
using Kindred.Rewards.Rewards.UnitTests.Common;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.WebApi.Validation;

public class CreatePromotionTemplateRequestValidatorTests : TestBase
{
    private ValidatorBase<CreateRewardTemplateRequest> _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreatePromotionTemplateRequestValidator();
    }

    [TestCase("A_a0B-b5Z.z 9", false)]
    [TestCase("A_a0B-b5Z.z9", true)]
    public void TemplateKeyShouldNotContainSpecialChar(string value, bool isValid)
    {
        // arrange
        var request = new CreateRewardTemplateRequest { TemplateKey = value };

        // act
        var result = _validator.Validate(request);

        // assert
        if (isValid)
        {
            result.Errors.Should().NotContain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(PromotionTemplateErrorCodes.TemplateKeyShouldNotContainSpecialChar)));
        }
        else
        {
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(PromotionTemplateErrorCodes.TemplateKeyShouldNotContainSpecialChar)));
        }
    }

    [TestCase(".A_a0B-b5Z.z9")]
    [TestCase("A_a0B-b5Z.z9.")]
    [TestCase(".A_a0B-b5Z.z9.")]
    public void TemplateKeyShouldNotContainLeadingOrTrailingDot(string value)
    {
        // arrange
        var request = new CreateRewardTemplateRequest { TemplateKey = value };

        // act
        var result = _validator.Validate(request);

        // assert
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(GetExpectedErrorMessage(PromotionTemplateErrorCodes.TemplateKeyShouldNotContainLeadingOrTrailingDot)));
    }
}
