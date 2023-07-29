using System.Security.Cryptography;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.WebApi;
using Kindred.Rewards.Plugin.Reward.Models;

using PurposeType = Kindred.Rewards.Core.WebApi.Enums.PurposeType;

namespace Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

public static class CustomizationComposerExtension
{
    public static T UpdateRewardBaseProps<T>(this T model)
        where T : class
    {
        var purposes = Enum.GetValues<PurposeType>();
        var randomPurposeIndex = RandomNumberGenerator.GetInt32(0, purposes.Length);
        var purpose = purposes[randomPurposeIndex];

        var subPurposes = PurposeHelper.GetSubPurpose(purpose).ToList();
        var randomSubPurposeIndex = RandomNumberGenerator.GetInt32(0, subPurposes.Count);
        var subPurpose = subPurposes[randomSubPurposeIndex];

        switch (model)
        {
            case RewardDomainModel baseDomainModel:
                baseDomainModel.CountryCode = TestConstants.DefaultCountryCode;
                baseDomainModel.Purpose = purpose.ToString();
                baseDomainModel.SubPurpose = subPurpose.ToString();
                return baseDomainModel as T;

            case RewardRequest createBonusRequest:
                createBonusRequest.CountryCode = TestConstants.DefaultCountryCode;
                createBonusRequest.Purpose = purpose.ToString();
                createBonusRequest.SubPurpose = subPurpose.ToString();
                return createBonusRequest as T;

            default:
                return model;
        }
    }

    public static T UpdateRewardClassSpecificProps<T>(this T model, RewardType rewardType)
        where T : class
    {
        switch (model)
        {
            case RewardDomainModel bonusDomainModel:
                bonusDomainModel.Name = $"{TestConstants.TestPrefix}_{rewardType}_{Guid.NewGuid()}";
                bonusDomainModel.Comments = TestConstants.DefaultPromotionComments;
                bonusDomainModel.CustomerId ??= TestConstants.DefaultCustomerId;
                return bonusDomainModel as T;

            case RewardRequest createBonusRequest:
                createBonusRequest.Name = $"{TestConstants.TestPrefix}_{rewardType}_{Guid.NewGuid()}";
                createBonusRequest.Comments = TestConstants.DefaultPromotionComments;
                createBonusRequest.CustomerId ??= TestConstants.DefaultCustomerId;
                return createBonusRequest as T;

            default:
                return model;
        }
    }
}
