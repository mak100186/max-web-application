using Kindred.Rewards.Core.WebApi.Payloads;

namespace Kindred.Rewards.Core.WebApi.Requests;

public class BatchClaimRequest
{
    public BatchClaimRequest()
    {
        Claims = new();
    }

    public string CustomerId { get; set; }

    public string CouponRn { get; set; }

    public List<ClaimApiModel> Claims { get; set; }

    public string CurrencyCode { get; set; }
}
