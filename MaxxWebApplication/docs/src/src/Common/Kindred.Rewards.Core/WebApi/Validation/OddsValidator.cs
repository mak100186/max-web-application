using FluentValidation;

using Kindred.Rewards.Core.WebApi.Payloads.BetModel;
using Kindred.Rewards.Core.WebApi.Validation.Enums;

namespace Kindred.Rewards.Core.WebApi.Validation;

/// <summary>
/// Adds basic validation for the claim coupon.  Deep validation (checks the data is valid in other systems) is not performed
/// </summary>
public class OddsValidator : ValidatorBase<OddsApiModel>
{
    /// <inheritdoc />
    public OddsValidator()
    {
        RuleFor(x => x.RequestedPrice).NotEmpty().WithMessage(GenerateErrorMessage(ClaimErrorCodes.RequestedOddsAreRequired));
    }
}
