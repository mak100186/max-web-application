using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Infrastructure.Data.Interfaces;
using Kindred.Rewards.Core.Infrastructure.Data.Seeding;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kindred.Rewards.Core.Infrastructure.Data;

public class RewardsDbContext : DbContext
{
    public virtual DbSet<AuditReward> AuditRewards { get; set; }
    public virtual DbSet<CombinationRewardPayoff> CombinationRewardPayoffs { get; set; }
    public virtual DbSet<CustomerReward> CustomerRewards { get; set; }
    public virtual DbSet<Reward> Rewards { get; set; }
    public virtual DbSet<RewardClaim> RewardClaims { get; set; }
    public virtual DbSet<RewardTemplate> RewardTemplates { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<RewardTemplateCustomer> RewardTemplateCustomers { get; set; }
    public virtual DbSet<RewardTemplateReward> RewardTemplateRewards { get; set; }

    private static DbContextOptions GetDbContextOptionsForMigrations()
    {
        var configs = new ConfigurationBuilder()
            .AddJsonFile("appsettings.repository.json")
            .Build();

        return
            new DbContextOptionsBuilder<RewardsDbContext>()
                .UseNpgsql(configs.GetConnectionString(DomainConstants.ConnectionString))
        .Options;
    }

    public RewardsDbContext()
        : base(GetDbContextOptionsForMigrations()) { }

    public RewardsDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RewardsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);

        new RewardTemplateSeedData(modelBuilder).Seed();
    }

    public override int SaveChanges()
    {
        PopulateInsertUpdateDate();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        PopulateInsertUpdateDate();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void PopulateInsertUpdateDate()
    {
        var addedEntities = ChangeTracker
                            .Entries<IPersistenceAwareEntity>()
                            .Where(p => p.State == EntityState.Added)
                            .Select(p => p.Entity);

        var modifiedEntities = ChangeTracker
                                    .Entries<IPersistenceAwareEntity>()
                                    .Where(p => p.State == EntityState.Modified)
                                    .Select(p => p.Entity);

        var now = DateTime.UtcNow;
        foreach (var added in addedEntities)
        {
            added.CreatedOn = now;
            if (added is IEditablePersistenceAwareEntity editableEntity)
            {
                editableEntity.UpdatedOn = now;
            }
        }
        foreach (var modified in modifiedEntities)
        {
            // check if the CreatedOn property is mapped/registered to DbContext
            if (Entry(modified).OriginalValues.Properties.Any(p => p.Name.Equals(nameof(modified.CreatedOn), StringComparison.InvariantCultureIgnoreCase)))
            {
                Entry(modified).Property(x => x.CreatedOn).IsModified = false;
            }
            if (modified is IEditablePersistenceAwareEntity editableEntity)
            {
                editableEntity.UpdatedOn = now;
            }
        }
    }
}
