using Kindred.Rewards.Core.WebApi.Payloads;

namespace Kindred.Rewards.Core.WebApi.Responses;

public class CustomerEntitlementsResponse
{
    public ICollection<RewardEntitlementApiModel> Entitlements { get; set; }
}
