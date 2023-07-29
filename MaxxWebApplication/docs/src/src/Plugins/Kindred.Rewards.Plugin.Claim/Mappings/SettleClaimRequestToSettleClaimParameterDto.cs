using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.RewardClaims.Bet;
using Kindred.Rewards.Plugin.Claim.Models.Dto;
using Kindred.Rewards.Plugin.Claim.Models.Requests;
using Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim;
using SettlementCombinationStatus = Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim.Enums.SettlementCombinationStatus;
using SettlementSegmentStatus = Kindred.Rewards.Plugin.Claim.Models.Requests.SettleClaim.Enums.SettlementSegmentStatus;

namespace Kindred.Rewards.Plugin.Claim.Mappings;

public static class SettleClaimRequestToSettleClaimParameterDto
{
    public static SettleClaimParameterDto ToDto(this SettleClaimRequest request, IEnumerable<CompoundStagePayload> stages,
        CombinationPayload combination, RewardClaimDomainModel claim)
    {
        return new()
        {
            RewardClaim = claim,
            BetStake = request.Stake,
            CombinationStake = request.Stake / request.AcceptedCombinations.Count(),
            BetPayoff = request.Settlement.FinalPayoff,
            BetRn = request.Rn,
            CustomerId = request.CustomerId,
            Formula = request.Formula.ToString(),
            CombinationStages = stages.Select(x => x.ToDomain()).ToList(),
            CombinationRn = combination.Rn,
            AggregatedSegmentStatus =
                combination.Settlement.Segments.All(x => x.Status is SettlementSegmentStatus.Won)
                    ? Core.Enums.SettlementSegmentStatus.Won
                    : Core.Enums.SettlementSegmentStatus.Lost,
            CombinationSettlementStatus = Enum.Parse<Core.Enums.SettlementCombinationStatus>(combination.Settlement.Status.ToString()),
            BetOutcome = GetBetOutcome(request.AcceptedCombinations)
        };
    }

    private static CompoundStageDomainModel ToDomain(this CompoundStagePayload dto)
    {
        return new()
        {
            ContestType = dto.Market.GetContestTypeFromMarket() ?? string.Empty,
            Market = dto.Market,
            RequestedOutcome = dto.Selection.Outcome,
            RequestedPrice = dto.Odds.Price,
        };
    }

    private static BetOutcome? GetBetOutcome(IEnumerable<CombinationPayload> combinations)
    {
        var combinationPayloads = combinations.ToList();

        if (combinationPayloads.Any(x => x.Settlement.Status is not SettlementCombinationStatus.Resolved))
        {
            return null;
        }

        if (combinationPayloads.All(x => x.Settlement.Segments.All(y => y.Status is SettlementSegmentStatus.Won)))
        {
            return BetOutcome.Winning;
        }

        if (combinationPayloads.All(x => x.Settlement.Segments.All(y => y.Status is SettlementSegmentStatus.Refunded)))
        {
            return BetOutcome.FullRefund;
        }

        if (combinationPayloads.All(x => x.Settlement.Segments.Any(y => y.Status is SettlementSegmentStatus.PartWon or SettlementSegmentStatus.PartRefunded)))
        {
            return BetOutcome.WinningAndPartialRefund;
        }

        return BetOutcome.Losing;
    }
}
