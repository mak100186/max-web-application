using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Models.Messages;

public class RewardBet
{
    public string BetRn { get; set; } //MinotaurBet.Rn
    public decimal Stake { get; set; } //MinotaurBet.Stake
    public string Formula { get; set; } //MinotaurBet.Formula
    public BetStatus? Status { get; set; } //MinotaurBet.Status
    public string CustomerId { get; set; } //MinotaurBet.CustomerId
    public string CurrencyCode { get; set; } //MinotaurBet.Currency
    public BetOutcome? BetOutcome { get; set; } //MinotaurBet.AcceptedCombinations[].Settlement.Segments[].Status
    public ICollection<Combination> AcceptedCombinations { get; set; }//MinotaurBet.AcceptedCombinations
    public IReadOnlyCollection<CompoundStage> Stages { get; set; } //MinotaurBet.Stages


    //populated at settling or unsettling claim
    public string RewardRn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string ClaimInstanceId { get; set; }
    public RewardConfiguration.RewardTerms Terms { get; set; }
    public RewardType Type { get; set; }
    public string RewardName { get; set; }

    //populated by the applied strategy
    public decimal? RewardPaymentAmount { get; set; }
    public decimal? StakeDeduction { get; set; }
    public decimal? BoostedOdds { get; set; } //populated by the strategy itself

    //populated by settlement handlers
    public decimal ReturnAmount { get; set; }

    public bool CompliesWithRestrictions()
    {
        var stagesAboveMinimumOddRestriction = StagesAboveMinimumOddRestriction();

        if (!stagesAboveMinimumOddRestriction.Any())
        {
            return false;
        }

        var compoundOdds = Stages.Select(x => x.OriginalOdds).Aggregate((a, b) => a * b);
        if (Terms.Restrictions.OddLimits != null && compoundOdds < Terms.Restrictions.OddLimits.MinimumCompoundOdds)
        {
            return false;
        }

        var hasContestRefRestrictions = false;
        var hasContestTypeRestrictions = false;
        var hasOutcomeRestrictions = false;

        var isPassingContestRefRestrictions = false;
        var isPassingContestTypeRestrictions = false;
        var isPassingOutcomeRestrictions = false;

        if (Terms.Restrictions.AllowedContestRefs.IsNotNullAndNotEmpty())
        {
            hasContestRefRestrictions = true;

            isPassingContestRefRestrictions = stagesAboveMinimumOddRestriction.Any(stage => Terms.Restrictions.AllowedContestRefs.Contains(stage.Market));
        }

        if (Terms.Restrictions.AllowedContestTypes.IsNotNullAndNotEmpty())
        {
            hasContestTypeRestrictions = true;

            isPassingContestTypeRestrictions = stagesAboveMinimumOddRestriction.Any(stage => Terms.Restrictions.AllowedContestTypes.Contains(stage.ContestType));
        }

        if (Terms.Restrictions.AllowedOutcomes.IsNotNullAndNotEmpty())
        {
            hasOutcomeRestrictions = true;

            isPassingOutcomeRestrictions = stagesAboveMinimumOddRestriction.Any(stage => Terms.Restrictions.AllowedOutcomes.Contains(stage.Selection.Outcome));
        }

        List<dynamic> restrictionResultSets = new()
        {
            new { HasValue = hasContestTypeRestrictions, IsPassing = isPassingContestTypeRestrictions },
            new { HasValue = hasContestRefRestrictions,  IsPassing = isPassingContestRefRestrictions },
            new { HasValue = hasOutcomeRestrictions,  IsPassing = isPassingOutcomeRestrictions}
        };

        //if it HasValue and !IsPassing then the Bet is NOT valid
        //if it does not HasValue, then we allow all of that particular thing
        return restrictionResultSets.All(restrictionResultSet => !restrictionResultSet.HasValue || restrictionResultSet.IsPassing);
    }

    public IReadOnlyCollection<CompoundStage> StagesAboveMinimumOddRestriction()
    {
        if (Terms?.Restrictions?.OddLimits?.MinimumStageOdds == null)
        {
            return Stages;
        }

        var result = Stages
            .Where(stage => stage.OriginalOdds >= Terms.Restrictions.OddLimits.MinimumStageOdds)
            .ToList();

        return result;
    }

    public decimal GetStakePortionApplicableToReward()
    {
        return Math.Min(Terms.GetMaxStakeAmount(), Stake);
    }

    public decimal GetStakePortionNotApplicableToReward()
    {
        return Math.Max(Stake - GetStakePortionApplicableToReward(), 0);
    }

    /// <summary>
    ///    Calculates the capped extra winnings based on the rewards configured max extra winnings if it exists.
    /// </summary>
    public decimal CalculateCappedExtraWinnings(decimal extraWinnings)
    {
        return Terms.GetMaxExtraWinningsAmount() == 0
            ? extraWinnings
            : Math.Min(extraWinnings, Terms.GetMaxExtraWinningsAmount());
    }

    public override string ToString()
    {
        return
            $"RewardType:{Type}, RewardRn:{RewardRn}, ClaimInstanceId:{ClaimInstanceId}, CustomerId:{CustomerId}, RewardName:{RewardName}, Stake:{Stake}, Status:{Status}";
    }
}
