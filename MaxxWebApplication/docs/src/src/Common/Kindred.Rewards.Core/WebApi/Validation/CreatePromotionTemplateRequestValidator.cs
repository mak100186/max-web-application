using FluentValidation;

using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Validation.Enums;

namespace Kindred.Rewards.Core.WebApi.Validation;

public class CreatePromotionTemplateRequestValidator : ValidatorBase<CreateRewardTemplateRequest>
{
    public CreatePromotionTemplateRequestValidator()
    {
        RuleFor(t => t.TemplateKey)
            .Matches(ApiConstants.ValidKeyRegEx)
            .WithMessage(GenerateErrorMessage(PromotionTemplateErrorCodes.TemplateKeyShouldNotContainSpecialChar));

        RuleFor(t => t.TemplateKey.HasLeadingOrTrailingDot())
            .Equal(false)
            .WithMessage(GenerateErrorMessage(PromotionTemplateErrorCodes.TemplateKeyShouldNotContainLeadingOrTrailingDot));

        RuleFor(t => t.Comments)
            .NotEmpty()
            .MaximumLength(300)
            .WithMessage(GenerateErrorMessage(PromotionTemplateErrorCodes.TemplateDescriptionShouldNotBeLongerThan300Characters));

        RuleFor(t => t.Title)
            .MaximumLength(300)
            .WithMessage(GenerateErrorMessage(PromotionTemplateErrorCodes.TemplateTitleShouldNotBeLongerThan300Characters));
    }
}
