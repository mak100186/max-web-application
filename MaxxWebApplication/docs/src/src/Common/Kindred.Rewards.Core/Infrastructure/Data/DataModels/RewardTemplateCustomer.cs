namespace Kindred.Rewards.Core.Infrastructure.Data.DataModels;

public class RewardTemplateCustomer : BaseDataModel<int>
{
    public string PromotionTemplateKey { get; set; }

    public string CustomerId { get; set; }
}
