using AutoMapper;

using Kindred.Rewards.Core.Client;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Events;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Plugin.Claim.Clients.MarketMirror;
using Kindred.Rewards.Plugin.Claim.Models.Dto;
using Kindred.Rewards.Plugin.Claim.Services.Strategies.Base;
using Kindred.Rewards.Plugin.Claim.Services.Strategies.Validations;

using Microsoft.Extensions.Logging;

using ContestStatus = Kindred.Rewards.Plugin.Claim.Clients.MarketMirror.Responses.ContestStatus;

namespace Kindred.Rewards.Plugin.Claim.Services.Strategies;

public class UniBoostClaimStrategy : RewardClaimStrategyBase
{
    protected IOddsLadderClient OddsLadderLogic;
    protected IMarketMirrorClient MarketMirrorClient;

    public UniBoostClaimStrategy(RewardsDbContext context, IMapper mapper, ILogger<UniBoostClaimStrategy> logger, IOddsLadderClient oddsLadderLogic, IMarketMirrorClient marketMirrorClient)
        : base(context, mapper, logger)
    {
        OddsLadderLogic = oddsLadderLogic;
        RewardName = nameof(RewardType.Uniboost);
        MarketMirrorClient = marketMirrorClient;
    }

    protected override bool ShouldCalculateRewardPayout(RewardBet rewardBet)
    {
        rewardBet.StakeDeduction = 0;
        rewardBet.RewardPaymentAmount = null;

        var eligibleStages = rewardBet
            .StagesAboveMinimumOddRestriction();

        if (eligibleStages == null || !eligibleStages.Any() || rewardBet.Stages.Count > 1 || rewardBet.BetOutcome.IsLosingBet())
        {
            rewardBet.RewardPaymentAmount = null;
            rewardBet.BoostedOdds = null;

            return false;
        }

        return true;
    }

    protected override async Task<bool> CalculateRewardPaymentAmountAsync(RewardBet rewardBet)
    {
        var originalCompoundOdds = rewardBet
            .StagesAboveMinimumOddRestriction()
            .Select(x => x.OriginalOdds)
            .Aggregate((a, b) => a * b);

        var boostedCompoundOdds = await GetBoostedOdds(rewardBet, originalCompoundOdds);

        rewardBet.RewardPaymentAmount = CalculateRewardPayment(rewardBet, originalCompoundOdds, boostedCompoundOdds);
        rewardBet.BoostedOdds = boostedCompoundOdds;

        Logger.LogInformation(
            "Reward bet {@rewardBet} successfully calculated reward payout {rewardPaymentAmount} for boosted compound odds {boostedCompoundOdds} which were boosted from fixed compound odds of {originalCompoundOdds}",
            rewardBet,
            rewardBet.RewardPaymentAmount,
            boostedCompoundOdds,
            originalCompoundOdds);

        return await Task.FromResult(true);
    }

    public override async Task<ClaimResultDto> CalculateClaimResultAsync(ClaimParameterDto claimParameterDto)
    {
        var result = new List<RewardClaimOddsMetadataDomainModel>();

        foreach (var stage in claimParameterDto.CombinationStages)
        {
            var boosted = await GetBoostedOdds(claimParameterDto, stage.RequestedPrice);
            result.Add(new()
            {
                Boosted = boosted,
                Original = stage.RequestedPrice,
                Outcome = stage.RequestedOutcome,
            });
        }

        return new()
        {
            PayoffMetadata = new()
            {
                Odds = result
            }
        };
    }

    protected override async Task<RewardPaymentAmountResultDto> CalculateRewardPaymentAmountAsync(SettleClaimParameterDto settleClaimParameterDto)
    {
        if (!ShouldCalculateRewardPayout(settleClaimParameterDto))
        {
            return new() { Payoff = 0 };
        }

        var originalCompoundOdds = settleClaimParameterDto.StagesAboveMinimumOddRestriction()
            .Select(x => x.RequestedPrice)
            .Aggregate((a, b) => a * b);

        var boostedCompoundOdds = settleClaimParameterDto.GetOddsMetadataApplicableToClaim()
            .Select(x => x.Boosted)
            .Aggregate((a, b) => a * b);

        var result = new RewardPaymentAmountResultDto
        {
            Payoff = CalculateRewardPayment(settleClaimParameterDto, originalCompoundOdds, boostedCompoundOdds),
        };

        Logger.LogInformation(
            "Reward settle claim {@response} successfully calculated reward payout {rewardPaymentAmount} " +
            "using boosted odds of {boostedCompoundOdds}",
            result,
            result.Payoff,
            boostedCompoundOdds);

        return result;
    }

