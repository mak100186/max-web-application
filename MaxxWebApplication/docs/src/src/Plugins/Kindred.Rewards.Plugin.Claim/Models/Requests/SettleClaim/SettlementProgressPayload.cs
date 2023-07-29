namespace Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim;

public class SettlementProgressPayload
{
    public int Combinations { get; set; }
    public int Resolved { get; set; }
    public int Pending { get; set; }
    public int Unresolved { get; set; }
}
