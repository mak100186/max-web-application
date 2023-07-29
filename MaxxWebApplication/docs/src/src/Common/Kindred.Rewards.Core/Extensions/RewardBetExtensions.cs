using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models.Events;
using Kindred.Rewards.Core.Models.RewardConfiguration;

namespace Kindred.Rewards.Core.Extensions;

public static class RewardBetExtensions
{
    public static bool IsSingleBet(this RewardBet rewardBet)
    {
        rewardBet.ThrowIfNull($"{nameof(rewardBet)} is null");

        return rewardBet.GetStageCount() == 1;
    }

    public static bool IsMultiBet(this RewardBet rewardBet)
    {
        rewardBet.ThrowIfNull($"{nameof(rewardBet)} is null");

        return rewardBet.GetStageCount() > 1; //we could have multiple stages of formula.singles
    }

    public static int GetStageCount(this RewardBet rewardBet)
    {
        rewardBet.ThrowIfNull($"{nameof(rewardBet)} is null");

        return rewardBet.Stages.Count;
    }

    public static BetTypes GetBetType(this RewardBet rewardBet)
    {
        rewardBet.ThrowIfNull($"{nameof(rewardBet)} is null");

        return BetDomainModelExtensions.DeduceBetType(rewardBet.Stages.Count, rewardBet.Formula);
    }

    public static IReadOnlyCollection<BetTypes> GetApplicableBetTypes(this IDictionary<string, string> rewardParameters)
    {
        rewardParameters.ThrowIfNull($"{nameof(rewardParameters)} is null");

        rewardParameters.TryGetValue(RewardParameterKey.MinStages, out var minStagesString);
        rewardParameters.TryGetValue(RewardParameterKey.MaxStages, out var maxStagesString);
        rewardParameters.TryGetValue(RewardParameterKey.MinCombinations, out var minCombinationsString);
        rewardParameters.TryGetValue(RewardParameterKey.MaxCombinations, out var maxCombinationsString);

        var allowedFormulae = rewardParameters.GetValue<string>(RewardParameterKey.AllowedFormulae).ExtractValues();

        return allowedFormulae.GetApplicableToBetTypes(minStagesString.ToInt(1), maxStagesString.ToInt(1), minCombinationsString.ToInt(1), maxCombinationsString.ToInt(1));
    }

    public static bool IsRewardPayable(this RewardBet rewardBet)
    {
        return rewardBet.BetOutcome.IsWinningBet() &&
               rewardBet.IsApplicableToCurrentBetType() &&
               rewardBet.CompliesWithRestrictions();
    }

    public static bool IsApplicableToCurrentBetType(this RewardBet rewardBet)
    {
        try
        {
            var betType = rewardBet.GetBetType();

            var applicableTo = rewardBet.Terms.RewardParameters.GetApplicableBetTypes();

            if (!applicableTo.Contains(betType))
            {
                return false;
            }

            var legCount = rewardBet.GetStageCount();
            var minimumAllowedLegs = rewardBet.Terms.GetMinStages(DomainConstants.MinNumberOfLegsInMulti).GetValueOrDefault();
            var maximumAllowedLegs = rewardBet.Terms.GetMaxStages(DomainConstants.MaxNumberOfLegsInMulti).GetValueOrDefault();

            return legCount.IsInRangeInclusive(minimumAllowedLegs, maximumAllowedLegs);
        }
        catch
        {
            return false;
        }
    }
}
