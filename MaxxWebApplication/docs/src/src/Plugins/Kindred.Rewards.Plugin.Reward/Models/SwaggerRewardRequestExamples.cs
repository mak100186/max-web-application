using Kindred.Rewards.Core;
using Kindred.Rewards.Core.WebApi.Enums;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Plugin.Reward.Services;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.Reward.Models;

public class SwaggerRewardRequestExamples : IMultipleExamplesProvider<RewardRequest>
{
    public IEnumerable<SwaggerExample<RewardRequest>> GetExamples()
    {
        yield return SwaggerExample.Create(
            nameof(RewardType.Freebet),
            new RewardRequest
            {
                Name = "name for bonus",
                RewardType = nameof(RewardType.Freebet),
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
                        FilterFormulae = FreeBetCreationStrategy.AllowedFormulae.ToArray(),
                        MinStages = 1,
                        MaxStages = DomainConstants.MaxNumberOfLegsInMulti,
                        MinCombinations = DomainConstants.MinNumberOfCombinationsInMulti,
                        MaxCombinations = DomainConstants.MaxNumberOfCombinationsInMulti
                    }
                },
                RewardParameters = new FreeBetParametersApiModel { Amount = 10, MaxExtraWinnings = 10 },
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
            nameof(RewardType.Uniboost),
            new RewardRequest
            {
                Name = "name for bonus",
                Comments = "example_comments",
                CustomerId = "12345",
                RewardType = nameof(RewardType.Uniboost),
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
                DomainRestriction =
                    new()
                    {
                        FilterContestStatuses = null,
                        FilterOutcomes = new List<string>(),
                        FilterContestTypes = new List<string>(),
                        FilterContestCategories = new List<string>(),
                        FilterContestRefs = new List<string>()
                    },
                RewardParameters = new UniBoostParametersApiModel
                {
                    OddsLadderOffset = 3,
                    MaxStakeAmount = null,
                    MaxExtraWinnings = 10
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
            nameof(RewardType.UniboostReload),
            new RewardRequest
            {
                Name = "name for bonus",
                Comments = "example_comments",
                CustomerId = "12345",
                RewardType = nameof(RewardType.UniboostReload),
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
                DomainRestriction =
                    new()
                    {
                        FilterContestStatuses = ContestStatus.InPlay,
                        FilterOutcomes = new List<string>(),
                        FilterContestTypes = new List<string>(),
                        FilterContestCategories = new List<string>(),
                        FilterContestRefs = new List<string>()
                    },
                RewardParameters = new UniBoostReloadParametersApiModel
                {
                    OddsLadderOffset = 3,
                    MaxStakeAmount = null,
                    Reload = new() { MaxReload = 10, StopOnMinimumWinBets = 5 },
                    MaxExtraWinnings = 10
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
            $"{nameof(RewardType.Profitboost)} - Applicable to multis - Multiple legs configured",
            $"{nameof(RewardType.Profitboost)} - Applicable to multis - Multiple legs configured",
            "Creates a ProfitBoost that is applicable to multis including multis made up of singles. " +
            "Legs 1 - 2 have a 10% boost, 2 - 4 have 10%, 4 - 7 have 15%, 7 - 10 have 20% and 10 - 20 have 0%.",
            new RewardRequest
            {
                Name = "name for bonus",
                Comments = "example_comments",
                CustomerId = "12345",
                RewardType = nameof(RewardType.Profitboost),
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
                        FilterFormulae = ProfitBoostCreationStrategy.AllowedFormulae.ToArray(),
                        MinStages = DomainConstants.MinNumberOfLegsInMulti,
                        MaxStages = DomainConstants.MaxNumberOfLegsInMulti,
                        MinCombinations = DomainConstants.MinNumberOfCombinationsInMulti,
                        MaxCombinations = DomainConstants.MaxNumberOfCombinationsInMulti
                    }
                },
                RewardParameters = new ProfitBoostParametersApiModel
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
             new RewardRequest
             {
                 Name = "name for bonus",
                 Comments = "example_comments",
                 CustomerId = "12345",
                 RewardType = nameof(RewardType.Profitboost),
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
                         FilterFormulae = ProfitBoostCreationStrategy.AllowedFormulae.ToArray(),
                         MinStages = DomainConstants.MinNumberOfLegsInMulti,
                         MaxStages = DomainConstants.MaxNumberOfLegsInMulti,
                         MinCombinations = DomainConstants.MinNumberOfCombinationsInMulti,
                         MaxCombinations = DomainConstants.MaxNumberOfCombinationsInMulti
                     }
                 },
                 RewardParameters = new ProfitBoostParametersApiModel
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
            new RewardRequest
            {
                Name = "name for bonus",
                Comments = "example_comments",
                CustomerId = "12345",
                RewardType = nameof(RewardType.Profitboost),
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
                RewardParameters = new ProfitBoostParametersApiModel
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
