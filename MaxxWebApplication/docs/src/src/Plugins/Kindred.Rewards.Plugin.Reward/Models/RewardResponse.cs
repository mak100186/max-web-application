using System.ComponentModel.DataAnnotations;

using Kindred.Rewards.Core.WebApi.Payloads;

namespace Kindred.Rewards.Plugin.Reward.Models;

public interface IRewardResponse
{
    string RewardRn { get; set; }
    string RewardId { get; set; }
}

public class RewardResponse : IRewardResponse
{
    public string CustomerId { get; set; }

    public string RewardRn { get; set; }
    public string RewardId { get; set; }

    public string RewardCategory { get; set; }

    public string? CurrencyCode { get; set; }

    public string RewardType { get; set; }

    public DomainRestrictionApiModel DomainRestriction { get; set; }
    public RewardRestrictionApiModel Restrictions { get; set; }

    public SettlementApiModel SettlementTerms { get; set; }

    public RewardParameterApiModelBase RewardParameters { get; set; }

    public Dictionary<string, string> Attributes { get; set; }

    public bool IsCancelled { get; set; }

    public string CancellationReason { get; set; }

    public string Name { get; set; }

    public string Comments { get; set; }

    public string RewardRules { get; set; }

    public IEnumerable<string> Tags { get; set; }

    [StringLength(3)]
    public string CountryCode { get; set; }

    public string Jurisdiction { get; set; }
    public string State { get; set; }
    public string Brand { get; set; }

    public string Purpose { get; set; }

    public string SubPurpose { get; set; }

    [StringLength(200)]
    public string CustomerFacingName { get; set; }

    public IEnumerable<RewardTemplateApiModel> Templates { get; set; }

    public string CreatedBy { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}
