using Kindred.Rewards.Core.Infrastructure.Data.Interfaces;

namespace Kindred.Rewards.Core.Infrastructure.Data.DataModels;

public abstract class BaseDataModel<TKey> : IPersistenceAwareEntity
{
    public virtual TKey Id { get; set; }

    public DateTime CreatedOn { get; set; }
}
