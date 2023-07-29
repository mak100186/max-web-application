using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Helpers;

namespace Kindred.Rewards.Core.Models.Rn;

public class VariantRn : BaseRn
{
    public ContestKey ContestKey { get; set; }
    public string PropositionKey { get; set; }
    public string VariantKey { get; set; }

    public VariantRn(ContestKey contestKey, string propositionKey, string variantKey)
        : base(DomainConstants.Rn.Namespaces.Ksp,
            DomainConstants.Rn.EntityTypes.Variant,
            DomainConstants.Rn.DefaultSchemeVersion)
    {
        EntitySegments.Add(contestKey.Type);
        EntitySegments.Add(contestKey.DateTime);
        EntitySegments.Add(contestKey.Identifier);
        EntitySegments.Add(propositionKey);
        EntitySegments.Add(variantKey);

        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }

        ContestKey = contestKey;
        PropositionKey = propositionKey;
        VariantKey = variantKey;
    }

    public VariantRn(string rnString) : base(rnString)
    {
        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }

        ContestKey = new(EntitySegments[0], EntitySegments[1], EntitySegments[2]);
        PropositionKey = EntitySegments[3];
        VariantKey = EntitySegments[4];
    }

    private List<string> Validate()
    {
        List<string> errors = new();

        if (!EntityType.Equals(DomainConstants.Rn.EntityTypes.Variant, StringComparison.InvariantCultureIgnoreCase))
        {
            errors.Add("Invalid entity type");
        }

        if (EntitySegments.Count != 5)
        {
            errors.Add("Extra or missing values");
            return errors;
        }

        if (RegionHelper.IsIso2Valid(EntitySegments[4]))
        {
            errors.Add("Invalid variant key");
        }

        return errors;
    }

    public override string ToString() =>
        SemanticResourceNameGenerator.GenerateVariantRn(Namespace, SchemeVersion.ToString(), ContestKey.ToString(), PropositionKey, VariantKey);
}
