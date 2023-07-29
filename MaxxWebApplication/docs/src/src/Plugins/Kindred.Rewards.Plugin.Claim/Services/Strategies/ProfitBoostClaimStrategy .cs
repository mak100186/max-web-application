using AutoMapper;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Models.Events;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Plugin.Claim.Models.Dto;
using Kindred.Rewards.Plugin.Claim.Services.Strategies.Base;
using Kindred.Rewards.Plugin.Claim.Services.Strategies.Validations;

using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.Claim.Services.Strategies;

public class ProfitBoostClaimStrategy : RewardClaimStrategyBase
{
    public ProfitBoostClaimStrategy(RewardsDbContext context, IMapper mapper, ILogger<ProfitBoostClaimStrategy> logger)
        : base(context, mapper, logger)
    {
        RewardName = nameof(RewardType.Profitboost);
    }

    protected override bool ShouldCalculateRewardPayout(RewardBet rewardBet)
    {
        if (rewardBet.IsRewardPayable())
        {
            return true;
        }

        rewardBet.RewardPaymentAmount = null;

        return false;

    }

    protected override async Task<bool> CalculateRewardPaymentAmountAsync(RewardBet rewardBet)
    {
        Logger.LogInformation("{@rewardBet}", rewardBet);

        var originalCompoundOdds = rewardBet
            .StagesAboveMinimumOddRestriction()
            .Select(x => x.OriginalOdds)
            .Aggregate((a, b) => a * b);

        rewardBet.RewardPaymentAmount = CalculateRewardPayment(rewardBet, originalCompoundOdds);
        rewardBet.BoostedOdds = CalculateBoostedCompoundOdds(rewardBet, originalCompoundOdds);

        Logger.LogInformation(
            "Reward bet {@rewardBet} successfully calculated reward payout {rewardPaymentAmount} for boosted compound odds {boostedCompoundOdds} which were boosted from fixed compound odds of {originalCompoundOdds}",
            rewardBet,
            rewardBet.RewardPaymentAmount,
            rewardBet.BoostedOdds,
            originalCompoundOdds);

        return await Task.FromResult(true);
    }

    private decimal CalculateRewardPayment(RewardBet rewardBet, decimal originalCompoundOdds)
    {
        var winningsFromApplicableStakeAndOriginalOdds = CalculateWinnings(
            rewardBet.GetStakePortionApplicableToReward(),
            originalCompoundOdds);

        var extraWinnings = winningsFromApplicableStakeAndOriginalOdds * (GetPercentageBoost(rewardBet) / 100.0m);

        return rewardBet.CalculateCappedExtraWinnings(extraWinnings);
    }

    protected override async Task<RewardPaymentAmountResultDto> CalculateRewardPaymentAmountAsync(SettleClaimParameterDto settleClaimParameterDto)
    {
        Logger.LogInformation("{@settleClaimParameterDto}", settleClaimParameterDto);

        if (!ShouldCalculateRewardPayout(settleClaimParameterDto))
        {
            return new() { Payoff = 0 };
        }

        var originalCompoundOdds = settleClaimParameterDto
            .StagesAboveMinimumOddRestriction()
            .Select(x => x.RequestedPrice)
            .Aggregate((a, b) => a * b);

        var result = new RewardPaymentAmountResultDto
        {
            Payoff = CalculateRewardPayment(settleClaimParameterDto, originalCompoundOdds)
        };

        Logger.LogInformation(
            "Reward settle claim {@response} successfully calculated reward payout {rewardPaymentAmount} " +
            "using odds of {originalCompoundOdds}",
            result,
            result.Payoff,
            originalCompoundOdds);

        return result;
    }

    private decimal CalculateRewardPayment(SettleClaimParameterDto settleClaimParameterDto, decimal originalCompoundOdds)
    {
        var winningsFromApplicableStakeAndOriginalOdds = CalculateWinnings(
            settleClaimParameterDto.GetStakePortionApplicableToReward(),
            originalCompoundOdds);

        var extraWinnings = winningsFromApplicableStakeAndOriginalOdds * (GetPercentageBoost(settleClaimParameterDto) / 100.0m);

        return settleClaimParameterDto.CalculateCappedExtraWinnings(extraWinnings);
    }

    private decimal CalculateBoostedCompoundOdds(RewardBet rewardBet, decimal originalCompoundOdds)
    {
        var normalWinnings = CalculateWinnings(rewardBet.Stake, originalCompoundOdds);

        return (CalculateRewardPayment(rewardBet, originalCompoundOdds) + normalWinnings + rewardBet.Stake) / rewardBet.Stake;
    }

    private decimal GetPercentageBoost(RewardBet rewardBet)
    {
        return LegTable
            .GetLegTable(rewardBet.Terms.RewardParameters, rewardBet.GetBetType())
            .GetLegDefinition(rewardBet.StagesAboveMinimumOddRestriction().Count)
            .Value;
    }

    private decimal GetPercentageBoost(SettleClaimParameterDto settleClaimParameterDto)
    {
        return LegTable
            .GetLegTable(settleClaimParameterDto.RewardClaim.Terms.RewardParameters, settleClaimParameterDto.GetBetType())
            .GetLegDefinition(settleClaimParameterDto.StagesAboveMinimumOddRestriction().Count())
            .Value;
    }

    public override ClaimResultDomainModel ValidateClaim(RewardClaimDomainModel reward, BatchClaimItemDomainModel claim)
    {
        var claimResult = base.ValidateClaim(reward, claim);

        if (!claimResult.Success)
        {
            return claimResult;
        }

        if (!CommonClaimValidations.ValidateMinNumberOfLegs(reward, claim, claimResult))
        {
            return claimResult;
        }

        if (!CommonClaimValidations.ValidateBetType(reward, claim, claimResult))
        {
            return claimResult;
        }

        return null;
    }

    private static decimal CalculateWinnings(decimal stake, decimal odds) => (stake * odds) - stake;

    private bool ShouldCalculateRewardPayout(SettleClaimParameterDto settleClaim)
    {
        return settleClaim is { AggregatedSegmentStatus: SettlementSegmentStatus.Won, CombinationSettlementStatus: SettlementCombinationStatus.Resolved };
    }
}
