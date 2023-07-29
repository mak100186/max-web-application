using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Plugin.Reward.Services;

namespace Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

public static class RewardBaseDomainModelBuilder
{
    public static T Create<T>(RewardType rewardType = RewardType.Freebet, Action<T> action = null)
        where T : RewardDomainModel, new()
    {
        const PurposeType defaultPurposeType = PurposeType.Crm;

        var model = new T
        {
            Category = rewardType.ToRewardCategory(),
            Type = rewardType,
            Terms = GetTerms(rewardType),

            Name = "DefaultRewardName" + Guid.NewGuid(),
            CustomerFacingName = "DefaultCustomerFacingName",
            Comments = "DefaultComments",
            CountryCode = "AUS",

            IsCancelled = false,
            CancellationReason = null,

            Purpose = defaultPurposeType.ToString(),
            SubPurpose = defaultPurposeType.GetSubPurpose().First().ToString(),

            RewardId = Guid.NewGuid().ToString(),
            SourceInstanceId = default,

            Tags = new List<string> { "DefaultTag" },

            CreatedBy = "ModelBuilder",
            UpdatedBy = "ModelBuilder"
        };

        action?.Invoke(model);

        return model;
    }

    public static RewardTerms GetTerms(RewardType rewardType)
    {
        return rewardType switch
        {
            RewardType.Freebet => new()
            {
                Restrictions = new()
                {
                    ClaimInterval = DomainConstants.Every30SecondsCronInterval,
                    ClaimAllowedPeriod = null,
                    ClaimsPerInterval = 1,
                    StartDateTime = DateTime.MinValue,
                    ExpiryDateTime = DateTime.MaxValue
                },
                SettlementTerms = FreeBetCreationStrategy.DefaultSettlementTerms,
                RewardParameters = FreeBetCreationStrategy.DefaultParameterKeys,
                Attributes = new Dictionary<string, string>()
            },
            RewardType.Uniboost => new()
            {
                Restrictions = new()
                {
                    ClaimInterval = DomainConstants.Every30SecondsCronInterval,
                    ClaimAllowedPeriod = null,
                    ClaimsPerInterval = 1,
                    StartDateTime = DateTime.MinValue,
                    ExpiryDateTime = DateTime.MaxValue
                },
                SettlementTerms = UniBoostCreationStrategy.DefaultSettlementTerms,
                RewardParameters = UniBoostCreationStrategy.DefaultParameterKeys,
                Attributes = new Dictionary<string, string>()
            },
            RewardType.UniboostReload => new()
            {
                Restrictions = new()
                {
                    ClaimInterval = DomainConstants.Every30SecondsCronInterval,
                    ClaimAllowedPeriod = null,
                    ClaimsPerInterval = 1,
                    StartDateTime = DateTime.MinValue,
                    ExpiryDateTime = DateTime.MaxValue,
                    Reload = new()
                    {
                        MaxReload = 3,
                        StopOnMinimumWinBets = 5
                    }
                },
                SettlementTerms = UniBoostCreationStrategy.DefaultSettlementTerms,
                RewardParameters = UniBoostCreationStrategy.DefaultParameterKeys,
                Attributes = new Dictionary<string, string>()
            },
            RewardType.Profitboost => new()
            {
                Restrictions = new()
                {
                    ClaimInterval = DomainConstants.Every30SecondsCronInterval,
                    ClaimAllowedPeriod = null,
                    ClaimsPerInterval = 1,
                    StartDateTime = DateTime.MinValue,
                    ExpiryDateTime = DateTime.MaxValue,
                },
                SettlementTerms = ProfitBoostCreationStrategy.DefaultSettlementTerms,
                RewardParameters = ProfitBoostCreationStrategy.DefaultParameterKeys,
                Attributes = new Dictionary<string, string>()
            },
            _ => throw new ArgumentOutOfRangeException(nameof(rewardType), rewardType, null)
        };
    }

}
