using Kindred.Rewards.Core;
using Kindred.Rewards.Core.WebApi.Enums;
using Kindred.Rewards.Plugin.ProfitBoost.Services;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.ProfitBoost.Models;

public class CreateProfitBoostRewardRequestExamples : IMultipleExamplesProvider<ProfitBoostRewardRequest>
{
    public IEnumerable<SwaggerExample<ProfitBoostRewardRequest>> GetExamples()
    {
        yield return SwaggerExample.Create(
            $"{nameof(RewardType.Profitboost)} - Applicable to multis - Multiple legs configured",
            $"{nameof(RewardType.Profitboost)} - Applicable to multis - Multiple legs configured",
            "Creates a ProfitBoost that is applicable to multis including multis made up of singles. " +
            "Legs 1 - 2 have a 10% boost, 2 - 4 have 10%, 4 - 7 have 15%, 7 - 10 have 20% and 10 - 20 have 0%.",
            new ProfitBoostRewardRequest
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
                    FilterContestStatuses = ContestStatus.PreGame,
                    FilterOutcomes = new List<string>(),
                    FilterContestTypes = new List<string>(),
                    FilterContestCategories = new List<string>(),
                    FilterContestRefs = new List<string>(),
                    MultiConfig = new()
                    {
                        FilterFormulae = ProfitBoostService.AllowedFormulae.ToArray(),
                        MinStages = DomainConstants.MinNumberOfLegsInMulti,
                        MaxStages = DomainConstants.MaxNumberOfLegsInMulti,
                        MinCombinations = DomainConstants.MinNumberOfCombinationsInMulti,
                        MaxCombinations = DomainConstants.MaxNumberOfCombinationsInMulti
                    }
                },
                RewardParameters = new()
                {
                    MaxStakeAmount = null,
                    LegTable = new()
                    {
                        { 1, 10 },
                        { 2, 10 },
                        { 4, 15 },
                        { 7, 20 },
                        { 10, 0 }
                    },
                    MaxExtraWinnings = null
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

        yield return SwaggerExample.Create(
            $"{nameof(RewardType.Profitboost)} - Applicable to multis - One Leg Configured",
            $"{nameof(RewardType.Profitboost)} - Applicable to multis - One Leg Configured",
            "Creates a ProfitBoost that is applicable to multis including multis made up of singles. " +
            "Legs 2 onwards are deduced by Rewards i.e. legs 2 onwards have a 10% boost applied.",
            new ProfitBoostRewardRequest
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
                    FilterContestStatuses = ContestStatus.PreGame,
                    FilterOutcomes = new List<string>(),
                    FilterContestTypes = new List<string>(),
                    FilterContestCategories = new List<string>(),
                    FilterContestRefs = new List<string>(),
                    MultiConfig = new()
                    {
                        FilterFormulae = ProfitBoostService.AllowedFormulae.ToArray(),
                        MinStages = DomainConstants.MinNumberOfLegsInMulti,
                        MaxStages = DomainConstants.MaxNumberOfLegsInMulti,
                        MinCombinations = DomainConstants.MinNumberOfCombinationsInMulti,
                        MaxCombinations = DomainConstants.MaxNumberOfCombinationsInMulti
                    }
                },
                RewardParameters = new()
                {
                    LegTable = new()
                    {
                        { 2, 10 }
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

        yield return SwaggerExample.Create(
            $"{nameof(RewardType.Profitboost)} - Applicable to singles",
            $"{nameof(RewardType.Profitboost)} - Applicable to singles",
            "Creates a ProfitBoost that is applicable to single bets.",
            new ProfitBoostRewardRequest
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
                    FilterContestStatuses = ContestStatus.PreGame,
                    FilterOutcomes = new List<string>(),
                    FilterContestTypes = new List<string>(),
                    FilterContestCategories = new List<string>(),
                    FilterContestRefs = new List<string>(),
                    MultiConfig = new()
                    {
                        FilterFormulae = new List<string> { "singles" },
                        MinStages = 1,
                        MaxStages = 1,
                        MinCombinations = 1,
                        MaxCombinations = 1
                    }
                },
                RewardParameters = new()
                {
                    LegTable = new() { { 1, 10 } }
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
