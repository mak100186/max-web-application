namespace Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim;

public class CompoundStagePayload
{
    public string Market { get; set; }
    public OddsPayload Odds { get; set; }
    public SelectionPayload Selection { get; set; }
}
