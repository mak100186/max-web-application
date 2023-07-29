namespace Kindred.Rewards.Core.Infrastructure.Data.DataModels;

public class RewardTemplate : BaseEditableDataModel<int>
{
    public string Key { get; set; }
    public string Comments { get; set; }
    public string Title { get; set; }
    public bool Enabled { get; set; }
    public List<RewardTemplateReward> RewardTemplateReward { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}
