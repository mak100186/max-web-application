using FluentValidation;

using Kindred.Rewards.Core.WebApi.Enums;
using Kindred.Rewards.Core.WebApi.Payloads;

namespace Kindred.Rewards.Core.WebApi.Validation;

public class FreebetRewardParameterValidator : AbstractValidator<FreeBetParametersApiModel>
{
    public FreebetRewardParameterValidator()
    {
        RuleFor(x => x.Type)
            .Equal(nameof(RewardType.Freebet));
        RuleFor(x => x.Amount)
            .NotEmpty();
        When(x => x.MaxExtraWinnings.HasValue, () =>
        {
            RuleFor(x => x.MaxExtraWinnings).GreaterThan(0);
        });
    }
}

public class UniboostRewardParameterValidator : AbstractValidator<UniBoostParametersApiModel>
{
    public UniboostRewardParameterValidator()
    {
        RuleFor(x => x.Type)
            .Equal(nameof(RewardType.Uniboost));
        When(x => x.MaxStakeAmount.HasValue, () =>
        {
            RuleFor(x => x.MaxStakeAmount).GreaterThan(0);
        });
        When(x => x.MaxExtraWinnings.HasValue, () =>
        {
            RuleFor(x => x.MaxExtraWinnings).GreaterThan(0);
        });
    }
}

public class UniboostReloadRewardParameterValidator : AbstractValidator<UniBoostReloadParametersApiModel>
{
    public UniboostReloadRewardParameterValidator()
    {
        RuleFor(x => x.Type)
            .Equal(nameof(RewardType.UniboostReload));
        When(x => x.MaxStakeAmount.HasValue, () =>
        {
            RuleFor(x => x.MaxStakeAmount).GreaterThan(0);
        });
        RuleFor(x => x.Reload)
            .NotEmpty();
        When(x => x.MaxExtraWinnings.HasValue, () =>
        {
            RuleFor(x => x.MaxExtraWinnings).GreaterThan(0);
        });
    }
}

public class ProfitBoostRewardParameterValidator : AbstractValidator<ProfitBoostParametersApiModel>
{
    public ProfitBoostRewardParameterValidator()
    {
        RuleFor(x => x.Type)
            .Equal(nameof(RewardType.Profitboost));
        When(x => x.MaxStakeAmount.HasValue, () =>
        {
            RuleFor(x => x.MaxStakeAmount).GreaterThan(0);
        });
        RuleFor(x => x.LegTable)
            .NotEmpty();
        When(x => x.MaxExtraWinnings.HasValue, () =>
        {
            RuleFor(x => x.MaxExtraWinnings).GreaterThan(0);
        });
    }
}
