using Kindred.Rewards.Core.Infrastructure.Data.Interfaces;

namespace Kindred.Rewards.Core.Infrastructure.Data.DataModels;

public class CustomerReward : IPersistenceAwareEntity
{
    public string CustomerId { get; set; }
    public string RewardId { get; set; }
    public DateTime CreatedOn { get; set; }

    public virtual Reward Reward { get; set; }
}
