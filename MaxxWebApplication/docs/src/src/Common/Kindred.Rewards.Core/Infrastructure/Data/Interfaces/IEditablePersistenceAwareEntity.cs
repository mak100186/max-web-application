namespace Kindred.Rewards.Core.Infrastructure.Data.Interfaces;

public interface IEditablePersistenceAwareEntity : IPersistenceAwareEntity
{
    DateTime UpdatedOn { get; set; }
}
