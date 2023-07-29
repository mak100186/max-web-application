using FluentValidation;

using Kindred.Customer.WalletGateway.ExternalModels.ChimeraModels;
using Kindred.Customer.WalletGateway.ExternalModels.Common.Enums;

namespace Kindred.Rewards.Plugin.MessageConsumers.Validators;

public class BetMessageEventValidator : AbstractValidator<BetMessage>
{
    public BetMessageEventValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(couponMessage => couponMessage.Actions).NotNull().NotEmpty();
        RuleFor(couponMessage => couponMessage.Resources).NotNull().NotEmpty();

        When(message => message.Resources.First().Status is BetStatus.Rejected, () =>
        {
            RuleFor(message => message)
                .Must(message =>
                {
                    var failingConditions = message.Resources.First().RewardClaims == null || !message.Resources.First().RewardClaims.Any() || message.Resources.First().RewardClaims.Any(rc => string.IsNullOrWhiteSpace(rc.ClaimRn));

                    return !failingConditions;
                })
                .WithMessage(message =>
                    $"Reward claims can not be null and must have InstanceId, Bet RN: {message.Resources.First().Rn}");
        });
    }
}
