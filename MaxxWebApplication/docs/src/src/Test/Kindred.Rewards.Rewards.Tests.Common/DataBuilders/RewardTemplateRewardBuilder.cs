using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Models.RewardConfiguration;

using Newtonsoft.Json;

namespace Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

public static class RewardTemplateRewardBuilder
{
    public static RewardTemplateReward Create(Action<RewardTemplateReward> callback = null)
    {
        var model = new RewardTemplateReward
        {
            Reward = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Comments = "Comments" + Guid.NewGuid(),
                RewardType = RewardType.Freebet.ToString(),
                TermsJson = JsonConvert.SerializeObject(new RewardTerms
                {
                    Restrictions = new()
                    {
                        StartDateTime = DateTime.Now
                    }
                })
            }
        };

        callback?.Invoke(model);

        return model;
    }
}
