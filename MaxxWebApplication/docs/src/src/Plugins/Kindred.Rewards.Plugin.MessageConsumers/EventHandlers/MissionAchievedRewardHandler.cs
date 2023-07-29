using Confluent.Kafka;

using Kindred.Infrastructure.Kafka.Handlers;
using Kindred.Rewards.Core;
using Kindred.Rewards.Plugin.MessageConsumers.Exceptions;
using Kindred.Rewards.Plugin.MessageConsumers.Services;

using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.MessageConsumers.EventHandlers;

public class MissionAchievedRewardMessage
{
    public string CustomerId { get; set; }
    public string RewardId { get; set; }
}


public class MissionAchievedRewardHandler : BaseMessageHandler<MissionAchievedRewardMessage>
{
    private readonly ICustomerRewardService _customerRewardService;
    private readonly ILogger<MissionAchievedRewardHandler> _logger;

    public override string ProfileName { get; init; } = DomainConstants.TopicProfileMissionsAchieved;

    public MissionAchievedRewardHandler(ICustomerRewardService customerRewardService, ILogger<MissionAchievedRewardHandler> logger)
    {
        _customerRewardService = customerRewardService;
        _logger = logger;
    }

    public override void ProcessMessage(Message<string, MissionAchievedRewardMessage> message)
    {
        var missionAchievedRewardMessage = message.Value;

        _logger.LogInformation("Received MissionAchievedReward message for customer {customerId} with reward {rewardId}", missionAchievedRewardMessage.CustomerId, missionAchievedRewardMessage.RewardId);

        var reward = _customerRewardService.GetRewardAsync(missionAchievedRewardMessage.RewardId).GetAwaiter().GetResult();
        if (reward.CustomerId != null || !reward.IsSystemGenerated)
        {
            _logger.LogWarning("Invalid reward with id {id}, when processing MissionsRewardAchieved for customer {customerId}", missionAchievedRewardMessage.RewardId, missionAchievedRewardMessage.CustomerId);
            throw new MissionsRewardException($"Invalid reward with id {missionAchievedRewardMessage.RewardId} for customer {missionAchievedRewardMessage.CustomerId}");
        }

        _customerRewardService.AddCustomerRewardAsync(missionAchievedRewardMessage.CustomerId, missionAchievedRewardMessage.RewardId).GetAwaiter().GetResult();
    }
}
