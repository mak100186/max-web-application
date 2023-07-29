namespace Kindred.Rewards.Core.Infrastructure.Data.DataModels;

public class RewardTemplateReward
{
    public int RewardTemplateId { get; set; }

    public RewardTemplate RewardTemplate { get; set; }

    public string RewardRn { get; set; }

    public Reward Reward { get; set; }
}
