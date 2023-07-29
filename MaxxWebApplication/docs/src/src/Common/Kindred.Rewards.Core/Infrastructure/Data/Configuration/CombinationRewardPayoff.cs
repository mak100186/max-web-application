using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kindred.Rewards.Core.Infrastructure.Data.Configuration;

[ExcludeFromCodeCoverage]
public sealed class CombinationRewardPayoff : BaseEditableDataModelMap<DataModels.CombinationRewardPayoff>
{
    public override void Configure(EntityTypeBuilder<DataModels.CombinationRewardPayoff> builder)
    {
        base.Configure(builder);

        builder.ToTable("CombinationRewardPayoff");

        builder.Property(m => m.BetRn).IsRequired();
        builder.Property(m => m.CombinationRn).IsRequired();
    }
}
