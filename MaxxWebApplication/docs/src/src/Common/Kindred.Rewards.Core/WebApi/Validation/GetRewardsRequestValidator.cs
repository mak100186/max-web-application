using FluentValidation;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.WebApi.Enums;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Validation.Enums;

namespace Kindred.Rewards.Core.WebApi.Validation;

public class GetRewardsRequestValidator : ValidatorBase<GetRewardsRequest>
{
    /// <inheritdoc />
    public GetRewardsRequestValidator()
    {
        Transform(r => r.SortBy, sortBy => sortBy?.Replace("-", ""))
            .IsEnumName(typeof(SortableRewardFields), caseSensitive: false)
            .WithMessage(GenerateErrorMessage(RewardErrorCodes.RewardsAreOnlySortableOnCertainFields) +
                         Enum.GetValues<SortableRewardFields>().Select(x => x.ToString()).ToCsv());
    }
}
