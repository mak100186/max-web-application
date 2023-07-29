using Kindred.Infrastructure.Hosting.WebApi.Sorting;
using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Models.Promotions;

public class PromotionTemplateFilterDomainModel : SortRequest
{
    public string TemplateKey { get; set; }

    public bool IncludeDisabled { get; set; }
    protected override List<string> SortableFields { get; } = Enum.GetValues<SortableRewardTemplateFields>().Select(x => x.ToString()).ToList();
    protected override string DefaultSortBy { get; } = SortableRewardTemplateFields.Enabled.ToString(); //TODO For now default sort to this (currently don't expose it)
}
