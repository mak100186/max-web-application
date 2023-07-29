using Kindred.Rewards.Core.Models.RewardConfiguration;

using Newtonsoft.Json;

namespace Kindred.Rewards.Rewards.Tests.Common.Extensions.DataModifiers;
public static class RewardTermsModifiers
{

    public static RewardTerms WithMinimumOdds(this RewardTerms rewardTerms, decimal? minimumStageOdds = null)
    {
        rewardTerms.Restrictions.OddLimits = new() { MinimumStageOdds = minimumStageOdds };

        return rewardTerms;
    }

    public static RewardTerms WithMaxExtraWinnings(this RewardTerms rewardTerms, decimal? maxExtraWinnings = null)
    {
        rewardTerms.RewardParameters[RewardParameterKey.MaxExtraWinnings] = maxExtraWinnings.GetValueOrDefault().ToString();

        return rewardTerms;
    }

    public static RewardTerms WithMaxStake(this RewardTerms rewardTerms, decimal? maxStake = null)
    {
        rewardTerms.RewardParameters[RewardParameterKey.MaxStakeAmount] = maxStake.GetValueOrDefault().ToString();

        return rewardTerms;
    }

    public static RewardTerms WithLegTable(this RewardTerms rewardTerms, Dictionary<string, string> legTable = null)
    {
        if (legTable == null && rewardTerms.RewardParameters.ContainsKey(RewardParameterKey.LegTable))
        {
            rewardTerms.RewardParameters.Remove(RewardParameterKey.LegTable);
            return rewardTerms;
        }

        rewardTerms.RewardParameters[RewardParameterKey.LegTable] = JsonConvert.SerializeObject(legTable);

        return rewardTerms;
    }
}
