using Kindred.Rewards.Core.WebApi.Enums;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Plugin.Reward.Models;
using ContestStatus = Kindred.Rewards.Core.WebApi.Enums.ContestStatus;

namespace Kindred.Rewards.Rewards.Tests.Common.Extensions.DataModifiers;
public static class RewardRequestModifiers
{
    public static RewardRequest WithMinimumOdds(this RewardRequest createRewardRequest, decimal? minimumStageOdds = null, decimal? minimumCompoundOdds = null)
    {
        createRewardRequest.DomainRestriction.OddLimits = new()
        {
            MinimumStageOdds = minimumStageOdds,
            MinimumCompoundOdds = minimumCompoundOdds,
        };

        return createRewardRequest;
    }

    public static RewardRequest WithMaxExtraWinnings(this RewardRequest createRewardRequest, decimal? maxExtraWinnings = null)
    {
        createRewardRequest.RewardParameters.MaxExtraWinnings = maxExtraWinnings;

        return createRewardRequest;
    }

    public static RewardRequest WithAmount(this RewardRequest createRewardRequest, decimal? amount)
    {
        if (createRewardRequest.RewardType == RewardType.Freebet.ToString())
        {
            var fpam = createRewardRequest.RewardParameters as FreeBetParametersApiModel;
            fpam.Amount = (decimal)amount;
        }

        return createRewardRequest;
    }

    public static RewardRequest WithAllowedContestStatus(this RewardRequest createRewardRequest, string? contestStatus)
    {
        if (string.IsNullOrWhiteSpace(contestStatus))
        {
            return createRewardRequest;
        }

        createRewardRequest.DomainRestriction.FilterContestStatuses = Enum.Parse<ContestStatus>(contestStatus);
        return createRewardRequest;
    }


    public static RewardRequest WithStartDaysFromNow(this RewardRequest createRewardRequest, string startDaysFromNow)
    {
        if (string.IsNullOrWhiteSpace(startDaysFromNow))
        {
            createRewardRequest.Restrictions.StartDateTime = null;
            return createRewardRequest;
        }

        createRewardRequest.Restrictions.StartDateTime = DateTime.UtcNow.AddDays(int.Parse(startDaysFromNow));
        return createRewardRequest;
    }

    public static RewardRequest WithReload(this RewardRequest createRewardRequest, int? maxReload, int stopOnMinimumWinBets)
    {
        if (createRewardRequest.RewardType != RewardType.UniboostReload.ToString())
        {
            return createRewardRequest;
        }

        var rewardParameters = createRewardRequest.RewardParameters as UniBoostReloadParametersApiModel;

        rewardParameters.Reload = new()
        {
            MaxReload = maxReload,
            StopOnMinimumWinBets = stopOnMinimumWinBets,
        };

        return createRewardRequest;
    }
}
