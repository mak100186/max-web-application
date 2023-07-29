using FluentValidation;

using Kindred.Customer.WalletGateway.ExternalModels.ChimeraModels;
using Kindred.Customer.WalletGateway.ExternalModels.ChimeraModels.Enums;

namespace Kindred.Rewards.Plugin.MessageConsumers.Validators;

public class CouponMessageEventValidator : AbstractValidator<CouponMessage>
{
    public CouponMessageEventValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(couponMessage => couponMessage.Actions).NotNull().NotEmpty();
        RuleFor(couponMessage => couponMessage.Resources).NotNull().NotEmpty();
        RuleFor(message => message.Resources.First().Bets).NotNull().NotEmpty();

        When(message => message.Resources.First().Status is CouponStatus.Declined or CouponStatus.Failed, () =>
        {
            RuleFor(message => message)
                .Must(message =>
                {
                    var rewardClaims = message.Resources.First().Bets.Where(b => b.RewardClaims != null).SelectMany(x => x.RewardClaims).ToList();
                    return rewardClaims.Any() && !rewardClaims.Any(rc => string.IsNullOrWhiteSpace(rc.ClaimRn));
                })
                .WithMessage(message =>
                    $"Reward claims can not be null and must have InstanceId, Bet RN: {message.Resources.First().Bets.First().Rn}");
        });
    }
}
