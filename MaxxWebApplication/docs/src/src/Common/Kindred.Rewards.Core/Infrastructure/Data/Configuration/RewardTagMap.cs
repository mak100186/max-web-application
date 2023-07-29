using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kindred.Rewards.Core.Infrastructure.Data.Configuration;
public class RewardTagMap : IEntityTypeConfiguration<RewardTag>
{
    public void Configure(EntityTypeBuilder<RewardTag> builder)
    {
        builder.ToTable("RewardTags");

        builder.HasKey(e => new { RewardTagKey = e.RewardId, e.TagId });

        builder.Property(e => e.RewardId).IsRequired().HasColumnName("RewardId");
        builder.Property(e => e.TagId).IsRequired().HasColumnName("TagId");

        builder
            .HasOne(e => e.Reward)
            .WithMany(e => e.RewardTags)
            .HasForeignKey(e => e.RewardId);

        builder
            .HasOne(e => e.Tag)
            .WithMany(e => e.RewardTags)
            .HasForeignKey(e => e.TagId);

        builder.HasIndex(e => e.RewardId)
            .HasDatabaseName("IX_RewardTags_RewardId");

        builder.HasIndex(e => e.TagId)
            .HasDatabaseName("IX_RewardTags_TagId");
    }
}
