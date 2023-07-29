using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Helpers;

namespace Kindred.Rewards.Core.Models.Rn;

public class CombinationRn : BaseRn
{
    public string GuidValue { get; set; }
    public int? BetIndex { get; set; }
    public int? StageIndex { get; set; }

    public CombinationRn(Guid guidValue, int betIndex, int stageIndex)
        : base(DomainConstants.Rn.Namespaces.Ksp,
            DomainConstants.Rn.EntityTypes.Combination,
            DomainConstants.Rn.DefaultSchemeVersion)
    {
        EntitySegments.Add(guidValue.ToString());
        EntitySegments.Add(betIndex.ToString());
        EntitySegments.Add(stageIndex.ToString());

        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }

        GuidValue = guidValue.ToString();
        BetIndex = betIndex;
        StageIndex = stageIndex;
    }

    public CombinationRn(string rnString) : base(rnString)
    {
        var errors = Validate();

        if (errors.IsNotNullAndNotEmpty())
        {
            throw new AggregateException(errors.ToCsv());
        }

        GuidValue = EntitySegments[0];
        BetIndex = EntitySegments[1].ToInt();
        StageIndex = EntitySegments[2].ToInt();
    }

    private List<string> Validate()
    {
        List<string> errors = new();

        if (!EntityType.Equals(DomainConstants.Rn.EntityTypes.Combination, StringComparison.InvariantCultureIgnoreCase))
        {
            errors.Add("Invalid entity type");
        }

        if (EntitySegments.Count != 3)
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

        if (!EntitySegments[2].IsNumeric() || EntitySegments[2].IsDate())
        {
            errors.Add("Invalid stage index");
        }

        return errors;
    }

    public override string ToString() =>
        SemanticResourceNameGenerator.GenerateCombinationRn(Namespace, SchemeVersion.ToString(), GuidValue, BetIndex.ToString(), StageIndex.ToString());
}
