namespace Kindred.Rewards.Core.WebApi.Payloads;

public class RewardEntitlementApiModel : RewardInfoApiModel
{
    public RewardEntitlementApiModel()
    {
        Reporting = new();
        DateTimeRestrictions = new();
        PlatformRestrictions = new();
        DomainRestriction = new();
    }

    public string Hash { get; set; }
    public SettlementApiModel SettlementTerms { get; set; }
    public ReportingApiModel Reporting { get; set; }
    public DateTimeRestrictionsApiModel DateTimeRestrictions { get; set; }
    public PlatformRestrictionsApiModel PlatformRestrictions { get; set; }
    public DomainRestrictionApiModel DomainRestriction { get; set; }
}
