using FluentValidation;

using Kindred.Customer.WalletGateway.ExternalModels.Common;

namespace Kindred.Rewards.Plugin.MessageConsumers.Validators;

public class RewardClaimsValidator : AbstractValidator<RewardClaim>
{
    public RewardClaimsValidator()
    {
        RuleFor(claim => claim.ClaimRn).NotNull().NotEmpty();
    }
}
