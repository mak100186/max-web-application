using Kindred.Infrastructure.Hosting.WebApi.Sorting;
using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Models.Rewards;

public class RewardFilterDomainModel : SortRequest
{
    public string Name { get; set; }

    public string CustomerId { get; set; }

    public DateTime? UpdatedDateFromUtc { get; set; }

    public DateTime? UpdatedDateToUtc { get; set; }

    public bool IncludeCancelled { get; set; } = true;

    public bool IncludeExpired { get; set; } = true;

    public bool IncludeActive { get; set; } = true;
    public RewardType? RewardType { get; set; }
    public string Jurisdiction { get; set; }
    public string Country { get; set; }
    public string Brand { get; set; }
    public string State { get; set; }
    protected override List<string> SortableFields => Enum.GetValues<SortableRewardFields>().Select(x => x.ToString()).ToList();

    protected override string DefaultSortBy => SortableRewardFields.StartDateTime.ToString();

}
