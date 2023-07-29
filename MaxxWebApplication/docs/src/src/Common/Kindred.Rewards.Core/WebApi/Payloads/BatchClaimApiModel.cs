using Kindred.Rewards.Core.WebApi.Responses;

namespace Kindred.Rewards.Core.WebApi.Payloads;

public class BatchClaimApiModel
{
    // Whether this particular reward is eligible for Claiming.  For batch claims,
    // the operation is atomic so a Success indicates *potential* success in this scenario
    public bool Success { get; set; }

    public string ErrorMessage { get; set; }

    public ClaimEntitlementResponse Claim { get; set; }
}
