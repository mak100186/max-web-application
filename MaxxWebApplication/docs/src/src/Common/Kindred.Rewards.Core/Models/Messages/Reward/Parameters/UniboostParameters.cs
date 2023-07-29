namespace Kindred.Rewards.Core.Models.Messages.Reward.Parameters;

public class UniBoostParameters : RewardParametersBase
{
    public decimal? MaxStakeAmount { get; set; }
    public int OddsLadderOffset { get; set; }
}
