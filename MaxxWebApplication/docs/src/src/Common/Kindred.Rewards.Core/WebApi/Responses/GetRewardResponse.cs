using Kindred.Rewards.Core.WebApi.Payloads;

namespace Kindred.Rewards.Core.WebApi.Responses;
public class GetRewardResponse : RewardEntitlementApiModel
{
    public string Status { get; set; }
    public ICollection<string> PromotionRns { get; set; }
}
