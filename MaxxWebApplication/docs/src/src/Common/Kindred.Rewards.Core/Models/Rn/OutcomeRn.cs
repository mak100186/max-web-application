using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Helpers;

namespace Kindred.Rewards.Core.Models.Rn;

public class OutcomeRn : BaseRn
{
    public ContestKey ContestKey { get; set; }
    public string PropositionKey { get; set; }
    public string VariantKey { get; set; }
    public string OptionKey { get; set; }

    public OutcomeRn(ContestKey contestKey, string propositionKey, string variantKey, string optionKey)
        : base(DomainConstants.Rn.Namespaces.Ksp,
            DomainConstants.Rn.EntityTypes.Outcome,
            DomainConstants.Rn.DefaultSchemeVersion)
    {
        EntitySegments.Add(contestKey.Type);
        EntitySegments.Add(contestKey.DateTime);
        EntitySegments.Add(contestKey.Identifier);
        EntitySegments.Add(propositionKey);
        EntitySegments.Add(variantKey);
        EntitySegments.Add(optionKey);

        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }

        ContestKey = contestKey;
        PropositionKey = propositionKey;
        VariantKey = variantKey;
        OptionKey = optionKey;
    }

    public OutcomeRn(string rnString) : base(rnString)
    {
        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }

        ContestKey = new(EntitySegments[0], EntitySegments[1], EntitySegments[2]);
        PropositionKey = EntitySegments[3];
        VariantKey = EntitySegments[4];
        OptionKey = EntitySegments[5];
    }

    private List<string> Validate()
    {
        List<string> errors = new();

        if (!EntityType.Equals(DomainConstants.Rn.EntityTypes.Outcome, StringComparison.InvariantCultureIgnoreCase))
        {
            errors.Add("Invalid entity type");
        }

        if (EntitySegments.Count != 6)
        {
            errors.Add("Extra or missing values");
            return errors;
        }

        //todo: add validations on propositionKey, variantKey and optionKey

        return errors;
    }

    public override string ToString() =>
        SemanticResourceNameGenerator.GenerateOutcomeRn(Namespace, SchemeVersion.ToString(), ContestKey.ToString(), PropositionKey, VariantKey, OptionKey);
}
