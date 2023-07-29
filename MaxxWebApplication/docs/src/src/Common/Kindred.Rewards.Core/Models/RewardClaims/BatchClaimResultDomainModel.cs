namespace Kindred.Rewards.Core.Models.RewardClaims;

public class BatchClaimResultDomainModel
{
    public bool AllRewardsFound { get; set; }

    public List<ClaimResultDomainModel> ClaimResults { get; set; }
}
