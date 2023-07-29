using System.Diagnostics.CodeAnalysis;

using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kindred.Rewards.Core.Infrastructure.Data.Configuration;

[ExcludeFromCodeCoverage]
public class RewardTemplateCustomerMap : BaseDataModelMap<RewardTemplateCustomer>
{
    public override void Configure(EntityTypeBuilder<RewardTemplateCustomer> builder)
    {
        base.Configure(builder);

        builder.ToTable("RewardTemplateCustomer");

        builder.Property(m => m.Id);

        builder.Property(m => m.CreatedOn);

        builder.Property(m => m.PromotionTemplateKey).IsRequired().HasMaxLength(50).IsUnicode(false);
        builder.Property(m => m.CustomerId).IsRequired().HasMaxLength(30).IsUnicode(false);

        builder.HasIndex(e => new { e.PromotionTemplateKey, e.CustomerId })
            .HasDatabaseName("IX_RewardTemplateCustomer_RewardTemplateKey");

        builder.HasIndex(e => new { e.CustomerId, e.PromotionTemplateKey })
            .HasDatabaseName("IX_RewardTemplateCustomer_CustomerId");
    }
}
