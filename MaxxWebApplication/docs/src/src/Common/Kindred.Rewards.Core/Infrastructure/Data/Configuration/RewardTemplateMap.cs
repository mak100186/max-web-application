using System.Diagnostics.CodeAnalysis;

using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kindred.Rewards.Core.Infrastructure.Data.Configuration;

[ExcludeFromCodeCoverage]
public sealed class RewardTemplateMap : BaseEditableDataModelMap<RewardTemplate>
{
    public override void Configure(EntityTypeBuilder<RewardTemplate> builder)
    {
        base.Configure(builder);

        builder.ToTable("RewardTemplate");

        builder.Property(m => m.Id);

        builder.Property(m => m.CreatedOn);
        builder.Property(m => m.UpdatedOn);

        builder.Property(m => m.Key).IsRequired().HasMaxLength(50).IsUnicode(false);

        builder.Property(m => m.Comments).HasMaxLength(300).IsUnicode();

        builder.Property(m => m.Title).IsRequired().HasMaxLength(200).IsUnicode();

        builder.Property(m => m.Enabled).IsRequired();

        builder.HasIndex(e => e.Key).IsUnique().HasDatabaseName("IX_RewardTemplate_Key");
    }
}
