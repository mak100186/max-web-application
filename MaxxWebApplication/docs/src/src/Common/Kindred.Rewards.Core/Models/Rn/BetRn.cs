using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Helpers;

namespace Kindred.Rewards.Core.Models.Rn;

public class BetRn : BaseRn
{
    public string GuidValue { get; set; }
    public int? Index { get; set; }

    public BetRn(Guid guidValue, int index)
        : base(DomainConstants.Rn.Namespaces.Ksp,
            DomainConstants.Rn.EntityTypes.Bet,
            DomainConstants.Rn.DefaultSchemeVersion)
    {
        EntitySegments.Add(guidValue.ToString());
        EntitySegments.Add(index.ToString());

        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }

        GuidValue = guidValue.ToString();
        Index = index;
    }

    public BetRn(string rnString) : base(rnString)
    {
        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }

        GuidValue = EntitySegments[0];
        Index = EntitySegments[1].ToInt();
    }

    private List<string> Validate()
    {
        List<string> errors = new();

        if (!EntityType.Equals(DomainConstants.Rn.EntityTypes.Bet, StringComparison.InvariantCultureIgnoreCase))
        {
            errors.Add("Invalid entity type");
        }

        if (EntitySegments.Count != 2)
        {
            errors.Add("Extra or missing values");
            return errors;
        }

        if (!EntitySegments[0].IsGuid())
        {
            errors.Add("Invalid UUID");
        }


        if (!EntitySegments[1].IsNumeric() || EntitySegments[1].IsDate())
        {
            errors.Add("Invalid bet index");
        }

        return errors;
    }

    public override string ToString() =>
        SemanticResourceNameGenerator.GenerateBetRn(Namespace, SchemeVersion.ToString(), GuidValue, Index.ToString());
}
