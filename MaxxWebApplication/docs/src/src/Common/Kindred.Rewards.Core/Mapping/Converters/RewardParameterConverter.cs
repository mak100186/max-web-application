using System.Diagnostics.CodeAnalysis;

using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.WebApi.Enums;
using Kindred.Rewards.Core.WebApi.Payloads;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kindred.Rewards.Core.Mapping.Converters;

public class RewardParameterApiModelBaseConverter : JsonConverter<RewardParameterApiModelBase>
{
    public override bool CanWrite => false;

    private const string ConversionError = "'Type' provided for RewardParameters is not supported. Supported values are: " +
        $"{nameof(RewardType.Freebet)}, " + $"{nameof(RewardType.Uniboost)}, " +
        $"{nameof(RewardType.UniboostReload)}, " + $"{nameof(RewardType.Profitboost)}. Case insensitive.";

    public override RewardParameterApiModelBase ReadJson(JsonReader reader, Type objectType,
        RewardParameterApiModelBase existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);
        var typeDiscriminator = jObject.GetValue("Type", StringComparison.InvariantCultureIgnoreCase)?.Value<string>()?.ToLowerInvariant();

        if (!Enum.TryParse(typeDiscriminator, true, out RewardType type))
        {
            throw new RewardsValidationException(ConversionError);
        }

        RewardParameterApiModelBase target = type switch
        {
            RewardType.Freebet => new FreeBetParametersApiModel(),
            RewardType.Uniboost => new UniBoostParametersApiModel(),
            RewardType.UniboostReload => new UniBoostReloadParametersApiModel(),
            RewardType.Profitboost => new ProfitBoostParametersApiModel(),
            _ => throw new RewardsValidationException(ConversionError)
        };

        serializer.Populate(jObject.CreateReader(), target);

        return target;
    }

    public override void WriteJson(JsonWriter writer, [AllowNull] RewardParameterApiModelBase value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
