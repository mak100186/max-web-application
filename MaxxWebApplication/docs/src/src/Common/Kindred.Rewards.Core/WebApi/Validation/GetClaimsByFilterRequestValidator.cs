using FluentValidation;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Validation.Enums;

namespace Kindred.Rewards.Core.WebApi.Validation;
public class GetClaimsByFilterRequestValidator : ValidatorBase<GetClaimsByFilterRequest>
{
    /// <inheritdoc />
    public GetClaimsByFilterRequestValidator()
    {
        Transform(r => r.SortBy, sortBy => sortBy?.Replace("-", ""))
            .IsEnumName(typeof(SortableRewardClaimFields), caseSensitive: false)
            .WithMessage(GenerateErrorMessage(ClaimErrorCodes.RewardClaimsAreOnlySortableOnCertainFields) +
                         Enum.GetValues<SortableRewardClaimFields>().Select(x => x.ToString()).ToCsv());
    }
}
