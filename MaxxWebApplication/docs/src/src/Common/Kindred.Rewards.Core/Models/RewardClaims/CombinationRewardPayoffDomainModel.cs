namespace Kindred.Rewards.Core.Models.RewardClaims;
public class CombinationRewardPayoffDomainModel
{
    public string BetRn { get; set; }
    public string CombinationRn { get; set; }
    public decimal BetPayoff { get; set; }
    public decimal BetStake { get; set; }
    public decimal CombinationPayoff { get; set; }
    public string ClaimInstanceId { get; set; }
    public DateTime CreatedOn { get; set; }
}
