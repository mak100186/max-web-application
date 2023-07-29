using Kindred.Rewards.Core.Mapping.Converters;

using Newtonsoft.Json;

namespace Kindred.Rewards.Rewards.FunctionalTests.Common.Helpers;
internal static class SerializerSettings
{
    public static JsonSerializerSettings GetJsonSerializerSettings() => new()
    {
        Converters = new List<Newtonsoft.Json.JsonConverter>
        {
            new RewardParameterApiModelBaseConverter()
        }
    };
}
