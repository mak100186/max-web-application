namespace Kindred.Rewards.Plugin.Claim.Exceptions;

public class ClaimNotFoundException : Exception
{
    public ClaimNotFoundException()
        : base("Could not find a claim with the provided claim Rn")
    {
    }

    public ClaimNotFoundException(string claimRn)
        : base($"Could not find a claim with the provided claim Rn {claimRn}")
    {
    }
}
