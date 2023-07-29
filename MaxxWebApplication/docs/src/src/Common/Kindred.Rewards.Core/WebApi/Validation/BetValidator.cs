using FluentValidation;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.WebApi.Payloads.BetModel;
using Kindred.Rewards.Core.WebApi.Validation.Enums;

namespace Kindred.Rewards.Core.WebApi.Validation;

public class BetValidator : ValidatorBase<BetApiModel>
{
    public BetValidator()
    {
        RuleFor(x => x.Stages).NotEmpty().WithMessage(GenerateErrorMessage(ClaimErrorCodes.BetStageIsRequired));
        RuleFor(x => x.RequestedStake).NotEmpty().WithMessage(GenerateErrorMessage(ClaimErrorCodes.BetStakeIsRequired));
        When(x => x.Stages.IsNotNullAndNotEmpty(), () => RuleForEach(s => s.Stages).SetValidator(new BetStageValidator()));
    }
}
