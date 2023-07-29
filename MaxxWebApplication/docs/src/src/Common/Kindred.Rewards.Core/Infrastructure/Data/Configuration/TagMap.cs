using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kindred.Rewards.Core.Infrastructure.Data.Configuration;

public class TagMap : BaseEditableDataModelMap<Tag>
{
    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        base.Configure(builder);

        builder.ToTable("Tag");

        builder.Property(m => m.Id).HasMaxLength(128).IsUnicode();
        builder.HasKey(m => m.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(100).IsUnicode(false);

        // optional (nullable) fields
        builder.Property(p => p.Comments).IsUnicode(false).HasMaxLength(2000);
        builder.Property(p => p.DisplayStyle).IsRequired(false).HasMaxLength(100).IsUnicode(false);

        builder.Property(m => m.CreatedOn);
        builder.Property(m => m.UpdatedOn);

        builder.HasIndex(p => p.Name).IsUnique().HasDatabaseName("IX_Tag_Name");
    }
}
