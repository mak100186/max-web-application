using AutoMapper;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Models.Events;
using Kindred.Rewards.Plugin.Claim.Models.Dto;
using Kindred.Rewards.Plugin.Claim.Services.Strategies.Base;

using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.Claim.Services.Strategies;

public class FreeBetClaimStrategy : RewardClaimStrategyBase
{
    public FreeBetClaimStrategy(RewardsDbContext context, IMapper mapper, ILogger<FreeBetClaimStrategy> logger)
        : base(context, mapper, logger)
    { }

    protected override bool ShouldCalculateRewardPayout(RewardBet rewardBet)
    {
        rewardBet.StakeDeduction = rewardBet.Terms == null ? 0m : rewardBet.Terms.GetAmount();
        if (rewardBet.IsRewardPayable())
        {
            return true;
        }

        rewardBet.RewardPaymentAmount = null;

        return false;
    }

    protected override async Task<bool> CalculateRewardPaymentAmountAsync(RewardBet rewardBet)
    {
        var originalCompoundOdds = rewardBet
            .StagesAboveMinimumOddRestriction()
            .Select(x => x.OriginalOdds)
            .Aggregate((a, b) => a * b);

        var extraWinnings = rewardBet.Stake * originalCompoundOdds - rewardBet.Stake;

        rewardBet.RewardPaymentAmount = CalculateRewardPayment(rewardBet, extraWinnings);

        Logger.LogInformation(
            $"Reward bet {{@rewardBet}} successfully calculated reward payout {rewardBet.RewardPaymentAmount} for fixed compound odds of {originalCompoundOdds}",
            rewardBet);

        return await Task.FromResult(true);
    }

    protected override async Task<RewardPaymentAmountResultDto> CalculateRewardPaymentAmountAsync(
        SettleClaimParameterDto settleClaimParameterDto)
    {
        if (settleClaimParameterDto.BetPayoff == 0)
        {
            return new() { Payoff = 0 };
        }

        if (settleClaimParameterDto.BetPayoff >= settleClaimParameterDto.RewardClaim.Terms.GetAmount())
        {
            return new() { Payoff = -settleClaimParameterDto.CombinationStake };
        }

        return settleClaimParameterDto.BetPayoff < settleClaimParameterDto.RewardClaim.Terms.GetAmount()
            ? new() { Payoff = -settleClaimParameterDto.BetPayoff }
            : new RewardPaymentAmountResultDto();
    }

    private static decimal CalculateRewardPayment(RewardBet rewardBet, decimal extraWinnings)
    {
        return rewardBet.CalculateCappedExtraWinnings(extraWinnings);
    }
}
