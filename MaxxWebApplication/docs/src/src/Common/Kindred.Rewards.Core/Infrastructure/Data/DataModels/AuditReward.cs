namespace Kindred.Rewards.Core.Infrastructure.Data.DataModels;

public class AuditReward : BaseDataModel<int>
{
    public string RewardId { get; set; }

    public string Activity { get; set; }

    public string CreatedBy { get; set; }
}
