using System.Web;

using AutoMapper;

using FluentAssertions;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Infrastructure.Kafka;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Messages.Reward;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.Models.Rn;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Responses;
using Kindred.Rewards.Plugin.Reward.Models;
using Kindred.Rewards.Plugin.Reward.Services;
using Kindred.Rewards.Rewards.FunctionalTests.Common;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Extensions;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Helpers;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Setup;
using Kindred.Rewards.Rewards.Tests.Common;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.Tests.Common.Extensions;
using Kindred.Rewards.Rewards.Tests.Common.Extensions.DataModifiers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using NUnit.Framework;

using TechTalk.SpecFlow;


namespace Kindred.Rewards.Rewards.FunctionalTests.Steps;

[Binding]
public class RewardStepsBase<TRwd, TGetRwdResp, TGetRwdsReq, TGetRwdsResp, TCreateRwdReq> : AcceptanceScenarioBase
    where TRwd : RewardApiModel
    where TGetRwdsReq : GetRewardsRequest
    where TCreateRwdReq : RewardRequest, new()
    where TGetRwdResp : GetRewardResponse
    where TGetRwdsResp : GetRewardsResponse
{
    private const string CancellationReason = "AcceptanceTest";
    private const string DefaultCustomerId = "12345";
    protected readonly IApiClient ApiClient;
    private TCreateRwdReq _createPromotionRequest;
    private TRwd _createPromotionResponse;
    protected TGetRwdResp GetRewardResponse;
    protected TGetRwdsResp GetRewardsResponse;
    protected IMapper Mapper;
    protected TRwd TestReward;
    protected TRwd PatchBonusResponse;

    protected RewardStepsBase(ScenarioContext scenarioContext, FeatureContext featureContext) : base(scenarioContext, featureContext)
    {
        try
        {
            TestRewards ??= new();
        }
        catch (Exception)
        {
            TestRewards = new();
        }

        ApiClient = new ApiClient("rewards", RestClientFactory, scenarioContext, SerializerSettings.GetJsonSerializerSettings);
        Mapper = ServiceProvider.GetService<IMapper>();
    }

    protected Dictionary<string, TRwd> TestRewards
    {
        get
        {
            if (!FeatureContext.TryGetValue(nameof(PromotionTemplateSteps.TestRewards), out Dictionary<string, TRwd> returnValue))
            {
                returnValue = new();
            }

            return returnValue;
        }

        set => FeatureContext.Set(value, nameof(PromotionTemplateSteps.TestRewards));
    }

    private static void SetRewardClassSpecificProps<T>(T obj, TableRow row)
    {
        if (obj is not RewardRequest cpr)
        {
            return;
        }

        var promoName = row.GetValue("name") ?? row.GetValue("promo_name");
        cpr.Name = CommonSteps.Prefix(promoName);

        var customerId = row.GetValue("customer_id");
        if (!string.IsNullOrWhiteSpace(customerId))
        {
            cpr.CustomerId = CommonSteps.PrefixCustomerId(customerId).ToString();
        }
    }

    protected async Task GivenIHaveRewardsInTheSystem(Table table, bool isBonus = true)
    {
        var testPromotions = new Dictionary<string, TRwd>();

        foreach (var row in table.Rows)
        {
            var promoName = row[TableColumns.Name];
            var rewardTypeString = row[TableColumns.RewardType];
            var rewardType = Enum.Parse<RewardType>(rewardTypeString, true);
            var rewardTypeApi = Enum.Parse<Core.WebApi.Enums.RewardType>(rewardTypeString, true);

            var request = RewardRequestBuilder.CreateRewardRequestForType<TCreateRwdReq>(rewardType, rewardRequest =>
            {
                rewardRequest.CustomerId = row.GetValueFromRow<string>(TableColumns.CustomerId, null);
                var mso = row.GetValueFromRow<decimal?>(TableColumns.MinimumStageOdds, null);
                var mco = row.GetValueFromRow<decimal?>(TableColumns.MinimumCompoundOdds, null);

                rewardRequest
                    .WithMinimumOdds(mso, mco)
                    .WithMaxExtraWinnings(row.GetValueFromRow<decimal?>(TableColumns.MaxExtraWinnings, null))
                    .WithAmount(row.GetValueFromRow<decimal>(TableColumns.Amount, 0))
                    .WithAllowedContestStatus(row.GetValueFromRow<string>(TableColumns.AllowedContestStatus, default))
                    .WithStartDaysFromNow(row.GetValueFromRow<string>(TestConstants.StartDaysFromNow, default))
                    .WithReload(row.GetValueFromRow<int?>(TableColumns.MaxReload, null), row.GetValueFromRow<int>("stopOnMinimumWinBets", 2));

                rewardRequest.Restrictions.ClaimsPerInterval = row.GetValueFromRow<int?>(TableColumns.ClaimsPerInterval, 1);
            });

            request.RewardType = rewardTypeApi.ToString();

            SetRewardClassSpecificProps(request, row);

            if (!isBonus)
            {
                request.CustomerId = null;
            }

            if (!request.Restrictions.StartDateTime.HasValue)
            {
                request.Restrictions.StartDateTime = row.GetValueFromRow<string>(TableColumns.StartDateTime, default).ToNullableDateTime();
            }

            if (row.ContainsKeyThatIsNotEmpty(TestConstants.ExpiryDaysFromNow)
                && !string.Equals(TimeZoneInfo.Local.Id, TestConstants.EtcUtcTimeZone, StringComparison.InvariantCultureIgnoreCase))
            {
                if (row[TestConstants.ExpiryDaysFromNow] == TestConstants.TimeInNextDaylightSavingTimeShift)
                {
                    request.Restrictions.ExpiryDateTime = TimeZoneExtensions.GetLocalDaylightTransitionStartDate(DateTime.UtcNow.Year).AddSeconds(2)
                        .AsUtcDateTime();

                    if (request.Restrictions.ExpiryDateTime < DateTime.UtcNow)
                    {
                        request.Restrictions.ExpiryDateTime = TimeZoneExtensions.GetLocalDaylightTransitionStartDate(DateTime.UtcNow.AddYears(1).Year)
                            .AddSeconds(2)
                            .AsUtcDateTime();
                    }
                }
                else
                {
                    request.Restrictions.ExpiryDateTime = string.Equals(TimeZoneInfo.Local.Id, TestConstants.EtcUtcTimeZone, StringComparison.InvariantCultureIgnoreCase)
                        ? null
                        : row.ContainsKey(TestConstants.ExpiryDaysFromNow)
                            ? DateTime.UtcNow.AddDays(
                                int.Parse(row[TestConstants.ExpiryDaysFromNow]))
                            : null;
                }
            }

            if (row.ContainsKeyThatIsNotEmpty(RewardParameterKey.OddsLadderOffset))
            {
                var uniboostParametersApiModel = request.RewardParameters as UniBoostParametersApiModel;
                uniboostParametersApiModel.OddsLadderOffset = int.Parse(row[RewardParameterKey.OddsLadderOffset]);
            }

            if (row.ContainsKeyThatIsNotEmpty(RewardParameterKey.LegTable))
            {
                var legTable = JsonConvert.DeserializeObject<Dictionary<string, string>>(row[RewardParameterKey.LegTable]);

                var profitBoostRewardParams = request.RewardParameters as ProfitBoostParametersApiModel;
                profitBoostRewardParams.LegTable = legTable
                    .ToDictionary(x =>
                        int.Parse(x.Key), p => decimal.Parse(p.Value));
            }

            request.DomainRestriction.MultiConfig.MaxCombinations = row.GetValueFromRow<int?>(RewardParameterKey.MaxCombinations, default);
            request.DomainRestriction.MultiConfig.MinCombinations = row.GetValueFromRow<int?>(RewardParameterKey.MinCombinations, default);
            request.DomainRestriction.MultiConfig.MaxStages = row.GetValueFromRow<int?>(RewardParameterKey.MaxStages, default);
            request.DomainRestriction.MultiConfig.MinStages = row.GetValueFromRow<int?>(RewardParameterKey.MinStages, default);
            if (row.ContainsKey(RewardParameterKey.AllowedFormulae))
            {
                request.DomainRestriction.MultiConfig.FilterFormulae =
                    row[RewardParameterKey.AllowedFormulae].Split(",");
            }

            request.CountryCode = row.GetValueFromRow(TableColumns.CountryCode, DomainConstants.DefaultThreeLetterCountryCode);
            request.Jurisdiction = row.GetValueFromRow<string>(TableColumns.Jurisdiction, default);
            request.Comments = row.GetValueFromRow<string>(TableColumns.Comments, default);

            request.CreatedBy = row.GetValueFromRow<string>(TableColumns.CreatedBy, default);
            request.UpdatedBy = row.GetValueFromRow<string>(TableColumns.UpdatedBy, default);

            var reward = await ApiClient.PostAsync<TCreateRwdReq, TRwd>(request);

            EnsureSuccessResponse(reward);

            testPromotions.Add(CommonSteps.Prefix(promoName), reward);
        }

        TestRewards = testPromotions;
    }

    protected async Task CreateReward(TableRow row)
    {
        var rewardType = Enum.Parse<RewardType>(row[TableColumns.RewardType], true);

        var request = RewardRequestBuilder.CreateRewardRequestForType<TCreateRwdReq>(rewardType, rewardRequest =>
        {
            rewardRequest.CustomerId = row.GetValueFromRow<string>(TableColumns.CustomerId, null);
        });

        await ApiClient.PostAsync<TCreateRwdReq, TRwd>(request);
    }

    protected void ExpectRewardCreatedMessageWithRewardTypeAndCustomerId(string rewardType, string customerId)
    {
        KafkaConsumerManager.Wait<RewardCreated>(DomainConstants.TopicProfileRewardUpdates, (c) =>
        {
            var message = c.Value;
            return message.CustomerId == customerId && message.RewardType == rewardType;
        });
    }

    private void EnsureSuccessResponse(TRwd reward)
    {
        if (ScenarioContext.ContainsKey(TestConstants.RestResponseSharedError)
            && ScenarioContext[TestConstants.RestResponseSharedError] != null)
        {
            throw new(ScenarioContext[TestConstants.RestResponseSharedError].ToString());
        }

        reward.ShouldNotBeNull();
        reward.RewardRn.Should().NotBeNullOrEmpty();
        reward.RewardId.Should().NotBeNullOrEmpty();
        reward.IsCancelled.Should().BeFalse();
    }

    protected async Task WhenISubmitGetRewardRequestFor(string promoName)
    {
        var promotion = CommonSteps.Prefix(promoName);
        GetRewardResponse = await ApiClient.GetAsync<TGetRwdResp>(HttpUtility.UrlEncode(TestRewards[promotion].RewardRn));
    }

    protected async Task WhenISubmitGetRewardRequestForRewardRn(string rewardRn)
    {
        GetRewardResponse = await ApiClient.GetAsync<TGetRwdResp>(HttpUtility.UrlEncode(rewardRn));
    }

    protected void ThenTheGetRewardResponseShouldReturnReward(string promoName)
    {
        GetRewardResponse
            .Should()
            .BeEquivalentTo(new
            {
                TestRewards[CommonSteps.Prefix(promoName)].CustomerId,
                Reporting = new
                {
                    TestRewards[CommonSteps.Prefix(promoName)].Name,
                    TestRewards[CommonSteps.Prefix(promoName)].IsCancelled,
                    TestRewards[CommonSteps.Prefix(promoName)].CustomerFacingName
                },
                Rn = TestRewards[CommonSteps.Prefix(promoName)].RewardRn,
                TestRewards[CommonSteps.Prefix(promoName)].RewardId,
                PlatformRestrictions = new
                {
                    TestRewards[CommonSteps.Prefix(promoName)].State
                }
            });
    }

    private void ValidateExpectedRewards(IEnumerable<string> expectedRewards)
    {
        GetRewardsResponse.ShouldNotBeNull();
        GetRewardsResponse.Items.Select(p => p.Rn).Should().Contain(expectedRewards);
    }

    private TGetRwdsReq CreateGetRewardsReq(bool includeCancelled, bool includeExpired, bool includeActive, string name, string customerId, DateTimeOffset? updateTimeFrom = null, DateTimeOffset? updateTimeTo = null,
        string country = null)
    {
        var request = new GetRewardsRequest
        {
            IncludeCancelled = includeCancelled,
            IncludeExpired = includeExpired,
            IncludeActive = includeActive,
            CustomerId = customerId,
            Name = name,
            UpdatedDateFromUtc = updateTimeFrom,
            UpdatedDateToUtc = updateTimeTo,
            Country = country
        };

        return (TGetRwdsReq)request;
    }

    protected async Task WhenISubmitGetRewardsRequestForFollowingCriteria(Table table, DateTimeOffset? updateTimeFrom = null, DateTimeOffset? updateTimeTo = null)
    {
        string name = null;
        string customerId = null;
        string country = null;

        if (table.Rows[0].ContainsKey("promo_name"))
        {
            name = CommonSteps.Prefix(table.Rows[0]["promo_name"]);
        }

        if (table.Rows[0].ContainsKey("customer_id"))
        {
            customerId = CommonSteps.PrefixCustomerId(table.Rows[0]["customer_id"]).ToString();
        }

        if (table.Rows[0].ContainsKey("country"))
        {
            country = table.Rows[0]["country"];
        }

        var request = CreateGetRewardsReq(
            bool.Parse(table.Rows[0]["includeCancelled"]),
            bool.Parse(table.Rows[0]["includeExpired"]),
            bool.Parse(table.Rows[0]["includeActive"]),
            name, customerId, updateTimeFrom, updateTimeTo, country);

        GetRewardsResponse = await ApiClient.GetAllAsync<TGetRwdsReq, TGetRwdsResp>(request);
    }

    protected void ThenTheGetRewardsResponseShouldReturnReward(string promoName)
    {
        GetRewardsResponse.ShouldNotBeNull();

        GetRewardsResponse.Items.Select(p => p.Reporting.Name).Should().Contain(CommonSteps.Prefix(promoName));
    }

    protected void ThenTheGetRewardsResponseShouldNotReturnReward(string promoName)
    {
        GetRewardsResponse.Items.Select(p => p.Reporting.Name).Should().NotContain(CommonSteps.Prefix(promoName));
    }

    private void AssignRewardClassSpecificProps(TRwd rwd)
    {
        if (rwd is RewardApiModel promo)
        {
            promo.Comments = "Updated";
        }
    }

    protected TCreateRwdReq CreateCreateRewardReq()
    {
        var request = new RewardRequest
        {
            Name = TestReward.Name,
            Comments = TestReward.Comments,
            CustomerId = TestReward.CustomerId,
            RewardType = TestReward.RewardType,
            Restrictions = TestReward.Restrictions,
            RewardParameters = TestReward.RewardParameters,
            Attributes = TestReward.Attributes,
            Tags = TestReward.Tags,
            CountryCode = TestReward.CountryCode,
            Purpose = TestReward.Purpose.ToString(),
            SubPurpose = TestReward.SubPurpose.ToString(),
            CustomerFacingName = TestReward.CustomerFacingName,
            DomainRestriction = TestReward.DomainRestriction,
        };

        return (TCreateRwdReq)request;
    }

    protected async Task WhenISubmitUpdateRewardRequestFor(string promoName)
    {
        var promotion = TestRewards[CommonSteps.Prefix(promoName)];

        TestReward = promotion;
        TestReward.RewardCategory = RewardType.Freebet.ToRewardCategory().ToString();
        TestReward.RewardType = RewardType.Freebet.ToString();
        TestReward.SettlementTerms.ReturnStake = false;
        TestReward.RewardParameters = new FreeBetParametersApiModel
        {
            Amount = 15
        };

        var allowedTypes = new List<string>
        {
            nameof(RewardType.Freebet),
            nameof(RewardType.Profitboost)
        };

        if (allowedTypes.Contains(promotion.RewardType, StringComparer.InvariantCultureIgnoreCase))
        {
            TestReward.DomainRestriction = new()
            {
                MultiConfig = new()
                {
                    FilterFormulae = FreeBetCreationStrategy.AllowedFormulae,
                    MaxStages = int.Parse(FreeBetCreationStrategy.DefaultParameterKeys[RewardParameterKey.MaxStages]),
                    MinStages = int.Parse(FreeBetCreationStrategy.DefaultParameterKeys[RewardParameterKey.MinStages]),
                    MaxCombinations = int.Parse(FreeBetCreationStrategy.DefaultParameterKeys[RewardParameterKey.MaxCombinations]),
                    MinCombinations = int.Parse(FreeBetCreationStrategy.DefaultParameterKeys[RewardParameterKey.MinCombinations]),
                },
                FilterOutcomes = new List<string>(),
                FilterContestRefs = new List<string>(),
                FilterContestTypes = new List<string>(),
                FilterContestCategories = new List<string>()
            };
        }

        TestReward.Attributes = new()
        {
            { "updated", Guid.NewGuid().ToString() }
        };

        AssignRewardClassSpecificProps(TestReward);

        var request = CreateCreateRewardReq();

        await ApiClient.PutAsync(HttpUtility.UrlEncode(TestReward.RewardRn), request);


        KafkaConsumerManager.Wait<RewardUpdated>(DomainConstants.TopicProfileRewardUpdates, (c) =>
        {
            var message = c.Value;
            return message.RewardRn == TestReward.RewardRn && message.IsCancelled == false && message.RewardType == request.RewardType;
        });
    }

    protected async Task WhenIUpdateRewardFor(string promoName)
    {
        var promotion = TestRewards[CommonSteps.Prefix(promoName)];

        TestReward = promotion;
        TestReward.RewardCategory = RewardType.Freebet.ToRewardCategory().ToString();
        TestReward.RewardType = RewardType.Freebet.ToString();
        TestReward.SettlementTerms.ReturnStake = false;
        TestReward.RewardParameters = new FreeBetParametersApiModel
        {
            Amount = 15
        };

        var allowedTypes = new List<string>
        {
            nameof(RewardType.Freebet),
            nameof(RewardType.Profitboost)
        };

        if (allowedTypes.Contains(promotion.RewardType, StringComparer.InvariantCultureIgnoreCase))
        {
            TestReward.DomainRestriction = new()
            {
                MultiConfig = new()
                {
                    FilterFormulae = FreeBetCreationStrategy.AllowedFormulae,
                    MaxStages = int.Parse(FreeBetCreationStrategy.DefaultParameterKeys[RewardParameterKey.MaxStages]),
                    MinStages = int.Parse(FreeBetCreationStrategy.DefaultParameterKeys[RewardParameterKey.MinStages]),
                    MaxCombinations = int.Parse(FreeBetCreationStrategy.DefaultParameterKeys[RewardParameterKey.MaxCombinations]),
                    MinCombinations = int.Parse(FreeBetCreationStrategy.DefaultParameterKeys[RewardParameterKey.MinCombinations]),
                },
                FilterOutcomes = new List<string>(),
                FilterContestRefs = new List<string>(),
                FilterContestTypes = new List<string>(),
                FilterContestCategories = new List<string>()
            };
        }

        TestReward.Attributes = new()
        {
            { "updated", Guid.NewGuid().ToString() }
        };

        AssignRewardClassSpecificProps(TestReward);

        var request = CreateCreateRewardReq();

        var rewardDomainModel = Mapper.Map<RewardDomainModel>(request);
        rewardDomainModel.RewardId = TestReward.RewardId;

        await UpdateRewardInDb(rewardDomainModel);
    }

    private async Task UpdateRewardInDb(RewardDomainModel rewardDomainModel)
    {
        var contextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<RewardsDbContext>>();
        await using var context = await contextFactory.CreateDbContextAsync();

        var existingReward = context.Rewards
            .Single(reward => reward.Id == rewardDomainModel.RewardId);

        rewardDomainModel.IsSystemGenerated = existingReward.IsSystemGenerated;
        rewardDomainModel.IsLocked = existingReward.IsLocked;

        Mapper.Map(rewardDomainModel, existingReward);
        await context.SaveChangesAsync();
    }

    protected async Task WhenISubmitUpdateRewardRequestForWithInvalidParameters(string promoName)
    {
        var promotion = TestRewards[CommonSteps.Prefix(promoName)];

        TestReward = promotion;
        TestReward.RewardParameters = new UniBoostParametersApiModel
        {
            MaxStakeAmount = 0,
            OddsLadderOffset = 50
        };

        var request = CreateCreateRewardReq();

        await ApiClient.PutAsync(HttpUtility.UrlEncode(TestReward.RewardRn), request);


        Assert.Throws<TimeoutException>(() =>
        {
            KafkaConsumerManager.Wait<RewardUpdated>(DomainConstants.TopicProfileRewardUpdates, (c) =>
            {
                var message = c.Value;
                return message.RewardRn == TestReward.RewardRn && message.IsCancelled == false && message.RewardType == request.RewardType;
            }, 5000);
        });
    }
    protected async Task WhenISubmitUpdateRewardRequestForWithInvalidMultileg(string promoName)
    {
        var promotion = TestRewards[CommonSteps.Prefix(promoName)];

        TestReward = promotion;

        var request = CreateCreateRewardReq();

        await ApiClient.PutAsync(HttpUtility.UrlEncode(TestReward.RewardRn), request);


        Assert.Throws<TimeoutException>(() =>
        {
            KafkaConsumerManager.Wait<RewardUpdated>(DomainConstants.TopicProfileRewardUpdates, (c) =>
            {
                var message = c.Value;
                return message.RewardRn == TestReward.RewardRn && message.IsCancelled == false && message.RewardType == request.RewardType;
            });
        });
    }

    protected void ThenTheUpdateRewardResponseShouldReturnUpdatedReward()
    {
        GetRewardResponse.ShouldNotBeNull();

        GetRewardResponse.CustomerId.Should().Be(TestReward.CustomerId);
        GetRewardResponse.Reporting.Name.Should().Be(TestReward.Name);
        GetRewardResponse.Reporting.IsCancelled.Should().Be(TestReward.IsCancelled);
        GetRewardResponse.Rn.Should().Be(TestReward.RewardRn);
        GetRewardResponse.RewardId.Should().Be(TestReward.RewardId);

        GetRewardResponse.Type.Should().Be(TestReward.RewardType);
        GetRewardResponse.Category.Should().Be(TestReward.RewardCategory);
        GetRewardResponse.DomainRestriction.Should().BeEquivalentTo(TestReward.DomainRestriction);

        GetRewardResponse.DateTimeRestrictions.ExpiryDateTime.Should().BeAfter(DateTime.UtcNow);

        if (GetRewardResponse.Type == nameof(RewardType.Freebet))
        {
            GetRewardResponse.SettlementTerms.ReturnStake.Should().Be(false);
        }
        else if (GetRewardResponse.Type == nameof(RewardType.Uniboost))
        {
            GetRewardResponse.SettlementTerms.ReturnStake.Should().Be(true);
        }
    }

    protected async Task WhenISubmitCancelRewardRequestFor(string promoName)
    {
        var rewards = TestRewards;

        var cancelledReward = rewards[CommonSteps.Prefix(promoName)];
        cancelledReward.IsCancelled = true;
        cancelledReward.CancellationReason = CancellationReason;

        rewards[CommonSteps.Prefix(promoName)] = cancelledReward;

        await ApiClient.PutAsync($"{HttpUtility.UrlEncode(cancelledReward.RewardRn)}/cancel", new CancelRequest { Reason = CancellationReason });


        KafkaConsumerManager.Wait<RewardUpdated>(DomainConstants.TopicProfileRewardUpdates,
            c =>
            {
                var evnt = c.Value;
                var match = evnt.RewardRn == cancelledReward.RewardRn && evnt.IsCancelled == cancelledReward.IsCancelled && evnt.RewardType == cancelledReward.RewardType;

                return match;
            });

        TestRewards = rewards;
    }

    protected void ThenTheCancelRewardResponseShouldReturnCancelledReward()
    {
        GetRewardResponse.ShouldNotBeNull();
        GetRewardResponse.Reporting.IsCancelled.Should().BeTrue();
        GetRewardResponse.Reporting.CancellationReason.Should().Be(CancellationReason);
    }

    protected async Task WhenISubmitCreateRewardRequestFor(string promoName, bool isBonus = true)
    {
        var request = RewardRequestBuilder.CreateRewardRequestForType<TCreateRwdReq>(RewardType.Uniboost,
            r =>
            {
                r.Name = CommonSteps.Prefix(promoName);
                r.CustomerId = DefaultCustomerId;
            });

        if (!isBonus)
        {
            request.CustomerId = null;
        }

        var response = await ApiClient.PostAsync<TCreateRwdReq, TRwd>(request);



        if (response.RewardRn != null)
        {
            KafkaConsumerManager.Wait<RewardCreated>(DomainConstants.TopicProfileRewardUpdates, (c) =>
            {
                var message = c.Value;
                return message.RewardType == response.RewardType && message.RewardRn == response.RewardRn;

            });
        }
        else
        {
            Assert.Throws<TimeoutException>(() =>
            {
                KafkaConsumerManager.Wait<RewardCreated>(DomainConstants.TopicProfileRewardUpdates, (c) =>
                {
                    var message = c.Value;
                    return message.RewardType == response.RewardType;

                }, 5000);
            });
        }

    }

    protected async Task WhenISubmitCreateBonusRequestFor(Table table)
    {
        foreach (var row in table.Rows)
        {
            var customerId = row["customer_id"];
            var name = row["name"];

            var request = RewardRequestBuilder.CreateRewardRequestForType<RewardRequest>(RewardType.Uniboost,
                r =>
                {
                    r.Name = CommonSteps.Prefix(name);
                    r.CustomerId = CommonSteps.PrefixCustomerId(customerId).ToString();
                    r.DomainRestriction.FilterOutcomes = row.TryGetValue(TableColumns.FilterOutcome, out var outcome) ? new() { outcome } : new List<string>();
                });
            request.RewardType = Core.WebApi.Enums.RewardType.Uniboost.ToString();

            await ApiClient.PostAsync<RewardRequest, RewardApiModel>(request);
        }
    }

    protected async Task WhenISubmitGetRewardsRequestWithNameOrCustomerId(string name, string customerId)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            name = CommonSteps.Prefix(name);
        }

        if (!string.IsNullOrWhiteSpace(customerId))
        {
            customerId = CommonSteps.PrefixCustomerId(customerId).ToString();
        }

        var request = CreateGetRewardsReq(false, false, true, name, customerId);
        GetRewardsResponse = await ApiClient.GetAllAsync<TGetRwdsReq, TGetRwdsResp>(request);
    }

    protected void ThenGetRewardsResponseShouldReturnAllRewardsWithName(string promoName)
    {
        var expectedRewards = TestRewards.Values.Where(p => p.Name == CommonSteps.Prefix(promoName)).Select(p => p.RewardRn);

        ValidateExpectedRewards(expectedRewards);
    }

    protected async Task GivenISubmitUpdateRewardRequestForToExpire(string promoName)
    {
        var reward = TestRewards[CommonSteps.Prefix(promoName)];

        var bonusToExpire = await GetFromDb(new RewardRn(reward.RewardRn).GuidValue);

        bonusToExpire.Terms.Restrictions.ExpiryDateTime = DateTime.UtcNow.AddSeconds(-1);
        await UpdateRewardInDb(bonusToExpire);
    }

    private async Task<RewardDomainModel> GetFromDb(string rewardId)
    {
        var contextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<RewardsDbContext>>();
        await using var context = await contextFactory.CreateDbContextAsync();

        var data = context.Rewards
            .Include(b => b.RewardTags).ThenInclude(bt => bt.Tag)
            .Include(p => p.RewardTemplateReward).ThenInclude(pt => pt.RewardTemplate)
            .AsNoTracking()
            .Single(reward => reward.Id == rewardId);

        // Make sure there are no circular dependencies otherwise auto-mapper does recursive mapping
        if (data is { RewardTemplateReward: { } })
        {
            foreach (var promotionTemplate in data.RewardTemplateReward.Select(pt => pt.RewardTemplate))
            {
                promotionTemplate.RewardTemplateReward.ForEach(ptp => ptp.Reward = null);
            }
        }

        return Mapper.Map<RewardDomainModel>(data);
    }

    protected async Task WhenISubmitCreateRewardRequestForReloadableBonusWithFollowingReloadOptions(Table table)
    {
        var rewardType = Enum.Parse<RewardType>(table.Rows[0]["rewardType"]);
        var rewardTypeApi = Enum.Parse<Core.WebApi.Enums.RewardType>(table.Rows[0]["rewardType"], true);

        _createPromotionRequest = RewardRequestBuilder.CreateRewardRequestForType<TCreateRwdReq>(rewardType, p =>
        {
            p.Restrictions.ClaimsPerInterval = table.Rows[0]["claimsPerInterval"].ToNullableInt();

            var enableReload = bool.Parse(table.Rows[0]["enableReload"]);
            if (enableReload)
            {
                var rewardParameters = p.RewardParameters as UniBoostReloadParametersApiModel;

                rewardParameters.Reload = new()
                {
                    MaxReload = table.Rows[0]["maxReload"].ToNullableInt(),
                    StopOnMinimumWinBets = int.Parse(table.Rows[0]["stopOnMinimumWinBets"])
                };
            }
        });

        _createPromotionRequest.RewardType = rewardTypeApi.ToString();

        _createPromotionResponse = await ApiClient.PostAsync<TCreateRwdReq, TRwd>(_createPromotionRequest);
    }


    protected void ThenUniboostReloadRewardShouldHaveBeenCreated()
    {
        _createPromotionResponse.Should().NotBeNull();

        var reloadParamResponse = _createPromotionResponse.RewardParameters as UniBoostReloadParametersApiModel;
        var reloadParamRequest = _createPromotionResponse.RewardParameters as UniBoostReloadParametersApiModel;

        reloadParamResponse.Reload.Should().NotBeNull();
        reloadParamResponse.Reload.MaxReload.Should().Be(reloadParamRequest.Reload.MaxReload);
        reloadParamResponse.Reload.StopOnMinimumWinBets.Should().Be(reloadParamRequest.Reload.StopOnMinimumWinBets);
    }

    protected void ThenItShouldContainOnlyRewards(int numOfRewards)
    {
        GetRewardsResponse.ShouldNotBeNull();
        GetRewardsResponse.ItemCount.Should().Be(numOfRewards);
        GetRewardsResponse.Items.Count.Should().Be(numOfRewards);
    }

    protected void ThenThePatchRewardResponseShouldReturnUpdatedBonusWithTheFollowing(Table table)
    {
        // Make sure people provide only 1 row
        Assert.AreEqual(table.Rows.Count, 1, "This step is designed to work only for 1 row.");

        // Assign name and comments from table
        string name = null;
        string comments = null;

        if (table.Rows[0].ContainsKey(TableColumns.Name))
        {
            name = CommonSteps.Prefix(table.Rows[0][TableColumns.Name]);
        }

        if (table.Rows[0].ContainsKey(TableColumns.Comments))
        {
            comments = table.Rows[0][TableColumns.Comments];
        }

        // Check that name and comments match what we expect
        PatchBonusResponse.Comments.Should().Be(comments);
        PatchBonusResponse.Name.Should().Be(name);
    }

    protected async Task WhenISubmitAPatchRewardRequestForTheFollowingCriteria(string promoName, Table table)
    {
        string name = null;
        string comments = null;

        if (table.Rows[0].ContainsKey(TableColumns.Name))
        {
            name = CommonSteps.Prefix(table.Rows[0][TableColumns.Name]);
        }

        if (table.Rows[0].ContainsKey(TableColumns.Comments))
        {
            comments = table.Rows[0][TableColumns.Comments];
        }

        var promotion = TestRewards[CommonSteps.Prefix(promoName)];
        promotion.Restrictions.StartDateTime = DateTime.MinValue; // make sure reward has started, so it can be patched
        TestReward = promotion;

        var patchRequest = new PatchRewardRequest { Comments = comments, Name = name };

        try
        {
            var response = await ApiClient.PatchAsync<PatchRewardRequest, TRwd>(HttpUtility.UrlEncode(TestReward.RewardRn), patchRequest);
            PatchBonusResponse = response;
        }
        catch
        {
            // ignored
        }
    }
    protected async Task WhenISubmitANullPatchRewardRequestFor(string promoName)
    {
        var promotion = TestRewards[CommonSteps.Prefix(promoName)];
        promotion.Restrictions.StartDateTime = DateTime.MinValue; // make sure reward has started, so it can be patched
        TestReward = promotion;

        var patchRequest = new PatchRewardRequest { Comments = null, Name = null };

        try
        {
            var response = await ApiClient.PatchAsync<PatchRewardRequest, TRwd>(HttpUtility.UrlEncode(TestReward.RewardRn), patchRequest);
            PatchBonusResponse = response;
        }
        catch
        {
            // ignored
        }
    }
}
