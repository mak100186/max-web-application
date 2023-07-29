namespace Kindred.Rewards.Core.Infrastructure.Data.DataModels;

public class CombinationRewardPayoff : BaseEditableDataModel<int>
{
    public string BetRn { get; set; }
    public string CombinationRn { get; set; }
    public decimal BetPayoff { get; set; }
    public decimal BetStake { get; set; }
    public decimal CombinationPayoff { get; set; }

    public string ClaimInstanceId { get; set; }
    public RewardClaim RewardClaim { get; set; }
}
