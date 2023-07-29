using FluentValidation;

using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Validation.Enums;

namespace Kindred.Rewards.Core.WebApi.Validation;

/// <summary>
/// Adds basic validation for the claim coupon.  Deep validation (checks the data is valid in other systems) is not performed
/// </summary>
public class BatchClaimCouponValidator : ValidatorBase<BatchClaimRequest>
{
    /// <inheritdoc />
    public BatchClaimCouponValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage(GenerateErrorMessage(ClaimErrorCodes.CustomerIdIsRequired));
        RuleFor(x => x.CouponRn).NotEmpty().WithMessage(GenerateErrorMessage(ClaimErrorCodes.CouponRnIsRequired));
        RuleFor(x => x.Claims).NotEmpty().WithMessage(GenerateErrorMessage(ClaimErrorCodes.ClaimsAreRequired));
        When(
            x => x.Claims.Any(),
            () =>
            {
                RuleForEach(x => x.Claims).SetValidator(new ClaimValidator());
            });
        RuleFor(x => x.CurrencyCode).NotEmpty().Must(x => x == null || x.Length == 3).WithMessage(GenerateErrorMessage(ClaimErrorCodes.CurrencyCodeMustBeNullOrLengthOfThree));
    }
}
