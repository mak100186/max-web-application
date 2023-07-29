using Kindred.Rewards.Core.Models.Messages.Reward.Parameters;

namespace Kindred.Rewards.Core.Models.Messages.Reward;

public class Reward : IRewardMessage
{
    public string RewardRn { get; set; }

    public string RewardId { get; set; }

    public string CustomerId { get; set; }

    public string RewardType { get; set; }

    public string RewardName { get; set; }
    public DomainRestriction DomainRestriction { get; set; }
    public string Name { get; set; }

    public string Comments { get; set; }

    public string RewardRules { get; set; }

    public RewardRestriction Restrictions { get; set; }

    public RewardParametersBase RewardParameters { get; set; }

    public Dictionary<string, string> Attributes { get; set; }

    public IEnumerable<string> Tags { get; set; }

    public string CountryCode { get; set; }

    public string Jurisdiction { get; set; }
    public string State { get; set; }

    public string Brand { get; set; }

    public string CurrencyCode { get; set; }

    public string Purpose { get; set; }

    public string SubPurpose { get; set; }

    public string CustomerFacingName { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Reward()
    {
        DomainRestriction = new();
    }
}
