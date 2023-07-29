using Confluent.Kafka;

using FluentValidation;

using Kindred.Customer.WalletGateway.ExternalModels.ChimeraModels;
using Kindred.Customer.WalletGateway.ExternalModels.ChimeraModels.Enums;
using Kindred.Infrastructure.Kafka.Handlers;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Infrastructure.Queue.Extensions;
using Kindred.Rewards.Plugin.MessageConsumers.Services;

using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.MessageConsumers.EventHandlers;

public class CouponDeclined : CouponMessage
{

}

public class CouponDeclinedMessageHandler : BaseMessageHandler<CouponDeclined>
{
    private readonly IClaimService _claimService;
    private readonly ILogger<CouponDeclinedMessageHandler> _logger;
    private readonly IValidator<CouponDeclined> _couponMessageValidator;

    public override string ProfileName { get; init; } = DomainConstants.TopicProfileBetUpdates;

    public CouponDeclinedMessageHandler(IValidator<CouponDeclined> validator, IClaimService claimService, ILogger<CouponDeclinedMessageHandler> logger)
    {
        _couponMessageValidator = validator;
        _claimService = claimService;
        _logger = logger;
    }
    public override void ProcessMessage(Message<string, CouponDeclined> message)
    {
        var couponMessage = message.Value;
        var validationOutcome = _couponMessageValidator.ValidateAsync(couponMessage).GetAwaiter().GetResult();
        var isValid = validationOutcome.IsValid; //validators are not inspection friendly
        if (isValid)
        {
            _logger.LogInformation("Received {@CouponMessage}", couponMessage);

            var correlationId = message.Headers.GetCorrelationId();
            var coupon = couponMessage.Resources.First();
            if (coupon.Status == CouponStatus.Declined)
            {
                _logger.LogInformation("Received CouponDeclinedMessageEvent {@coupon}", coupon);
                try
                {
                    var rewardClaims = coupon.Bets.SelectMany(b => b.RewardClaims).Select(rc => rc.ClaimRn).Distinct();
                    _claimService.ReActivateClaimAsync(rewardClaims, correlationId).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Reward claims could not be processed on CouponDeclined {@coupon}", coupon);
                }
            }
        }
        else
        {
            _logger.LogWarning("Invalid CouponMessage {@Errors}", validationOutcome.Errors);
        }
    }
}
