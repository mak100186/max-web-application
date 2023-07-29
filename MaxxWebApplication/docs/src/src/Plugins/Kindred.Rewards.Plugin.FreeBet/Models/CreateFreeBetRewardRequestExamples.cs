using Kindred.Rewards.Core;
using Kindred.Rewards.Core.WebApi.Enums;
using Kindred.Rewards.Plugin.FreeBet.Services;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.FreeBet.Models;

public class CreateFreeBetRewardRequestExamples : IMultipleExamplesProvider<FreeBetRewardRequest>
{
    public IEnumerable<SwaggerExample<FreeBetRewardRequest>> GetExamples()
    {
        yield return SwaggerExample.Create(
            nameof(RewardType.Freebet),
            new FreeBetRewardRequest
            {
                Name = "name for bonus",
                Comments = "example_comments",
                CustomerId = "12345",
                CountryCode = "AUS",
                Restrictions =
                    new()
                    {
                        StartDateTime = DateTime.UtcNow.Date,
                        ExpiryDateTime = DateTime.UtcNow.AddDays(2).Date,
                        ClaimInterval = "0 0 0 ? * * *",
                        ClaimsPerInterval = 5,
                        TimezoneId = "UTC",
                        ClaimAllowedPeriod = "* * * ? * MON-FRI *"
                    },
                DomainRestriction = new()
                {
                    FilterContestStatuses = null,
                    FilterOutcomes = new List<string>(),
                    FilterContestTypes = new List<string>(),
                    FilterContestCategories = new List<string>(),
                    FilterContestRefs = new List<string>(),
                    MultiConfig = new()
                    {
                        FilterFormulae = FreeBetService.AllowedFormulae.ToArray(),
                        MinStages = 1,
                        MaxStages = DomainConstants.MaxNumberOfLegsInMulti,
                        MinCombinations = DomainConstants.MinNumberOfCombinationsInMulti,
                        MaxCombinations = DomainConstants.MaxNumberOfCombinationsInMulti
                    }
                },
                RewardParameters = new() { Amount = 10, MaxExtraWinnings = 10 },
                Tags = new List<string> { "DefaultTag" },
                Attributes = new(),
                Brand = "brand",
                CreatedBy = "pluginTool",
                UpdatedBy = "pluginTool",
                CustomerFacingName = "CustomerFacingName",
                Jurisdiction = "ANZAC",
                Purpose = PurposeType.Crm.ToString(),
                SubPurpose = SubPurposeType.Acquisition.ToString(),
                RewardRules = "rewardRules",
                State = "NSW"
            });
    }
}
