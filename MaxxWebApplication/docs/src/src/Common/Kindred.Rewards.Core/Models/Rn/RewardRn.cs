using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Helpers;

namespace Kindred.Rewards.Core.Models.Rn;

public class RewardRn : BaseRn
{
    public string GuidValue { get; set; }

    public RewardRn(Guid guidValue)
        : base(DomainConstants.Rn.Namespaces.Ksp,
            DomainConstants.Rn.EntityTypes.Reward,
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

    public RewardRn(string rnString) : base(rnString)
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

        if (!EntityType.Equals(DomainConstants.Rn.EntityTypes.Reward, StringComparison.InvariantCultureIgnoreCase))
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
        SemanticResourceNameGenerator.GenerateRewardRn(Namespace, SchemeVersion.ToString(), GuidValue);
}
