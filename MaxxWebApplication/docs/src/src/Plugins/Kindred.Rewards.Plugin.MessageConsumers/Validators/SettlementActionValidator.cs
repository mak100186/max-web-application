using FluentValidation;

using Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels;

namespace Kindred.Rewards.Plugin.MessageConsumers.Validators;

public class SettlementActionValidator : AbstractValidator<SettlementAction>
{
    public SettlementActionValidator()
    {
        RuleFor(bet => bet.PreviousBetSettlement).NotNull();
    }
}
