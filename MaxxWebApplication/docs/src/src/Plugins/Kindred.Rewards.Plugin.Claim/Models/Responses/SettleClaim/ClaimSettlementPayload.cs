namespace Kindred.Rewards.Plugin.Claim.Models.Responses.SettleClaim;

public class ClaimSettlementPayload
{
    public decimal? Payoff { get; init; }
    public RewardPayoffPayload? Reward { get; set; }
}
