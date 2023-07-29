using Unibet.Infrastructure.Messaging.KafkaFlow.Model;

namespace Kindred.Rewards.Core.Models.Messages;

public interface IMessageInstance : IKspMessage
{
    string InstanceId { get; set; }
}
