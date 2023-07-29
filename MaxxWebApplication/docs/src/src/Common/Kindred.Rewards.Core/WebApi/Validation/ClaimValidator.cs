using FluentValidation;

using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Validation.Enums;

namespace Kindred.Rewards.Core.WebApi.Validation;

/// <summary>
/// Adds basic validation for the claim coupon.  Deep validation (checks the data is valid in other systems) is not performed
/// </summary>
public class ClaimValidator : ValidatorBase<ClaimApiModel>
{
    /// <inheritdoc />
    public ClaimValidator()
    {
        RuleFor(x => x.Rn).NotEmpty().WithMessage(GenerateErrorMessage(ClaimErrorCodes.RewardRnIsRequired));
        RuleFor(x => x.Bet).NotEmpty().WithMessage(GenerateErrorMessage(ClaimErrorCodes.BetIsRequired));
        RuleFor(x => x.Hash).NotEmpty().WithMessage(GenerateErrorMessage(ClaimErrorCodes.RewardHashIsRequired));
        RuleFor(x => x.Bet).SetValidator(new BetValidator());
    }
}
