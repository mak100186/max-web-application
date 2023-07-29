using System.Diagnostics.CodeAnalysis;

using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kindred.Rewards.Core.Infrastructure.Data.Configuration;

[ExcludeFromCodeCoverage]
public class AuditRewardMap : BaseDataModelMap<AuditReward>
{
    public override void Configure(EntityTypeBuilder<AuditReward> builder)
    {
        base.Configure(builder);

        builder.ToTable("AuditReward");

        builder.Property(m => m.Id);

        builder.Property(m => m.CreatedOn).HasAnnotation("Precision", 6);
        builder.Property(m => m.RewardId).IsRequired().HasMaxLength(128).IsUnicode();
        builder.Property(m => m.Activity).IsRequired().HasMaxLength(30).IsUnicode(false);
        builder.Property(m => m.CreatedBy).HasMaxLength(100);

        builder.HasIndex(e => e.RewardId).HasDatabaseName("IX_AuditReward_RewardId");
    }
}
