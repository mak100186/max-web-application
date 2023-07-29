using FluentValidation;

using Kindred.Rewards.Core.WebApi.Requests;

namespace Kindred.Rewards.Core.WebApi.Validation;

/// <inheritdoc />
public class CancelRequestValidator : AbstractValidator<CancelRequest>
{
    /// <inheritdoc />
    public CancelRequestValidator()
    {
        When(r => !string.IsNullOrWhiteSpace(r.Reason), () => RuleFor(r => r.Reason).Length(0, 20));
    }
}
