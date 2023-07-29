using Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim;
using Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim.Enums;

namespace Kindred.Rewards.Plugin.Claim.Models.Requests;

public class SettleClaimRequest
{
    public string Rn { get; set; }
    public decimal Stake { get; set; }
    public Formula Formula { get; set; }
    public IEnumerable<CompoundStagePayload> Stages { get; set; }
    public IEnumerable<CombinationPayload> AcceptedCombinations { get; set; }
    public string CustomerId { get; set; }
    public BetSettlementPayload Settlement { get; set; }
    public IEnumerable<RewardClaimPayload> RewardClaims { get; set; }
}
