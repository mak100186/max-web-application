using Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim.Enums;

namespace Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim;

public class BetSettlementPayload
{
    public SettlementBetStatus Status { get; set; }
    public decimal FinalPayoff { get; set; }
}