    private decimal CalculateRewardPayment(RewardBet rewardBet, decimal originalCompoundOdds, decimal boostedCompoundOdds)
    {
        var winningsFromTotalStakeAndOriginalOdds = CalculateWinnings(rewardBet.Stake, originalCompoundOdds);

        var winningsFromApplicableStakeAndBoostedOdds = CalculateWinnings(rewardBet.GetStakePortionApplicableToReward(), boostedCompoundOdds);

        var winningsFromNotApplicableStakeAndOriginalOdds = CalculateWinnings(rewardBet.GetStakePortionNotApplicableToReward(), originalCompoundOdds);

        var extraWinnings = Math.Max(0, (winningsFromApplicableStakeAndBoostedOdds + winningsFromNotApplicableStakeAndOriginalOdds) - winningsFromTotalStakeAndOriginalOdds);

        return rewardBet.CalculateCappedExtraWinnings(extraWinnings);
    }

    private decimal CalculateRewardPayment(SettleClaimParameterDto settleClaimParameterDto, decimal originalCompoundOdds, decimal boostedCompoundOdds)
    {
        var winningsFromTotalStakeAndOriginalOdds = CalculateWinnings(settleClaimParameterDto.CombinationStake, originalCompoundOdds);

        var winningsFromApplicableStakeAndBoostedOdds = CalculateWinnings(settleClaimParameterDto.GetStakePortionApplicableToReward(), boostedCompoundOdds);

        var winningsFromNotApplicableStakeAndOriginalOdds = CalculateWinnings(settleClaimParameterDto.GetStakePortionNotApplicableToReward(), originalCompoundOdds);

        var extraWinnings = Math.Max(0, (winningsFromApplicableStakeAndBoostedOdds + winningsFromNotApplicableStakeAndOriginalOdds) - winningsFromTotalStakeAndOriginalOdds);

        return settleClaimParameterDto.CalculateCappedExtraWinnings(extraWinnings);
    }

    public override ClaimResultDomainModel ValidateClaim(RewardClaimDomainModel reward, BatchClaimItemDomainModel claim)
    {
        var claimResult = base.ValidateClaim(reward, claim);
        if (!claimResult.Success)
        {
            return claimResult;
        }

        if (!CommonClaimValidations.ValidateSingleBetType(reward, claim, claimResult))
        {
            return claimResult;
        }

        return null;
    }

    protected virtual async Task<decimal> GetBoostedOdds(RewardBet rewardBet, decimal originalOdds)
    {
        var ladder = await GetOddsLadder(rewardBet);

        var fixedOddsIndex = ladder.FindIndex(o => o.Key >= originalOdds);

        if (fixedOddsIndex == -1)
        {
            //couldn't find the fixedOdds at all, use max odds.
            fixedOddsIndex = ladder.Count - 1;
        }
        else if (fixedOddsIndex.IsInRangeExclusive(0, ladder.Count) && !originalOdds.Equals(ladder[fixedOddsIndex].Key))
        {
            //i.e., due to some reason, the default and promotion odds ladder dont match due to which 
            //we couldn't find the corresponding (perfectly matching) Odds in the promotion ladder. Instead
            //we found one just greater than the expected odds. Hence, snapping to 
            //the odds just one below the expected fixed odds.
            fixedOddsIndex--;

            Logger.LogInformation(
                "For the reward bet {rewardBet} couldn't find the matching fixed odds {originalOdds} in the promotions ladder, hence snapping to next lower odds: {key}",
                rewardBet, originalOdds, ladder[fixedOddsIndex].Key);
        }

        var oddsLadderOffset = rewardBet.Terms.GetOddsLadderOffset();
        var oddsBoostOffsetIndex = fixedOddsIndex + oddsLadderOffset;

        decimal offsetOdds;
        if (oddsBoostOffsetIndex >= ladder.Count)
        {
            Logger.LogError(
                $"{nameof(this.GetBoostedOdds)}:[RewardRn={rewardBet.RewardRn}]:[RewardType={rewardBet.Type}]:Odds boost offset {oddsBoostOffsetIndex} is out of range. Applying max offset.");
            offsetOdds = ladder.Last().Key;
        }
        else
        {
            offsetOdds = ladder[oddsBoostOffsetIndex].Key;
        }

        return offsetOdds;
    }

