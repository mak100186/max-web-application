using Newtonsoft.Json;

namespace Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim.Enums;

public enum SettlementSegmentStatus
{
    [JsonProperty("lost")]
    Lost,

    [JsonProperty("part_refunded")]
    PartRefunded,

    [JsonProperty("part_won")]
    PartWon,

    [JsonProperty("pending")]
    Pending,

    [JsonProperty("refunded")]
    Refunded,

    [JsonProperty("unresolved")]
    Unresolved,

    [JsonProperty("won")]
    Won
}
