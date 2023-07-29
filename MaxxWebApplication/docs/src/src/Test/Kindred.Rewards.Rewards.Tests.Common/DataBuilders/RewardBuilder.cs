using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardConfiguration;

namespace Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

public class RewardBuilder : DataBuilderBase
{
    public static RewardTerms FreebetTerms => CreateReward(RewardType.Freebet, (Action<RewardDomainModel>)null).Terms;
    public static RewardTerms UniboostTerms => CreateReward(RewardType.Uniboost, (Action<RewardDomainModel>)null).Terms;
    public static RewardTerms ProfitBoostTerms => CreateReward(RewardType.Profitboost, (Action<RewardDomainModel>)null).Terms;

    public static T CreateReward<T>(RewardType rewardType, Action<T> callback)
        where T : RewardDomainModel
    {
        callback = SetCallbackIfNull(callback);

        var model = callback switch
        {
            Action<RewardDomainModel> rewardCallback => RewardBaseDomainModelBuilder.Create(rewardType, rewardCallback) as T,
            _ => throw new ArgumentException("Cannot determine the type of model")
        };

        return model
            .UpdateRewardBaseProps()
            .UpdateRewardClassSpecificProps(rewardType);
    }
}
