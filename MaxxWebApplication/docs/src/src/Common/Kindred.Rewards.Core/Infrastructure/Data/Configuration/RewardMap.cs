using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kindred.Rewards.Core.Infrastructure.Data.Configuration;

public sealed class RewardMap : IEntityTypeConfiguration<Reward>
{
    public void Configure(EntityTypeBuilder<Reward> builder)
    {
        builder.ToTable("Reward");

        builder.Property(m => m.Id).HasMaxLength(128).IsUnicode();
        builder.HasKey(m => m.Id);

        builder.Property(m => m.RewardType).IsRequired().HasMaxLength(30).IsUnicode(false);
        builder.Property(m => m.CustomerId).HasMaxLength(30).IsUnicode(false);
        builder.Property(m => m.Purpose).HasMaxLength(30).IsUnicode(false);
        builder.Property(m => m.SubPurpose).HasMaxLength(50).IsUnicode(false);
        builder.Property(m => m.Name).HasMaxLength(100);
        builder.Property(m => m.CountryCode).HasMaxLength(200);
        builder.Property(m => m.Comments).HasMaxLength(300).IsUnicode();
        builder.Property(m => m.RewardRules).HasMaxLength(2000);
        builder.Property(m => m.ContestStatus).HasMaxLength(10).IsUnicode(false);
        builder.Property(m => m.CreatedBy).HasMaxLength(100);
        builder.Property(m => m.UpdatedBy).HasMaxLength(100);
        builder.Property(m => m.StartDateTime);
        builder.Property(m => m.ExpiryDateTime);
        builder.Property(m => m.TermsJson).IsRequired().HasMaxLength(5000);
        builder.Property(m => m.CountryCode).HasMaxLength(3).IsUnicode(false);
        builder.Property(m => m.Jurisdiction).HasMaxLength(30);
        builder.Property(m => m.State).HasMaxLength(50);
        builder.Property(m => m.Brand).HasMaxLength(30);
        builder.Property(m => m.CustomerFacingName).HasMaxLength(200);
        builder.Property(m => m.IsCancelled).IsRequired();
        builder.Property(m => m.CancellationReason).HasMaxLength(50);
        builder.Property(m => m.SourceInstanceId).HasMaxLength(50).IsUnicode(false);
        builder.Property(m => m.IsSystemGenerated).HasDefaultValue(false);
        builder.Property(m => m.IsLocked).HasDefaultValue(false);

        builder.Property(m => m.CreatedOn);
        builder.Property(m => m.UpdatedOn);

        builder.HasMany(m => m.Audits);

        builder.HasIndex(m => new { m.CustomerId, m.ExpiryDateTime }).HasDatabaseName("IX_Reward_CustomerExpiry");
        builder.HasIndex(m => new { m.CustomerId, m.Name }).HasDatabaseName("IX_Reward_CustomerName");
    }
}
