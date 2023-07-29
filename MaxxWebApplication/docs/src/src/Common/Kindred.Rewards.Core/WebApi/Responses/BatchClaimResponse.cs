using Kindred.Rewards.Core.WebApi.Payloads;

namespace Kindred.Rewards.Core.WebApi.Responses;

public class BatchClaimResponse
{
    public List<BatchClaimApiModel> Responses { get; set; }
}
