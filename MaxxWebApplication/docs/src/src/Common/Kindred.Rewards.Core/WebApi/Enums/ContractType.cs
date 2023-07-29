using Newtonsoft.Json;

namespace Kindred.Rewards.Core.WebApi.Enums;

public enum ContractType
{
    [JsonProperty("WebApi")]
    WebApi,
    [JsonProperty("KafkaMessage")]
    KafkaMessage
}