    protected virtual async Task<decimal> GetBoostedOdds(ClaimParameterDto claimParameterDto, decimal originalOdds)
    {
        var ladder = await GetOddsLadder(claimParameterDto);

        var fixedOddsIndex = ladder.FindIndex(o => o.Key >= originalOdds);

        if (fixedOddsIndex == -1)
        {
            //couldn't find the fixedOdds at all, use max odds.
            fixedOddsIndex = ladder.Count - 1;
        }
        else if (fixedOddsIndex.IsInRangeExclusive(0, ladder.Count) && !originalOdds.Equals(ladder[fixedOddsIndex].Key))
        {
            //i.e., due to some reason, the default and promotion odds ladder dont match due to which 
            //we couldn't find the corresponding (perfectly matching) Odds in the promotion ladder. Instead
            //we found one just greater than the expected odds. Hence, snapping to 
            //the odds just one below the expected fixed odds.
            fixedOddsIndex--;

            Logger.LogInformation(
                "For the reward bet {rewardBet} couldn't find the matching fixed odds {originalOdds} in the promotions ladder, hence snapping to next lower odds: {key}",
                claimParameterDto, originalOdds, ladder[fixedOddsIndex].Key);
        }

        var oddsLadderOffset = claimParameterDto.Claim.Terms.GetOddsLadderOffset();
        var oddsBoostOffsetIndex = fixedOddsIndex + oddsLadderOffset;

        decimal offsetOdds;
        if (oddsBoostOffsetIndex >= ladder.Count)
        {
            Logger.LogError(
                $"{nameof(this.GetBoostedOdds)}:[RewardId={claimParameterDto.Claim.Reward.RewardId}]:[RewardType={claimParameterDto.Claim.Reward.Type}]:" +
                $"Odds boost offset {oddsBoostOffsetIndex} is out of range. Applying max offset.");
            offsetOdds = ladder.Last().Key;
        }
        else
        {
            offsetOdds = ladder[oddsBoostOffsetIndex].Key;
        }

        return offsetOdds;
    }

    public async Task<List<Odds>> GetOddsLadder(RewardBet rewardBet)
    {
        var contestType = rewardBet.Stages.Single().ContestType;
        var contestKey = rewardBet.Stages.Single().Market.GetContestKeyFromMarket();

        Logger.LogInformation("Contest type is {contestType}. Contest key is {contestKey}", contestType, contestKey);

        var oddsLadder = await OddsLadderLogic.GetOddsLadder(contestType);
        var getContestsResponse = await MarketMirrorClient.GetContests(new[] { contestKey });

        var contestStatus = getContestsResponse?.Contests?.SingleOrDefault()?.ContestStatus;

        Logger.LogInformation("Contest status is @{contestStatus}", contestStatus);

        return contestStatus switch
        {
            ContestStatus.InPlay => oddsLadder.InPlayOddsLadder,
            // in every other scenario, pregame odds ladder is used
            _ => oddsLadder.PreGameOddsLadder
        };
    }

    public async Task<List<Odds>> GetOddsLadder(ClaimParameterDto claimParameterDto)
    {
        var contestType = claimParameterDto.CombinationStages.Single().ContestType;
        var contestKey = claimParameterDto.CombinationStages.Single().Market.GetContestKeyFromMarket();

        Logger.LogInformation("Contest type is {contestType}. Contest key is {contestKey}", contestType, contestKey);

        var oddsLadder = await OddsLadderLogic.GetOddsLadder(contestType);
        var getContestsResponse = await MarketMirrorClient.GetContests(new[] { contestKey });

        var contestStatus = getContestsResponse?.Contests?.SingleOrDefault()?.ContestStatus;

        Logger.LogInformation("Contest status is {contestStatus}", contestStatus);

        return contestStatus switch
        {
            ContestStatus.InPlay => oddsLadder.InPlayOddsLadder,
            // in every other scenario, pregame odds ladder is used
            _ => oddsLadder.PreGameOddsLadder
        };
    }

    private static decimal CalculateWinnings(decimal stake, decimal odds) => (stake * odds) - stake;

    private bool ShouldCalculateRewardPayout(SettleClaimParameterDto settleClaim)
    {
        return settleClaim is { AggregatedSegmentStatus: SettlementSegmentStatus.Won, CombinationSettlementStatus: SettlementCombinationStatus.Resolved };
    }
}
