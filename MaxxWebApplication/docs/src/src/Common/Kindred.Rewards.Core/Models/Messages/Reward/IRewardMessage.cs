using Unibet.Infrastructure.Messaging.KafkaFlow.Model;

namespace Kindred.Rewards.Core.Models.Messages.Reward;

public interface IRewardMessage : IKspMessage
{
    string RewardRn { get; set; }
}
