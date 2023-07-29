using System.Diagnostics.CodeAnalysis;

using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kindred.Rewards.Core.Infrastructure.Data.Configuration;

[ExcludeFromCodeCoverage]
public sealed class RewardClaimMap : BaseEditableDataModelMap<RewardClaim>
{
    public override void Configure(EntityTypeBuilder<RewardClaim> builder)
    {
        base.Configure(builder);

        builder.ToTable("RewardClaim");

        builder.Property(m => m.Id);

        builder.Property(m => m.CreatedOn);
        builder.Property(m => m.UpdatedOn);

        builder.Property(m => m.RewardId).IsRequired().HasMaxLength(50);
        builder.Property(m => m.BetOutcomeStatus).IsRequired(false).HasMaxLength(50);
        builder.Property(m => m.CustomerId).IsRequired().HasMaxLength(30).IsUnicode(false);
        builder.Property(m => m.CouponRn).IsRequired().HasMaxLength(255).IsUnicode(false);
        builder.Property(m => m.BetRn).IsRequired();
        builder.Property(m => m.Status).IsRequired().HasMaxLength(20).IsUnicode(false);
        builder.Property(m => m.TermsJson).IsRequired().HasMaxLength(5000).IsUnicode(false);
        builder.Property(m => m.IntervalId).IsRequired();
        builder.Property(m => m.UsageId).IsRequired();
        builder.Property(m => m.InstanceId).IsRequired().HasMaxLength(50).IsUnicode(false);
        builder.Property(m => m.RewardPayoutAmount).IsRequired(false).HasColumnType("NUMERIC(16,2)");
        builder.Property(m => m.RewardName).IsRequired().HasMaxLength(2000);
        builder.Property(m => m.RewardType).IsRequired().HasMaxLength(30).IsUnicode(false);
        builder.Property(m => m.CurrencyCode).IsRequired(false).HasMaxLength(3).IsUnicode(false);
        builder.Property(m => m.CreatedBy).HasMaxLength(100);

        // index for concurrency check on creation of new RewardClaim
        builder.HasIndex(e => new { RewardRn = e.RewardId, e.CustomerId, e.IntervalId, e.UsageId }).IsUnique()
            .HasDatabaseName("IX_RewardClaim_Usage");

        builder.Property(m => m.PayoffMetadata).HasColumnType("jsonb");

        builder.HasIndex(e => e.InstanceId).HasDatabaseName("IX_RewardClaim_InstanceId");

        builder
            .HasMany(rewardClaim => rewardClaim.CombinationRewardPayoffs)
            .WithOne(combinationRewardPayoff => combinationRewardPayoff.RewardClaim)
            .HasForeignKey(combinationRewardPayoff => combinationRewardPayoff.ClaimInstanceId)
            .HasPrincipalKey(rewardClaim => rewardClaim.InstanceId);
    }
}
