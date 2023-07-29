namespace Kindred.Rewards.Core.Models.RewardClaims;

public class BatchClaimDomainModel
{
    public string CustomerId { get; set; }

    public string CouponRn { get; set; }

    public List<BatchClaimItemDomainModel> Claims { get; set; }

    public string CurrencyCode { get; set; }
}
