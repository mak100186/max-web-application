using Newtonsoft.Json;

namespace Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim.Enums;

public enum SettlementBetStatus
{
    [JsonProperty("pending")]
    Pending,

    [JsonProperty("resolved")]
    Resolved,

    [JsonProperty("unresolved")]
    Unresolved,
}
