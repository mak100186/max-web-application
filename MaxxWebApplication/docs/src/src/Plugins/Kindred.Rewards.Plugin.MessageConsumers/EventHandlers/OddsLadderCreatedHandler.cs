using Confluent.Kafka;

using Kindred.Infrastructure.Kafka.Handlers;
using Kindred.Offer.PriceManager.Event.Models;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Client;
using Kindred.Rewards.Core.Extensions;

using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.MessageConsumers.EventHandlers;

public class OddsLadderCreatedHandler : BaseMessageHandler<OddsLadderCreated>
{
    private readonly ILogger<OddsLadderCreatedHandler> _logger;
    private readonly IOddsLadderClient _oddsLadderClient;

    public override string ProfileName { get; init; } = DomainConstants.TopicProfileOddsLadder;

    public OddsLadderCreatedHandler(ILogger<OddsLadderCreatedHandler> logger, IOddsLadderClient oddsLadderClient)
    {
        _logger = logger;
        _oddsLadderClient = oddsLadderClient;
    }

    public override void ProcessMessage(Message<string, OddsLadderCreated> message)
    {
        var oddsLadderMessage = message.Value;

        _logger.LogInformation("{messageType} executed for contest type {contestType}", GetType().Name, oddsLadderMessage.ContestType);

        _oddsLadderClient.AddOrUpdate(oddsLadderMessage.ContestType.ToString(), oddsLadderMessage.ToOddsLadder());
    }
}
