using System.Security.Cryptography;

using Kindred.Rewards.Core.Models.Rewards;
using Kindred.Rewards.Core.WebApi.Requests;

namespace Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

public class PromotionTemplateBuilder : DataBuilderBase
{
    public static RewardTemplateDomainModel Create(Action<RewardTemplateDomainModel> callback = null)
    {
        var model = new RewardTemplateDomainModel
        {
            Id = RandomNumberGenerator.GetInt32(1, 100),
            TemplateKey = $"{TestConstants.TestPrefix}_Test_Template_{Guid.NewGuid().GetHashCode()}",
            Comments = "TestPromotionTemplate",
            Enabled = true,
            Title = Guid.NewGuid().ToString(),
            CreatedBy = "myUser",
            UpdatedBy = "myUser2"
        };

        callback?.Invoke(model);

        return model;
    }

    public static CreateRewardTemplateRequest CreateCreationRequest(Action<CreateRewardTemplateRequest> callback = null)
    {
        var model = new CreateRewardTemplateRequest
        {
            TemplateKey = Guid.NewGuid().ToString(),
            Comments = Guid.NewGuid().ToString(),
            Title = Guid.NewGuid().ToString()
        };

        callback?.Invoke(model);

        return model;
    }
}
