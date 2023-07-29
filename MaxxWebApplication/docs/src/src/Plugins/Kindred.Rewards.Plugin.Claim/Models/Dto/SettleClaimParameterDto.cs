using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.RewardClaims.Bet;

namespace Kindred.Rewards.Plugin.Claim.Models.Dto;

/// <summary>
/// Represents a Settle Claim Request and contains all the information required for a Reward Payout to be calculated for a Combination.
/// </summary>
public class SettleClaimParameterDto
{
    public string BetRn { get; init; }
    public decimal BetStake { get; init; }
    public decimal CombinationStake { get; init; }
    public string Formula { get; init; }
    public string CustomerId { get; init; }
    public IEnumerable<CompoundStageDomainModel> CombinationStages { get; init; }
    public RewardClaimDomainModel RewardClaim { get; init; }
    public decimal BetPayoff { get; init; }
    public SettlementSegmentStatus AggregatedSegmentStatus { get; init; }
    public string CombinationRn { get; init; }
    public SettlementCombinationStatus CombinationSettlementStatus { get; init; }

    /// <summary>
    /// Represents the 'entire' Bet status. In other words, the status if we aggregate all the combination statuses in a Bet and deduce an outcome.
    /// This is really only valid for single bets i.e. a bet with one combination and one stage since deducing if a doubles Bet was won or lost is subjective.
    /// </summary>
    public BetOutcome? BetOutcome { get; init; }

    internal IEnumerable<CompoundStageDomainModel> StagesAboveMinimumOddRestriction()
    {
        if (RewardClaim.Terms?.Restrictions?.OddLimits?.MinimumStageOdds == null)
        {
            return CombinationStages;
        }

        var result = CombinationStages
            .Where(stage => stage.RequestedPrice >= RewardClaim.Terms.Restrictions.OddLimits.MinimumStageOdds)
            .ToList();

        return result;
    }

    public decimal GetStakePortionApplicableToReward()
    {
        return Math.Min(RewardClaim.Terms.GetMaxStakeAmount(decimal.MaxValue), CombinationStake);
    }

    public decimal GetStakePortionNotApplicableToReward()
    {
        return Math.Max(CombinationStake - GetStakePortionApplicableToReward(), 0);
    }

    /// <summary>
    ///    Calculates the capped extra winnings based on the rewards configured max extra winnings if it exists.
    /// </summary>
    public decimal CalculateCappedExtraWinnings(decimal extraWinnings)
    {
        return RewardClaim.Terms.GetMaxExtraWinningsAmount() == 0
            ? extraWinnings
            : Math.Min(extraWinnings, RewardClaim.Terms.GetMaxExtraWinningsAmount());
    }

    public BetTypes GetBetType()
    {
        return BetDomainModelExtensions.DeduceBetType(CombinationStages.Count(), Formula);
    }

    public IEnumerable<RewardClaimOddsMetadataDomainModel> GetOddsMetadataApplicableToClaim()
    {
        return RewardClaim.PayoffMetadata.Odds.Where(oddsMetadata =>
            StagesAboveMinimumOddRestriction().Select(stage => stage.RequestedOutcome)
                .Contains(oddsMetadata.Outcome));
    }
}
