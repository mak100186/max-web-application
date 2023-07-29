using FluentValidation;

using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.WebApi.Payloads.BetModel;
using Kindred.Rewards.Core.WebApi.Validation.Enums;

namespace Kindred.Rewards.Core.WebApi.Validation;

public class BetStageValidator : ValidatorBase<CompoundStageApiModel>
{
    public BetStageValidator()
    {
        RuleFor(x => x.RequestedSelection).NotEmpty().WithMessage(GenerateErrorMessage(ClaimErrorCodes.StageSelectionIsRequired));
        RuleFor(x => x.RequestedSelection.Outcome).NotEmpty().WithMessage(GenerateErrorMessage(ClaimErrorCodes.StageSelectionIsRequired));
        RuleFor(x => x.Market).NotEmpty().DependentRules(() =>
        {
            RuleFor(x => x.Market.GetContestKeyFromMarket()).NotEmpty().WithMessage(GenerateErrorMessage(ClaimErrorCodes.ContestKeyIsRequired));
            RuleFor(x => x.Market.GetContestTypeFromMarket()).NotEmpty().WithMessage(GenerateErrorMessage(ClaimErrorCodes.ContestTypeIsRequired));
        });
        RuleFor(x => x.Odds).NotEmpty().SetValidator(new OddsValidator());
    }
}
