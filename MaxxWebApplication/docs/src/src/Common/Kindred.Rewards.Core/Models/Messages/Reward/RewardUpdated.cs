namespace Kindred.Rewards.Core.Models.Messages.Reward;

public class RewardUpdated : Reward
{
    public string UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool IsCancelled { get; set; }

    public string CancellationReason { get; set; }
}
