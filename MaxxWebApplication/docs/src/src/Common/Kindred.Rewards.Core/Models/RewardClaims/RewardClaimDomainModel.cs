using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models.RewardClaims.Bet;
using Kindred.Rewards.Core.Models.RewardConfiguration;

namespace Kindred.Rewards.Core.Models.RewardClaims;

public class RewardClaimDomainModel
{
    public int Id { get; set; }

    public string PromotionName { get; set; }
    public string RewardId { get; set; }

    public string InstanceId { get; set; }

    public string CustomerId { get; set; }

    public string Hash { get; set; }

    public RewardClaimStatus Status { get; set; }

    public RewardCategory Category { get; set; }

    public RewardType Type { get; set; }

    public RewardDomainModel Reward { get; set; } // TODO Replace all instances single reward details to use this
    public string CustomerFacingName { get; set; }

    public string CouponRn { get; set; }

    public string BetRn { get; set; }

    public BetDomainModel Bet { get; set; }

    public RewardTerms Terms { get; set; }

    public long IntervalId { get; set; }

    public int UsageId { get; set; }

    public int? ClaimLimit { get; set; }

    public decimal? RewardPayoutAmount { get; set; }

    public BetOutcome? BetOutcomeStatus { get; set; }

    public DateTime NextInterval { get; set; }

    public string CurrencyCode { get; set; }


    public string Purpose { get; set; }
    public string SubPurpose { get; set; }
    public string Comments { get; set; }
    public string RewardRules { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime UpdatedOn { get; set; }
    public ICollection<string> Tags { get; set; }

    public string CountryCode { get; set; }
    public string Jurisdiction { get; set; }
    public string State { get; set; }
    public string Brand { get; set; }
    public decimal? StakeDeduction { get; set; }
    public RewardClaimPayoffMetadataDomainModel PayoffMetadata { get; set; }
    public List<CombinationRewardPayoffDomainModel> CombinationRewardPayoffs { get; set; }
}
