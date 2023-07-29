using FluentValidation;

using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.WebApi;
using Kindred.Rewards.Core.WebApi.Validation.Enums;

namespace Kindred.Rewards.Plugin.Reward.Models;

public class RewardRequestValidator : RewardRequestValidatorBase<RewardRequest>
{
    public RewardRequestValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage(GenerateErrorMessage(RewardErrorCodes.RewardNameIsRequired));

        RuleFor(p => p.Name)
            .Length(1, 100)
            .WithMessage(GenerateErrorMessage(RewardErrorCodes.NameMaxLengthIs100));

        RuleFor(p => p.Name)
            .Matches(ApiConstants.ValidNameRegEx)
            .WithMessage(GenerateErrorMessage(RewardErrorCodes.RewardNameShouldNotContainInvalidChar));

        RuleFor(p => p.Name.HasLeadingOrTrailingDot())
            .Equal(false)
            .WithMessage(GenerateErrorMessage(RewardErrorCodes.RewardNameShouldNotContainLeadingOrTrailingDot));

        RuleFor(p => p.Comments)
            .Length(1, 300)
            .WithMessage(GenerateErrorMessage(RewardErrorCodes.DescriptionMaxLengthIs300));

        AddRewardValidationRules();
    }
}
