using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Helpers;

namespace Kindred.Rewards.Core.Models.Rn;

public class ClaimRn : BaseRn
{
    public string GuidValue { get; set; }

    public ClaimRn(Guid guidValue)
        : base(DomainConstants.Rn.Namespaces.Ksp,
            DomainConstants.Rn.EntityTypes.Claim,
            DomainConstants.Rn.DefaultSchemeVersion)
    {
        EntitySegments.Add(guidValue.ToString());

        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }

        GuidValue = guidValue.ToString();
    }

    public ClaimRn(string rnString) : base(rnString)
    {
        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }

        GuidValue = EntitySegments[0];
    }

    private List<string> Validate()
    {
        List<string> errors = new();

        if (!EntityType.Equals(DomainConstants.Rn.EntityTypes.Claim, StringComparison.InvariantCultureIgnoreCase))
        {
            errors.Add("Invalid entity type");
        }

        if (EntitySegments.Count != 1)
        {
            errors.Add("Extra or missing values");
            return errors;
        }

        if (!EntitySegments[0].IsGuid())
        {
            errors.Add("Invalid UUID");
        }

        return errors;
    }

    public override string ToString() =>
        SemanticResourceNameGenerator.GenerateClaimRn(Namespace, SchemeVersion.ToString(), GuidValue);
}
