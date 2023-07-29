using System.Reflection;

using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Infrastructure.Data.Configuration;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Kindred.Rewards.Rewards.UnitTests.Common;

internal class RewardsDbContextMock : RewardsDbContext
{
    public RewardsDbContextMock(DbContextOptions<RewardsDbContextMock> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(RewardTemplateMap)));

        modelBuilder.Entity<RewardClaim>().Property(x => x.PayoffMetadata).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<RewardClaimPayoffMetadata>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
    }
}
