using System.ComponentModel.DataAnnotations;

using Kindred.Rewards.Core.WebApi.Enums;

namespace Kindred.Rewards.Core.WebApi.Payloads;

public class RewardApiModel
{
    public string CustomerId { get; set; }

    public string RewardRn { get; set; }

    public string RewardId { get; set; }

    public string? CurrencyCode { get; set; }
    public string RewardCategory { get; set; }

    public string RewardType { get; set; }

    public bool IsNameAutoGenerated { get; set; }

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

    public PurposeType Purpose { get; set; }

    public SubPurposeType SubPurpose { get; set; }

    [StringLength(200)]
    public string CustomerFacingName { get; set; }

    public IEnumerable<RewardTemplateApiModel> Templates { get; set; }


}
