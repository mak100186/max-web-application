using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kindred.Rewards.Core.Infrastructure.Data.Configuration;

public class CustomerRewardMap : IEntityTypeConfiguration<CustomerReward>
{
    public void Configure(EntityTypeBuilder<CustomerReward> builder)
    {
        builder.ToTable("CustomerRewards");
        builder.HasKey(m => new { m.CustomerId, m.RewardId });
        builder.Property(m => m.CustomerId);
        builder.Property(m => m.RewardId);
        builder.Property(m => m.CreatedOn);

        builder.HasOne(r => r.Reward)
                .WithMany(r => r.CustomerRewards)
                .HasForeignKey(r => r.RewardId)
                .HasConstraintName("FK_CustomerReward_Reward");

        builder.HasIndex(e => e.CustomerId)
            .HasDatabaseName("IX_CustomerRewards_CustomerId");

        builder.HasIndex(e => e.RewardId)
            .HasDatabaseName("IX_CustomerRewards_RewardId");
    }
}
