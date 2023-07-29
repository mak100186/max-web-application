namespace Kindred.Rewards.Core.Models.Messages.Reward.Parameters;

public class ProfitBoostParameters : RewardParametersBase
{
    public decimal? MaxStakeAmount { get; set; }
    public Dictionary<int, decimal> LegTable { get; set; }
}
