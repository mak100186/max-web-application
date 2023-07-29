using FluentValidation;

using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Validation.Enums;

namespace Kindred.Rewards.Core.WebApi.Validation;

public class OddsLimitValidator : ValidatorBase<OddLimitsApiModel>
{
    public OddsLimitValidator()
    {
        RuleFor(x => x.MinimumCompoundOdds)
            .Empty()
            .When(x => x.MinimumStageOdds is >= 0)
            .WithMessage(GenerateErrorMessage(RewardErrorCodes.MinimumCompoundAndStageOddsShouldBeExclusive));
        RuleFor(x => x.MinimumStageOdds)
            .Empty()
            .When(x => x.MinimumCompoundOdds is >= 0)
            .WithMessage(GenerateErrorMessage(RewardErrorCodes.MinimumCompoundAndStageOddsShouldBeExclusive));
    }

}
