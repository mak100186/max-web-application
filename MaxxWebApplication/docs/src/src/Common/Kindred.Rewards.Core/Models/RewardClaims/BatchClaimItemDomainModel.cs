using Kindred.Rewards.Core.Models.RewardClaims.Bet;

namespace Kindred.Rewards.Core.Models.RewardClaims;

public class BatchClaimItemDomainModel
{
    public string RewardId { get; set; }
    public string Hash { get; set; }
    public BetDomainModel Bet { get; set; }
}
