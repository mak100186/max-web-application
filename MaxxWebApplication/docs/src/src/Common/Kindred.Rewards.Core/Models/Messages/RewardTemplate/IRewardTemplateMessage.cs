using Unibet.Infrastructure.Messaging.KafkaFlow.Model;

namespace Kindred.Rewards.Core.Models.Messages.RewardTemplate;

public interface IRewardTemplateMessage : IKspMessage
{
    public string TemplateKey { get; set; }
}
