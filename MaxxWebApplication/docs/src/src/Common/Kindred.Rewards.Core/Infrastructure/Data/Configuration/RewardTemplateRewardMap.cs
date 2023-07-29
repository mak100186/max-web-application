using System.Diagnostics.CodeAnalysis;

using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kindred.Rewards.Core.Infrastructure.Data.Configuration;

[ExcludeFromCodeCoverage]
public class RewardTemplateRewardMap : IEntityTypeConfiguration<RewardTemplateReward>
{
    public void Configure(EntityTypeBuilder<RewardTemplateReward> builder)
    {
        builder.ToTable("RewardTemplateReward");

        builder.HasKey(e => new { PromotionTemplateKey = e.RewardTemplateId, e.RewardRn });

        builder.Property(e => e.RewardTemplateId).IsRequired().HasColumnName("RewardTemplateId");
        builder.Property(e => e.RewardRn).IsRequired().HasColumnName("RewardId");

        builder
            .HasOne(e => e.RewardTemplate)
            .WithMany(e => e.RewardTemplateReward)
            .HasForeignKey(e => e.RewardTemplateId);

        builder
            .HasOne(e => e.Reward)
            .WithMany(e => e.RewardTemplateReward)
            .HasForeignKey(e => e.RewardRn);

        builder.HasIndex(e => e.RewardTemplateId)
            .HasDatabaseName("IX_RewardTemplateReward_RewardTemplateKey");

        builder.HasIndex(e => e.RewardRn)
            .HasDatabaseName("IX_RewardTemplateReward_RewardRn");
    }
}
