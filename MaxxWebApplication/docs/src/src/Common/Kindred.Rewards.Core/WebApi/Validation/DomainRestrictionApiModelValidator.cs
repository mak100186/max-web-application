using Kindred.Rewards.Core.WebApi.Payloads;

namespace Kindred.Rewards.Core.WebApi.Validation;

public class DomainRestrictionApiModelValidator : ValidatorBase<DomainRestrictionApiModel>
{
    public DomainRestrictionApiModelValidator()
    {
        RuleFor(x => x.OddLimits)
            .SetValidator(new OddsLimitValidator());
    }
}
