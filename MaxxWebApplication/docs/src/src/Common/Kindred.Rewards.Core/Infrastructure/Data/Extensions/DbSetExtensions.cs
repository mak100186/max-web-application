using System.Linq.Expressions;

using Kindred.Rewards.Core.Infrastructure.Data.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Kindred.Rewards.Core.Infrastructure.Data.Extensions;

public static class DbSetExtensions
{
    public static void Remove<T>(this DbSet<T> repo, Expression<Func<T, bool>> condition) where T : class, IPersistenceAwareEntity
    {
        var records = repo.Where(condition);
        foreach (var record in records)
        {
            repo.Remove(record);
        }
    }
}
