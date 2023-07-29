using Kindred.Rewards.Core.Infrastructure.Data.DataModels;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kindred.Rewards.Core.Infrastructure.Data.Configuration;

public abstract class BaseEditableDataModelMap<T> : BaseDataModelMap<T> where T : BaseEditableDataModel<int>
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);
        builder.Property(m => m.UpdatedOn).IsRequired().HasAnnotation("Precision", 6);
    }
}
