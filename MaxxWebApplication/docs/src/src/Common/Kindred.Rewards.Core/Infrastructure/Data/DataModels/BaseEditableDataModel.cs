using Kindred.Rewards.Core.Infrastructure.Data.Interfaces;

namespace Kindred.Rewards.Core.Infrastructure.Data.DataModels;

public abstract class BaseEditableDataModel<TKey> : BaseDataModel<TKey>, IEditablePersistenceAwareEntity
{
    public DateTime UpdatedOn { get; set; }
}
