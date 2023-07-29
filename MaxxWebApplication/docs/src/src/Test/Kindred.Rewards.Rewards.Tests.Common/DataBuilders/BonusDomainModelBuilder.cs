using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Rewards;

namespace Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

public static class BonusDomainModelBuilder
{
    public static T Create<T>(RewardType rewardType = RewardType.Freebet, Action<RewardDomainModel> action = null)
        where T : RewardDomainModel, new()
    {
        var model = RewardBaseDomainModelBuilder.Create<RewardDomainModel>(rewardType);

        model.CustomerId = null;
        model.UpdatedOn = DateTime.Now;
        model.StatusUpdates = new List<RewardStatusDomainModel>();

        action?.Invoke(model);

        return model as T;
    }
}
