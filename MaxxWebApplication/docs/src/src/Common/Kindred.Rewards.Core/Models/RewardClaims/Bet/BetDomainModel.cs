namespace Kindred.Rewards.Core.Models.RewardClaims.Bet;

public class BetDomainModel
{
    public string Rn { get; set; }
    public string Status { get; set; }
    public decimal RequestedStake { get; set; }
    public string Formula { get; set; }
    public ICollection<CompoundStageDomainModel> Stages { get; set; }
}
