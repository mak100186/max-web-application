using Kindred.Infrastructure.Hosting.WebApi.Sorting;
using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Models.RewardClaims;

public class RewardClaimFilterDomainModel : SortRequest
{
    public string InstanceId { get; set; }

    public string RewardRn { get; set; }
    public string RewardId { get; set; }

    public string RewardName { get; set; }

    public string CustomerId { get; set; }

    public string CouponRn { get; set; }

    public string BetRn { get; set; }

    public string RewardType { get; set; }

    public string ClaimStatus { get; set; }

    public string BetOutcomeStatus { get; set; }

    public DateTime? UpdatedDateFromUtc { get; set; }

    public DateTime? UpdatedDateToUtc { get; set; }

    protected override List<string> SortableFields => Enum.GetValues<SortableRewardClaimFields>().Select(x => x.ToString()).ToList();

    protected override string DefaultSortBy => SortableRewardClaimFields.UpdatedOn.ToString();

}
