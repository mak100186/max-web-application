namespace Kindred.Rewards.Core.Models.RewardClaims;

public class ClaimResultDomainModel
{
    public bool Success => string.IsNullOrWhiteSpace(ErrorMessage);

    public string ErrorMessage { get; set; }

    public RewardClaimDomainModel Claim { get; set; }
}
