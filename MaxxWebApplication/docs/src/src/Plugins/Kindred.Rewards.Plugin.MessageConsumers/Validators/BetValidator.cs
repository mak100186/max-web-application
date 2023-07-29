using FluentValidation;

using Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels;

namespace Kindred.Rewards.Plugin.MessageConsumers.Validators;

public class BetValidator : AbstractValidator<Bet>
{
    public BetValidator()
    {
        RuleFor(bet => bet.Settlement).NotNull();
        RuleFor(bet => bet.Rn).NotNull().NotEmpty();
        RuleForEach(bet => bet.RewardClaims).SetValidator(new RewardClaimsValidator());
    }
}
