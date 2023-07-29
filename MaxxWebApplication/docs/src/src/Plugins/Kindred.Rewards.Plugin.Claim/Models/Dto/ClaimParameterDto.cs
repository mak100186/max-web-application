using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.RewardClaims.Bet;

namespace Kindred.Rewards.Plugin.Claim.Models.Dto;

public class ClaimParameterDto
{
    public decimal CombinationStake { get; set; }
    public string Formula { get; set; }
    public IEnumerable<CompoundStageDomainModel> CombinationStages { get; set; }
    public RewardClaimDomainModel Claim { get; set; }
}
