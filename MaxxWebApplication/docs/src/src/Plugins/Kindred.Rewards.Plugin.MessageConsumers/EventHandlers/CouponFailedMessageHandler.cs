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

public class CouponFailed : CouponMessage
{

}
public class CouponFailedMessageHandler : BaseMessageHandler<CouponFailed>
{
    private readonly IClaimService _claimService;
    private readonly ILogger<CouponFailedMessageHandler> _logger;
    private readonly IValidator<CouponFailed> _couponMessageValidator;

    public override string ProfileName { get; init; } = DomainConstants.TopicProfileBetUpdates;

    public CouponFailedMessageHandler(IValidator<CouponFailed> validator, IClaimService claimService, ILogger<CouponFailedMessageHandler> logger)
    {
        _couponMessageValidator = validator;
        _claimService = claimService;
        _logger = logger;
    }
    public override void ProcessMessage(Message<string, CouponFailed> message)
    {
        var couponMessage = message.Value;
        var validationOutcome = _couponMessageValidator.ValidateAsync(couponMessage).GetAwaiter().GetResult();
        var isValid = validationOutcome.IsValid; //validators are not inspection friendly
        if (isValid)
        {
            _logger.LogInformation("Received {@CouponMessage}", couponMessage);

            var correlationId = message.Headers.GetCorrelationId();
            var coupon = couponMessage.Resources.First();

            if (coupon.Status == CouponStatus.Failed)
            {
                _logger.LogInformation("Received CouponFailedMessageEvent {@coupon}", coupon);
                try
                {
                    var rewardClaims = coupon.Bets.SelectMany(b => b.RewardClaims).Select(rc => rc.ClaimRn).Distinct();
                    _claimService.ReActivateClaimAsync(rewardClaims, correlationId).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Reward claims could not be processed on CouponFailed {@coupon}", coupon);
                    throw;
                }
            }
        }
        else
        {
            _logger.LogWarning("Invalid CouponMessage {@Errors}", validationOutcome.Errors);
        }
    }
}
