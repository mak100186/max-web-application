using Kindred.Rewards.Plugin.Claim.Models.Responses.SettleClaim;

namespace Kindred.Rewards.Plugin.Claim.Models.Responses;

public class SettleClaimResponse
{
    public List<ClaimSettlementPayload> RewardClaimSettlement { get; init; } = new();
    public List<ClaimSettlementPayload> PrevRewardClaimSettlement { get; init; } = new();
}
