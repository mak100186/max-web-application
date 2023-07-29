using System.ComponentModel;

using Kindred.Rewards.Core.Extensions;

namespace Kindred.Rewards.Core.Models.RewardConfiguration;

public class RewardTerms
{
    public RewardRestriction Restrictions { get; set; }

    public RewardSettlement SettlementTerms { get; set; }

    public IDictionary<string, string> RewardParameters { get; set; }

    public IDictionary<string, string> Attributes { get; set; }

    public decimal GetAmount(decimal fallback = 0)
    {
        return GetValue(RewardParameterKey.Amount, fallback);
    }

    public decimal GetMaxStakeAmount(decimal fallback = 0)
    {
        return GetValue(RewardParameterKey.MaxStakeAmount, fallback);
    }

    public decimal GetMaxExtraWinningsAmount(decimal fallback = 0)
    {
        return GetValue(RewardParameterKey.MaxExtraWinnings, fallback);
    }

    public decimal GetMinimumStageOdds(decimal fallback = 0)
    {
        return GetValue(RewardParameterKey.MinimumStageOdds, fallback);
    }

    public decimal GetMinimumCompoundOdds(decimal fallback = 0)
    {
        return GetValue(RewardParameterKey.MinimumCompoundOdds, fallback);
    }

    public IEnumerable<string> GetAllowedFormulae(string fallback = null)
    {
        var result = GetValue(RewardParameterKey.AllowedFormulae, fallback);

        return result?.ExtractValues();
    }
    public string GetLegTable(string fallback = null)
    {
        return GetValue(RewardParameterKey.LegTable, fallback);
    }

    public bool IsKeyPresent(string key)
    {
        return RewardParameters.ContainsKey(key);
    }

    public int? GetMinStages(int? fallback = 0)
    {
        return GetValue(RewardParameterKey.MinStages, fallback);
    }

    public int? GetMaxStages(int? fallback = 0)
    {
        return GetValue(RewardParameterKey.MaxStages, fallback);
    }
    public int? GetMinCombinations(int? fallback = 0)
    {
        return GetValue(RewardParameterKey.MinCombinations, fallback);
    }

    public int? GetMaxCombinations(int? fallback = 0)
    {
        return GetValue(RewardParameterKey.MaxCombinations, fallback);
    }

    public int GetOddsLadderOffset(int fallback = 0)
    {
        return GetValue(RewardParameterKey.OddsLadderOffset, fallback);
    }

    private T GetValue<T>(string key, T fallback = default)
    {
        if (!IsKeyPresent(key))
        {
            return fallback;
        }

        try
        {
            var conv = TypeDescriptor.GetConverter(typeof(T));
            return (T)conv.ConvertFrom(RewardParameters[key]);
        }
        catch
        {
            return fallback;
        }
    }
}
