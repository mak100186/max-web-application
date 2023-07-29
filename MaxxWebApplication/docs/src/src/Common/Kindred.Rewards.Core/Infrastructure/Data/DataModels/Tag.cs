namespace Kindred.Rewards.Core.Infrastructure.Data.DataModels;
public class Tag : BaseEditableDataModel<int>
{
    public string Name { get; set; }

    public string Comments { get; set; }

    public string DisplayStyle { get; set; }

    public virtual IReadOnlyCollection<RewardTag> RewardTags { get; set; }
}
