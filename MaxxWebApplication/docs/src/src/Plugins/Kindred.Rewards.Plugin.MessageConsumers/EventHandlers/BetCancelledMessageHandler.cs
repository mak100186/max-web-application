using Confluent.Kafka;

using FluentValidation;

using Kindred.Customer.WalletGateway.ExternalModels.ChimeraModels;
using Kindred.Customer.WalletGateway.ExternalModels.Common.Enums;
using Kindred.Infrastructure.Kafka.Handlers;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Infrastructure.Queue.Extensions;
using Kindred.Rewards.Plugin.MessageConsumers.Services;

using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.MessageConsumers.EventHandlers;


public class BetCancelled : BetMessage
{

}

public class BetCancelledMessageHandler : BaseMessageHandler<BetCancelled>
{
    private readonly IValidator<BetCancelled> _betMessageValidator;
    private readonly IClaimService _claimService;
    private readonly ILogger<BetCancelledMessageHandler> _logger;

    public override string ProfileName { get; init; } = DomainConstants.TopicProfileBetUpdates;

    public BetCancelledMessageHandler(IValidator<BetCancelled> betMessageValidator, IClaimService claimService, ILogger<BetCancelledMessageHandler> logger)
    {
        _betMessageValidator = betMessageValidator;
        _claimService = claimService;
        _logger = logger;
    }

    public override void ProcessMessage(Message<string, BetCancelled> message)
    {
        var betMessage = message.Value;
        var correlationId = message.Headers.GetCorrelationId();
        var result = _betMessageValidator.ValidateAsync(betMessage).GetAwaiter().GetResult();

        if (!result.IsValid)
        {
            _logger.LogWarning("Invalid BetMessage {@Errors}", result.Errors);
            return;
        }

        _logger.LogInformation("Received {@BetMessage}", betMessage);

        var bet = betMessage.Resources.First();

        if (bet.Status is BetStatus.Cancelled)
        {
            try
            {
                var rewardClaims = bet.RewardClaims.Select(rc => rc.ClaimRn).Distinct();
                _claimService.ReActivateClaimAsync(rewardClaims, correlationId).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reward claims could not be processed on BetCancelled {@bet}", bet);
                throw;
            }
        }
    }
}
