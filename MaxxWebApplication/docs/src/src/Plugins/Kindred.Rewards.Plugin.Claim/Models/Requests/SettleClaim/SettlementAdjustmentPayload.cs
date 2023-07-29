namespace Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim;

public class SettlementAdjustmentPayload
{
    public double ExpectedPayoff { get; set; }
    public double PrepaidAmount { get; set; }
    public string Note { get; set; }
}
