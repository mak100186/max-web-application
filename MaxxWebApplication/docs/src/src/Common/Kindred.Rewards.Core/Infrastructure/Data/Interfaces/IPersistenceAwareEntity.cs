namespace Kindred.Rewards.Core.Infrastructure.Data.Interfaces;

public interface IPersistenceAwareEntity
{
    DateTime CreatedOn { get; set; }
}
