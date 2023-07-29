using Kindred.Rewards.Core;
using Kindred.Rewards.Core.WebApi.Enums;
using Kindred.Rewards.Plugin.UniBoostReload.Services;

using Swashbuckle.AspNetCore.Filters;

using PurposeType = Kindred.Rewards.Core.Enums.PurposeType;
using SubPurposeType = Kindred.Rewards.Core.Enums.SubPurposeType;

namespace Kindred.Rewards.Plugin.UniBoostReload.Models;

public class CreateUniBoostReloadRewardRequestExamples : IMultipleExamplesProvider<UniBoostReloadRewardRequest>
{
    public IEnumerable<SwaggerExample<UniBoostReloadRewardRequest>> GetExamples()
    {
        yield return SwaggerExample.Create(
            nameof(RewardType.Uniboost),
            new UniBoostReloadRewardRequest
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
                        FilterFormulae = UniBoostReloadService.AllowedFormulae.ToArray(),
                        MinStages = 1,
                        MaxStages = DomainConstants.MaxNumberOfLegsInMulti,
                        MinCombinations = DomainConstants.MinNumberOfCombinationsInMulti,
                        MaxCombinations = DomainConstants.MaxNumberOfCombinationsInMulti
                    }
                },
                RewardParameters = new()
                {
                    OddsLadderOffset = 3,
                    MaxStakeAmount = 10,
                    MaxExtraWinnings = 20,
                    Reload = new()
                    {
                        MaxReload = 2,
                        StopOnMinimumWinBets = 1
                    }
                },
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
