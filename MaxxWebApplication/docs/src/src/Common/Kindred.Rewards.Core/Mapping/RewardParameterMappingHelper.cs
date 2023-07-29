using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Helpers;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.WebApi.Payloads;

using Newtonsoft.Json;

namespace Kindred.Rewards.Core.Mapping;

public static class RewardParameterMappingHelper
{
    public static RewardParameterApiModelBase MapFromRewardTermsToRewardParameters(
        RewardTerms terms, RewardType rewardType)
    {
        return rewardType switch
        {
            RewardType.UniboostReload => new UniBoostReloadParametersApiModel
            {
                MaxStakeAmount = GetOptionalRewardParameter(terms.RewardParameters, RewardParameterKey.MaxStakeAmount),
                OddsLadderOffset = int.Parse(terms.RewardParameters[RewardParameterKey.OddsLadderOffset]),
                Reload = new()
                {
                    MaxReload = terms.Restrictions.Reload.MaxReload,
                    StopOnMinimumWinBets = terms.Restrictions.Reload.StopOnMinimumWinBets
                },
                MaxExtraWinnings = GetOptionalRewardParameter(terms.RewardParameters, RewardParameterKey.MaxExtraWinnings)
            },
            RewardType.Uniboost => new UniBoostParametersApiModel
            {
                MaxStakeAmount = GetOptionalRewardParameter(terms.RewardParameters, RewardParameterKey.MaxStakeAmount),
                OddsLadderOffset = int.Parse(terms.RewardParameters[RewardParameterKey.OddsLadderOffset]),
                MaxExtraWinnings = GetOptionalRewardParameter(terms.RewardParameters, RewardParameterKey.MaxExtraWinnings)
            },
            RewardType.Freebet => new FreeBetParametersApiModel
            {
                Amount = decimal.Parse(terms.RewardParameters[RewardParameterKey.Amount]),
                MaxExtraWinnings = GetOptionalRewardParameter(terms.RewardParameters, RewardParameterKey.MaxExtraWinnings)
            },
            RewardType.Profitboost => new ProfitBoostParametersApiModel
            {
                MaxStakeAmount = GetOptionalRewardParameter(terms.RewardParameters, RewardParameterKey.MaxStakeAmount),
                LegTable = JsonConvert
                    .DeserializeObject<Dictionary<string, string>>(terms.RewardParameters[RewardParameterKey.LegTable])
                    ?.ToDictionary(x => int.Parse(x.Key), p => decimal.Parse(p.Value)),
                MaxExtraWinnings = GetOptionalRewardParameter(terms.RewardParameters, RewardParameterKey.MaxExtraWinnings)
            },
            _ => throw new ArgumentException("Unsupported Reward Type", nameof(rewardType))
        };
    }

    public static Dictionary<string, string> MapFromRewardParametersToDictionary(
        RewardParameterApiModelBase rewardParameterApiModel,
        string rewardType)
    {
        switch (Enum.Parse<RewardType>(rewardType))
        {
            case RewardType.Freebet:
                var freeBetParameters = (FreeBetParametersApiModel)rewardParameterApiModel;
                return new()
                {
                    { RewardParameterKey.Amount, freeBetParameters.Amount.ToString() },
                    { RewardParameterKey.MaxExtraWinnings, freeBetParameters.MaxExtraWinnings.ToString() }
                };
            case RewardType.UniboostReload:
                var uniBoostReloadParameters = (UniBoostReloadParametersApiModel)rewardParameterApiModel;
                return new()
                {
                    { RewardParameterKey.MaxStakeAmount, uniBoostReloadParameters.MaxStakeAmount.ToString() },
                    { RewardParameterKey.OddsLadderOffset, uniBoostReloadParameters.OddsLadderOffset.ToString() },
                    { RewardParameterKey.MaxExtraWinnings, uniBoostReloadParameters.MaxExtraWinnings.ToString() }
                };
            case RewardType.Uniboost:
                var uniBoostParameters = (UniBoostParametersApiModel)rewardParameterApiModel;
                return new()
                {
                    { RewardParameterKey.MaxStakeAmount, uniBoostParameters.MaxStakeAmount.ToString() },
                    { RewardParameterKey.OddsLadderOffset, uniBoostParameters.OddsLadderOffset.ToString() },
                    { RewardParameterKey.MaxExtraWinnings, uniBoostParameters.MaxExtraWinnings.ToString() }
                };
            case RewardType.Profitboost:
                var profitboostParameters = (ProfitBoostParametersApiModel)rewardParameterApiModel;
                return new()
                {
                    { RewardParameterKey.MaxStakeAmount, profitboostParameters.MaxStakeAmount.ToString() },
                    { RewardParameterKey.LegTable, JsonConvert.SerializeObject(profitboostParameters.LegTable) },
                    { RewardParameterKey.MaxExtraWinnings, profitboostParameters.MaxExtraWinnings.ToString() }
                };
            default:
                return new();
        }
    }

    public static RewardReloadConfig MapFromRewardParametersToRewardReload(RewardParameterApiModelBase rewardParameterApiModel,
        string rewardType)
    {
        if (!RewardTypeRetriever.GetReloadableRewardTypes().Contains(Enum.Parse<RewardType>(rewardType)))
        {
            return default;
        }

        if (rewardParameterApiModel is not UniBoostReloadParametersApiModel uniBoostReloadParameters)
        {
            return default;
        }

        return new()
        {
            MaxReload = uniBoostReloadParameters.Reload.MaxReload,
            StopOnMinimumWinBets = uniBoostReloadParameters.Reload.StopOnMinimumWinBets
        };
    }

    private static decimal? GetOptionalRewardParameter(IDictionary<string, string> rewardParameters, string key)
    {
        return rewardParameters.TryGetValue(key, out var result) &&
                decimal.TryParse(result, out _)
            ? decimal.Parse(result)
            : null;
    }
}
