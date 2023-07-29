using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Rewards;

namespace Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

public static class PromotionDomainModelBuilder
{
    public static T Create<T>(RewardType rewardType = RewardType.Freebet, Action<RewardDomainModel> action = null)
        where T : RewardDomainModel, new()
    {
        var model = RewardBaseDomainModelBuilder.Create<RewardDomainModel>(rewardType);

        model.Templates = new List<RewardTemplateDomainModel>();

        action?.Invoke(model);

        return model as T;
    }
}
