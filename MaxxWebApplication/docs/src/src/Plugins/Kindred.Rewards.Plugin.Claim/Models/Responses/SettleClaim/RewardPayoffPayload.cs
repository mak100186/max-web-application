using Kindred.Rewards.Core.WebApi.Enums;

namespace Kindred.Rewards.Plugin.Claim.Models.Responses.SettleClaim;

public class RewardPayoffPayload
{
    public string Rn { get; set; }
    public RewardType RewardType { get; set; }
    public Dictionary<string, string> Params { get; set; }
}
