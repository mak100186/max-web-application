using Confluent.Kafka;

using Kindred.Infrastructure.Kafka.Handlers;
using Kindred.Offer.PriceManager.Event.Models;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Client;

using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.MessageConsumers.EventHandlers;

public class OddsLadderDeletedHandler : BaseMessageHandler<OddsLadderDeleted>
{
    private readonly ILogger<OddsLadderDeletedHandler> _logger;
    private readonly IOddsLadderClient _oddsLadderClient;

    public override string ProfileName { get; init; } = DomainConstants.TopicProfileOddsLadder;
    public OddsLadderDeletedHandler(ILogger<OddsLadderDeletedHandler> logger, IOddsLadderClient oddsLadderClient)
    {
        _logger = logger;
        _oddsLadderClient = oddsLadderClient;
    }
    public override void ProcessMessage(Message<string, OddsLadderDeleted> message)
    {
        var oddsLadderMessage = message.Value;

        _logger.LogInformation("{messageType} executed for contest type {contestType}", GetType().Name, oddsLadderMessage.ContestType);

        _oddsLadderClient.Clear(oddsLadderMessage.ContestType.ToString());
    }
}
