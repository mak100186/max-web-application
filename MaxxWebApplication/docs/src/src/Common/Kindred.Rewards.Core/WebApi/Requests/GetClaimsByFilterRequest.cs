using Kindred.Infrastructure.Hosting.WebApi.Sorting;
using Kindred.Rewards.Core.WebApi.Enums;

namespace Kindred.Rewards.Core.WebApi.Requests;

public class GetClaimsByFilterRequest : SortRequest
{
    public string InstanceId { get; set; }

    public string RewardRn { get; set; }
    public string RewardId { get; set; }

    public string RewardName { get; set; }

    public string CustomerId { get; set; }

    public string CouponRef { get; set; }

    public string BetRef { get; set; }

    public string RewardType { get; set; }

    public string ClaimStatus { get; set; }

    public string BetOutcomeStatus { get; set; }

    public DateTimeOffset? UpdatedDateFromUtc { get; set; }

    public DateTimeOffset? UpdatedDateToUtc { get; set; }

    public bool? IsDescending { get; set; }
    protected override List<string> SortableFields => Enum.GetValues<SortableRewardClaimFields>().Select(x => x.ToString()).ToList();
    protected override string DefaultSortBy => SortableRewardClaimFields.UpdatedOn.ToString();

}
