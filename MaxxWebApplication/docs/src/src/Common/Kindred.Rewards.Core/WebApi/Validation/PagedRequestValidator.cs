using FluentValidation;

using Kindred.Infrastructure.Hosting.WebApi.Extensions;
using Kindred.Rewards.Core.WebApi.Validation.Enums;

namespace Kindred.Rewards.Core.WebApi.Validation;

public class PagedRequestValidator : ValidatorBase<PagedRequest>
{
    /// <inheritdoc />
    public PagedRequestValidator()
    {
        RuleFor(request => request.Limit)
            .Must(limit => limit is >= 0)
            .When(request => request.Limit != null)
            .WithMessage(GenerateErrorMessage(ErrorCodes.LimitMustBeGreaterThanOrEqualToZeroIfProvided));

        RuleFor(r => r.Offset)
            .Must(offset => offset is >= 0)
            .When(request => request.Offset != null)
            .WithMessage(GenerateErrorMessage(ErrorCodes.OffsetMustBeGreaterThanOrEqualToZeroIfProvided));
    }
}
