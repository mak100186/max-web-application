using FluentValidation;
using FluentValidation.Results;

using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Validation.Enums;

namespace Kindred.Rewards.Core.WebApi.Validation;

/// <inheritdoc />
public class GetRewardsByFilterRequestValidator : ValidatorBase<GetRewardsByFilterRequest>
{
    /// <inheritdoc />
    public GetRewardsByFilterRequestValidator()
    {
        RuleFor(r => r.UpdatedDateFromUtc)
            .Must(value => value == null || value.Value.UtcDateTime > DateTime.MinValue)
            .WithMessage(GenerateErrorMessage(ErrorCodes.UpdateDateFromUtcInvalid));

        RuleFor(r => r.UpdatedDateToUtc)
            .Must(value => value == null || value.Value.UtcDateTime > DateTime.MinValue)
            .WithMessage(GenerateErrorMessage(ErrorCodes.UpdateDateToUtcInvalid));

        RuleFor(x => x).Custom((req, ctx) =>
            {
                if (req.UpdatedDateFromUtc.HasValue &&
                    req.UpdatedDateToUtc.HasValue &&
                    req.UpdatedDateFromUtc > req.UpdatedDateToUtc)
                {
                    ctx.AddFailure(new ValidationFailure(
                        nameof(req.UpdatedDateToUtc),
                        GenerateErrorMessage(ErrorCodes.UpdateDateFromToUtcDependencyInvalid),
                        req.UpdatedDateToUtc));
                }
            });
    }
}
