using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models.RewardClaims;

namespace Kindred.Rewards.Core.Validations;

public static class CommonClaimValidations
{
    public static bool ValidateSingleBetType(RewardClaimDomainModel reward, BatchClaimItemDomainModel claim, ClaimResultDomainModel claimResult)
    {
        var betType = claim.Bet.DeduceBetType();

        if (betType == BetTypes.SingleLeg)
        {
            return true;
        }

        claimResult.ErrorMessage = $"Bet type of {betType} is not an allowed bet type for this reward. Allowed BetTypes:[{BetTypes.SingleLeg}]";
        return false;

    }
    public static bool ValidateBetType(RewardClaimDomainModel reward, BatchClaimItemDomainModel claim, ClaimResultDomainModel claimResult)
    {
        try
        {
            var allowedBetTypes = reward.Terms.RewardParameters.GetApplicableBetTypes();

            var betType = claim.Bet.DeduceBetType();

            if (!allowedBetTypes.Contains(betType))
            {
                claimResult.ErrorMessage = $"Bet type of {betType} is not an allowed bet type for this reward. Allowed BetTypes:[{allowedBetTypes.ToCsv()}]";
                return false;
            }

        }
        catch (Exception e)
        {
            claimResult.ErrorMessage = $"Exception occurred while validating bet type, message: {e.Message}";
            return false;
        }
        return true;
    }
    public static bool ValidateMinNumberOfLegs(RewardClaimDomainModel reward, BatchClaimItemDomainModel claim, ClaimResultDomainModel claimResult)
    {

        try
        {
            var betTypes = reward.Terms.RewardParameters.GetApplicableBetTypes();

            if (claim.Bet.Stages.Count == 1 && betTypes.Contains(BetTypes.SingleLeg))
            {
                return true;
            }

            var minStages = reward.Terms.GetMinStages().GetValueOrDefault();
            var maxStages = reward.Terms.GetMaxStages().GetValueOrDefault();

            if (betTypes.ContainsMultiBetTypes() &&
                minStages > 0 &&
                maxStages > 0 &&
                !claim.Bet.Stages.Count.IsInRangeInclusive(minStages, maxStages))
            {
                claimResult.ErrorMessage = $"Multi bet has {claim.Bet.Stages.Count} stages which is not in allowed range: [{minStages}-{maxStages}]";
                return false;
            }
        }
        catch (Exception e)
        {
            claimResult.ErrorMessage = $"Exception occurred while validating minStages, message: {e.Message}";
            return false;
        }

        return true;
    }
    public static bool ValidateMinimumStageOdds(RewardClaimDomainModel reward, BatchClaimItemDomainModel claim, ClaimResultDomainModel claimResult)
    {
        var minimumStageOdds = reward.Terms.GetMinimumStageOdds();
        if (minimumStageOdds == 0)
        {
            return true;
        }

        var stageOdds = claim.Bet.Stages.Select(x => x.RequestedPrice).ToCsv();
        if (claim.Bet.Stages.Any(x => x.RequestedPrice < minimumStageOdds))
        {
            claimResult.ErrorMessage = $"All stage odds {stageOdds} must be equal to or greater than {minimumStageOdds} to claim {reward.Type} reward";
            return false;
        }

        return true;
    }
    public static bool ValidateMinimumCompoundOdds(RewardClaimDomainModel reward, BatchClaimItemDomainModel claim, ClaimResultDomainModel claimResult)
    {
        var minimumCompoundOdds = reward.Terms.GetMinimumCompoundOdds();
        if (minimumCompoundOdds == 0)
        {
            return true;
        }

        var compoundOdds = claim.Bet.Stages.Select(x => x.RequestedPrice).Aggregate((a, b) => a * b);
        if (compoundOdds < minimumCompoundOdds)
        {
            claimResult.ErrorMessage = $"Compound odds {compoundOdds} must be equal to or greater than {minimumCompoundOdds} to claim {reward.Type} reward";
            return false;
        }

        return true;
    }
}
