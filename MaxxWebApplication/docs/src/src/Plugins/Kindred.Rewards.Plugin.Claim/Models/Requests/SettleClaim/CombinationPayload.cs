namespace Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim;

public class CombinationPayload
{
    public string Rn { get; set; }
    public IEnumerable<SelectionPayload> Selections { get; set; }
    public CombinationSettlementPayload Settlement { get; set; }
}
