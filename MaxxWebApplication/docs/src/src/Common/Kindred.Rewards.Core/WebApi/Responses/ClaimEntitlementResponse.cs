using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Payloads.BetModel;

namespace Kindred.Rewards.Core.WebApi.Responses;

public class ClaimEntitlementResponse
{
    public string Rn { get; set; } //previously the instanceId, this is rewardClaimsRn
    public string Status { get; set; } //string rep of Domain.Enums.RewardClaimStatus
    public string RewardName { get; set; }
    public string CurrencyCode { get; set; }
    public RewardInfoApiModel Reward { get; set; } //Contains info about the reward
    public BetResponseApiModel Bet { get; set; } //Contains info about the bet that was placed with original odds and the boosted odds after claiming
}
