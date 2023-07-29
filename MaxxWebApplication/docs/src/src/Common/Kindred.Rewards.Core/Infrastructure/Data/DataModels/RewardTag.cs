namespace Kindred.Rewards.Core.Infrastructure.Data.DataModels;
public class RewardTag
{
    public string RewardId { get; set; }

    public virtual Reward Reward { get; set; }

    public int TagId { get; set; }

    public virtual Tag Tag { get; set; }
}
