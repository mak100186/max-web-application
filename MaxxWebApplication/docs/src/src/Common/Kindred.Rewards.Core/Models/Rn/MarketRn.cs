using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Helpers;

namespace Kindred.Rewards.Core.Models.Rn;

public class MarketRn : BaseRn
{
    public ContestKey ContestKey { get; set; }
    public string PropositionKey { get; set; }
    public string Tenant { get; set; }

    public MarketRn(ContestKey contestKey, string propositionKey, string tenant)
        : base(DomainConstants.Rn.Namespaces.Ksp,
            DomainConstants.Rn.EntityTypes.Market,
            DomainConstants.Rn.DefaultSchemeVersion)
    {
        EntitySegments.Add(contestKey.Type);
        EntitySegments.Add(contestKey.DateTime);
        EntitySegments.Add(contestKey.Identifier);
        EntitySegments.Add(propositionKey);

        if (!string.IsNullOrWhiteSpace(tenant))
        {
            EntitySegments.Add(tenant);
        }

        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }

        ContestKey = contestKey;
        PropositionKey = propositionKey;
        Tenant = tenant;
    }

    public MarketRn(string rnString) : base(rnString)
    {
        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }

        ContestKey = new(EntitySegments[0], EntitySegments[1], EntitySegments[2]);
        PropositionKey = EntitySegments[3];

        if (EntitySegments.Count > 4)
        {
            Tenant = EntitySegments[4];
        }
    }

    private List<string> Validate()
    {
        List<string> errors = new();

        if (!EntityType.Equals(DomainConstants.Rn.EntityTypes.Market, StringComparison.InvariantCultureIgnoreCase))
        {
            errors.Add("Invalid entity type");
        }

        if (EntitySegments.Count < 4 || EntitySegments.Count > 5)
        {
            errors.Add("Extra or missing values");
            return errors;
        }

        if (EntitySegments.Count == 5 && !RegionHelper.IsIso2Valid(EntitySegments[4]))
        {
            errors.Add("Invalid tenant code");
        }

        return errors;
    }

    public override string ToString() =>
        SemanticResourceNameGenerator.GenerateMarketRn(Namespace, SchemeVersion.ToString(), ContestKey.ToString(), PropositionKey, Tenant);
}
