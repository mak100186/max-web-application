using FluentAssertions;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Helpers;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Payloads.BetModel;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Responses;
using Kindred.Rewards.Rewards.FunctionalTests.Common;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Helpers;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Setup;
using Kindred.Rewards.Rewards.FunctionalTests.Steps.Context;
using Kindred.Rewards.Rewards.Tests.Common;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.Tests.Common.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Polly;

using Quartz.Util;

using TechTalk.SpecFlow;

using SortableRewardClaimFields = Kindred.Rewards.Core.WebApi.Enums.SortableRewardClaimFields;

namespace Kindred.Rewards.Rewards.FunctionalTests.Steps;

[Binding]
public class RewardClaimSteps : AcceptanceScenarioBase
{
    private readonly IApiClient _claimApiClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly BetContext _betContext;

    private GetClaimsResponse _getClaimsResponse;

    public RewardClaimSteps(ScenarioContext scenarioContext, FeatureContext featureContext,
        BetContext betContext) : base(scenarioContext, featureContext)
    {
        _claimApiClient = new ApiClient("rewardclaims", RestClientFactory, scenarioContext, SerializerSettings.GetJsonSerializerSettings);
        _serviceScopeFactory = ServiceProvider.GetService<IServiceScopeFactory>();
        _betContext = betContext;
    }

    private CustomerEntitlementsResponse GetEntitlementResponse
    {
        get => ScenarioContext.Get<CustomerEntitlementsResponse>(nameof(GetEntitlementResponse));

        set => ScenarioContext.Set(value, nameof(GetEntitlementResponse));
    }

    private BatchClaimResponse BatchClaimResponse
    {
        get => ScenarioContext.Get<BatchClaimResponse>(nameof(BatchClaimResponse));

        set => ScenarioContext.Set(value, nameof(BatchClaimResponse));
    }

    private Dictionary<string, RewardApiModel> TestPromotions
    {
        get => ScenarioContext.Get<Dictionary<string, RewardApiModel>>(nameof(TestPromotions));

        set => FeatureContext.Set(value, nameof(TestPromotions));
    }

    [When(@"I submit GetEntitlement request for following criteria:")]
    public async Task WhenISubmitGetEntitlementRequestForFollowingCriteria(Table table)
    {
        var customerId = CommonSteps.PrefixCustomerId(table.Rows[0]["customer_id"]).ToString();
        var templateKeys = table.Rows[0]["template_keys"].ExtractValues().Select(CommonSteps.Prefix).ToList();

        var parameters = new List<Tuple<string, string>>();

        foreach (var templateKey in templateKeys)
        {
            parameters.Add(new("templateKeys", templateKey));
        }

        GetEntitlementResponse = await _claimApiClient.GetAsync<CustomerEntitlementsResponse>($"entitlements/{customerId}", parameters);
    }

    [Then(@"GetEntitlement response should not include following rewards:")]
    public void ThenGetEntitlementResponseShouldNotIncludeFollowingRewards(Table table)
    {
        Assert.AreEqual(table.Rows.Count, 1, "This step is designed to work only for 1 row.");

        var entitlements = GetEntitlementResponse.Entitlements.Select(x => x.Reporting.Name).ToList();

        entitlements.Should().NotContain(table.Rows[0]["bonus_name"].ExtractValues().Select(CommonSteps.Prefix).ToList());
    }

    [Then(@"GetEntitlement response should include following rewards:")]
    public void ThenGetEntitlementResponseShouldIncludeFollowingPromotions(Table table)
    {
        Assert.AreEqual(table.Rows.Count, 1, "This step is designed to work only for 1 row.");

        GetEntitlementResponse.ShouldNotBeNull();
        GetEntitlementResponse.Entitlements.Should().NotBeEmpty();

        foreach (var expectedPromotionName in table.Rows[0]["promotion_name"].ExtractValues().Select(CommonSteps.Prefix))
        {
            var found = GetEntitlementResponse.Entitlements.Where(e => e.Reporting.Name.Contains(expectedPromotionName)).ToList();

            found.Should().NotBeNullOrEmpty($"Expected Name {expectedPromotionName} to be returned in entitlements");
            found.All(e => e.Reporting.Name.Contains(expectedPromotionName)).Should().BeTrue($"Expected promotion name for Name ${expectedPromotionName}");
        }

        foreach (var expectedBonusName in table.Rows[0]["bonus_name"].ExtractValues().Select(CommonSteps.Prefix))
        {
            var found = GetEntitlementResponse.Entitlements.Where(e => e.Reporting.Name.Contains(expectedBonusName)).ToList();

            found.Should().NotBeNullOrEmpty($"Expected bonus Name {expectedBonusName} to be returned in entitlements");
        }
    }

    [Then(@"Rewards should have nextInterval field:")]
    public void ThenRewardsShouldHaveFollowingNextIntervals(Table table)
    {
        foreach (var tableRow in table.Rows)
        {
            var expectedPromotion = CommonSteps.Prefix(tableRow["promotion_name"]);
            var promo = GetEntitlementResponse.Entitlements.FirstOrDefault(e => e.Reporting.Name.Contains(expectedPromotion));
            promo.ShouldNotBeNull();
        }
    }

    [When(@"I submit ClaimEntitlement request for following criteria for Currency '(.*)':")]
    public async Task WhenISubmitClaimEntitlementRequestForFollowingCriteriaForCurrency(string currencyCode, Table table)
    {
        var request = new BatchClaimRequest
        {
            CustomerId = CommonSteps.PrefixCustomerId(table.Rows[0]["customer_id"]).ToString(),
            CouponRn = TestConstants.TestPrefix,
            Claims = new()
        };

        foreach (var row in table.Rows)
        {
            var entitlement = GetEntitlementResponse.Entitlements.First(e => e.Reporting.Name == CommonSteps.Prefix(row["name"]));

            for (var i = 0; i < int.Parse(row["number_of_claims"]); i++)
            {
                var outcome = row.GetValueFromRow("outcome", ObjectBuilder.CreateString());
                request.CurrencyCode = currencyCode ?? "AUD";

                var stakeAmount = 3.0m;

                if (row.ContainsKeyThatIsNotEmpty("stake_amount"))
                {
                    stakeAmount = decimal.Parse(row["stake_amount"]);
                }

                //add a stage for each entry in the stagePriceData
                var stages = new List<CompoundStageApiModel>();
                if (row.ContainsKeyThatIsNotEmpty("stagePriceData"))
                {
                    var stagePrices = row["stagePriceData"].ExtractValues();
                    foreach (var stagePrice in stagePrices)
                    {
                        stages.Add(new()
                        {
                            RequestedSelection = new() { Outcome = outcome },
                            Odds = new() { RequestedPrice = decimal.Parse(stagePrice) },
                            Market = ObjectBuilder.CreateMarketKey(contestKey: row.GetValueFromRow<string>(TableColumns.ContestKey, default)),
                        });
                    }
                }

                //else add a single stage if no stagePriceData found
                if (!stages.Any())
                {
                    stages.Add(new()
                    {
                        RequestedSelection = new() { Outcome = outcome },
                        Odds = new() { RequestedPrice = 1.5m },
                        Market = ObjectBuilder.CreateMarketKey(contestKey: row.GetValueFromRow<string>(TableColumns.ContestKey, default)),
                    });
                }

                request.Claims.Add(new()
                {
                    Rn = entitlement.Rn,
                    Hash = entitlement.Hash,
                    Bet = new()
                    {
                        Formula = "singles",
                        RequestedStake = stakeAmount,
                        Status = "pending",
                        Rn = Guid.NewGuid().ToString(),
                        Stages = stages,
                        RequestedCombinations = new List<CombinationApiModel>
                        {
                            new()
                            {
                                Rn = Guid.NewGuid().ToString(),
                                Selections = new List<SelectionApiModel>
                                {
                                    new()
                                    {
                                        Outcome = outcome,
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }

        BatchClaimResponse = await _claimApiClient.PostAsync<BatchClaimRequest, BatchClaimResponse>("batchclaim", request);
    }

    [When(@"I submit ClaimEntitlement request for following criteria:")]
    public async Task WhenISubmitClaimEntitlementRequestForFollowingCriteria(Table table)
    {
        await WhenISubmitClaimEntitlementRequestForFollowingCriteriaForCurrency(null, table);
    }

    [When(@"a request to claim an entitlement is received for customer '([^']*)':")]
    public async Task WhenARequestToClaimAnEntitlementIsReceivedForCustomer(string customerId, Table table)
    {
        var request = new BatchClaimRequest
        {
            CustomerId = CommonSteps.PrefixCustomerId(customerId).ToString(),
            CouponRn = TestConstants.TestPrefix,
            Claims = new(),
            CurrencyCode = "AUD"
        };

        foreach (var row in table.Rows)
        {
            var bet = _betContext.ReferenceBets.Single(bet => bet.Rn == row.GetValueFromRow("betRn", ObjectBuilder.CreateString()));

            var entitlement =
                GetEntitlementResponse.Entitlements.First(e => e.Reporting.Name == CommonSteps.Prefix(row["name"]));

            request.Claims.Add(new()
            {
                Rn = entitlement.Rn,
                Hash = entitlement.Hash,
                Bet = new()
                {
                    Formula = bet.Formula,
                    RequestedStake = bet.Stake,
                    Status = "pending",
                    Rn = bet.Rn,
                    Stages = bet.Stages.Select(stage => new CompoundStageApiModel
                    {
                        AcceptedSelection = new()
                        {
                            Outcome = stage.Selection.Outcome
                        },
                        Market = stage.Market,
                        Odds = new()
                        {
                            RequestedPrice = stage.Odds.Price
                        },
                        RequestedSelection = new()
                        {
                            Outcome = stage.Selection.Outcome
                        }
                    }),
                    RequestedCombinations = bet.Combinations.Select(combination => new CombinationApiModel
                    {
                        Rn = combination.Rn,
                        Selections = combination.Selection.Select(selection => new SelectionApiModel
                        {
                            Outcome = selection.Outcome,
                        })
                    })
                }
            });
        }

        BatchClaimResponse = await _claimApiClient.PostAsync<BatchClaimRequest, BatchClaimResponse>("batchclaim", request);
    }

    [Then(@"the entitlements for customer '([^']*)' should contain:")]
    public void ThenTheEntitlementsForCustomerShouldContain(string customerId, Table table)
    {
        customerId = CommonSteps.PrefixCustomerId(customerId).ToString();

        foreach (var row in table.Rows)
        {
            var name = CommonSteps.Prefix(row.GetValueFromRow<string>(TableColumns.Name, default));

            var entitlement = GetEntitlementResponse.Entitlements.Select(entitlement => entitlement)
                .FirstOrDefault(c => c.CustomerId == customerId && c.Reporting.Name == name);

            entitlement.Should().BeEquivalentTo(new
            {
                DateTimeRestrictions = new
                {
                    RemainingClaimsPerInterval =
                        row.GetValueFromRow<int>(TableColumns.RemainingClaimsPerInterval, default)
                }
            });
        }
    }


    [Then(@"I expect following in the ClaimedEntitlements for the customerID='(.*)':")]
    public void ThenIExpectFollowingInTheClaimedEntitlementsForTheCustomerId(string customerId, Table table)
    {
        customerId = CommonSteps.PrefixCustomerId(customerId).ToString();

        foreach (var row in table.Rows)
        {
            var name = CommonSteps.Prefix(TableColumns.Name);
            var claim = BatchClaimResponse.Responses.Select(c => c.Claim).FirstOrDefault(c => c.Reward.CustomerId == customerId);
            claim.ShouldNotBeNull();
        }
    }

    [Then(@"ClaimEntitlement response should claim following rewards:")]
    public void ThenClaimEntitlementResponseShouldClaimFollowingRewards(Table table)
    {
        BatchClaimResponse.ShouldNotBeNull();
        BatchClaimResponse.Responses.All(r => r.Success).Should().BeTrue("All claim response should be successful");

        foreach (var expectedPromotion in table.Rows[0]["customer_id"].ExtractValues().Select(x => CommonSteps.PrefixCustomerId(x).ToString()))
        {
            BatchClaimResponse.Responses.Select(e => e.Claim.Reward.CustomerId).Should().Contain(expectedPromotion);

            var stages = BatchClaimResponse
                .Responses
                .SelectMany(e => e.Claim.Bet.Stages);
        }
    }

    [Then(@"GetEntitlement response should not include promotion '(.*)'")]
    public void ThenGetEntitlementResponseShouldNotIncludePromotion(string promoName)
    {
        GetEntitlementResponse.ShouldNotBeNull();
        GetEntitlementResponse.Entitlements
            .Should()
            .NotContain(p => p.Reporting.Name == CommonSteps.Prefix(promoName));
    }


    [When(@"I submit GetEntitlement request after the next interval for following criteria:")]
    public void WhenISubmitGetEntitlementRequestAfterTheNextIntervalForFollowingCriteria(Table table)
    {
        var nextInterval = new DateTime(CronService.GetNextInterval(table.Rows[0]["claim_interval"]), DateTimeKind.Utc);
        var utcNow = DateTime.UtcNow;
        var delay = Math.Max((int)(nextInterval - utcNow).TotalMilliseconds, TimeSpan.FromSeconds(5).TotalMilliseconds);
        var customerId = CommonSteps.PrefixCustomerId(table.Rows[0]["customer_id"]).ToString();
        var templateKeys = table.Rows[0]["template_keys"].ExtractValues().Select(CommonSteps.Prefix).ToList();

        var parameters = new List<Tuple<string, string>>();

        foreach (var templateKey in templateKeys)
        {
            parameters.Add(new("templateKeys", templateKey));
        }

        delay.Should().BeGreaterThan(0, "Expected delay in ms to be greater than zero");
        delay.Should().BeLessThan(int.MaxValue, "Expected delay in ms to be less than int.MaxValue");

        Policy
            .Handle<InvalidOperationException>()
            .WaitAndRetry(
                5,
                retryAttempt => TimeSpan.FromMilliseconds(delay),
                (exception, timeSpan, retryCount, context) =>
                    {
                        TestContext.Progress.WriteLine(
                            "No entitlements found for customerId {0} cache on attempt {1}",
                            customerId,
                            retryCount);
                    })
            .Execute(() =>
                {
                    GetEntitlementResponse = _claimApiClient.GetAsync<CustomerEntitlementsResponse>($"entitlements/{customerId}", parameters).Result;

                    if (!GetEntitlementResponse.Entitlements.Any())
                    {
                        throw new InvalidOperationException();
                    }
                });
    }

    private readonly IDictionary<string, RewardClaimDomainModel> _rewardClaims = new Dictionary<string, RewardClaimDomainModel>();

    [Given(@"I have claims in the system")]
    public async Task GivenIHaveClaimsInTheSystem(Table table)
    {
        foreach (var row in table.Rows)
        {
            var claim = await RewardClaimBuilder.CreatePromotionClaimInDb(
                _serviceScopeFactory,
                c =>
                    {
                        c.CustomerId = CommonSteps.Prefix(row["customer_id"]);
                        c.PromotionName = CommonSteps.Prefix(row["name"]);
                        c.CouponRn = TestConstants.TestPrefix;
                        c.InstanceId = CommonSteps.Prefix(row["instance_id"]);
                        c.RewardId = row.GetValueFromRow(TableColumns.RewardRn, Guid.NewGuid().ToString());
                        c.CouponRn = CommonSteps.Prefix(row["bet_client_ref"]);
                        c.BetRn = row["sequence_no"];
                        c.BetOutcomeStatus = row["bet_outcome_status"].ToEnumSafe<BetOutcome>();
                        c.CurrencyCode = row.GetValue("CurrencyCode");
                    });

            claim.Should().NotBeNull();

            _rewardClaims[claim.PromotionName] = claim;
        }
    }

    [Then(@"Claims should have correct CurrencyCode values")]
    public void ThenClaimsShouldHaveCorrectCurrencyCodeValues()
    {
        var claims = _getClaimsResponse.Items.ToList();
        claims.ForEach(c => (_rewardClaims[c.RewardName].CurrencyCode == c.CurrencyCode).Should().BeTrue());
    }

    [When(@"I update the UpdateTime of a claim for customerId '(.*)' to '(.*)'")]
    public async Task WhenIUpdateTheUpdateTimeOfAClaimForCustomerIdTo(string custId, string updateDateTime)
    {
        var contextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<RewardsDbContext>>();
        await using var context = await contextFactory.CreateDbContextAsync();

        if (DateTime.TryParse(updateDateTime, out var parsedUpdateDateTime))
        {
            parsedUpdateDateTime = DateTime.SpecifyKind(parsedUpdateDateTime, DateTimeKind.Utc);
            var updatedOn = parsedUpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz");

            custId = CommonSteps.Prefix(custId);

            var sqlString = @$" UPDATE ""{nameof(RewardClaim)}""
                                    SET ""{nameof(RewardClaim.UpdatedOn)}"" = '{updatedOn}'
                                    WHERE ""{nameof(RewardClaim.CustomerId)}"" = '{custId}'";

            (await context.Database.ExecuteSqlRawAsync(sqlString)).Should().Be(1);
        }
    }

    [When(@"I submit GetClaims request for criteria")]
    public async Task WhenISubmitGetClaimsRequestForCriteria(Table table)
    {
        var request = new GetClaimsByFilterRequest
        {
            CustomerId = TryGetTableRowValue(table.Rows[0], "customer_id", TestConstants.TestPrefix),
            RewardName = TryGetTableRowValue(table.Rows[0], "name", TestConstants.TestPrefix),
            InstanceId = TryGetTableRowValue(table.Rows[0], "instance_id", TestConstants.TestPrefix),
            RewardRn = TryGetTableRowValue(table.Rows[0], "reward_key", TestConstants.RewardRn.ToString()),
            CouponRef = TryGetTableRowValue(table.Rows[0], "bet_client_ref", TestConstants.TestPrefix),
            BetRef = table.Rows[0].TryGetAndReturn("sequence_no"),
            RewardType = TryGetTableRowValue(table.Rows[0], "reward_type", string.Empty),
            BetOutcomeStatus = TryGetTableRowValue(table.Rows[0], "bet_outcome_status", string.Empty),
            ClaimStatus = TryGetTableRowValue(table.Rows[0], "claim_status", string.Empty)
        };

        if (DateTime.TryParse(TryGetTableRowValue(table.Rows[0], "update_date_time_utc_from", string.Empty), out var updatedDateFromUtc))
        {
            request.UpdatedDateFromUtc = updatedDateFromUtc.ToUniversalTime();
        }

        if (DateTime.TryParse(TryGetTableRowValue(table.Rows[0], "update_date_time_utc_to", string.Empty), out var updatedDateToUtc))
        {
            request.UpdatedDateToUtc = updatedDateToUtc.ToUniversalTime();
        }

        if (bool.TryParse(TryGetTableRowValue(table.Rows[0], "is_descending", string.Empty), out var isDescending))
        {
            request.SortBy = "-" + SortableRewardClaimFields.UpdatedOn;
        }
        else
        {
            request.SortBy = SortableRewardClaimFields.UpdatedOn.ToString();
        }

        _getClaimsResponse = await _claimApiClient.GetAllAsync<GetClaimsByFilterRequest, GetClaimsResponse>(request);
    }

    [When(@"I submit a GetClaims request filtering by reward rn for claim: '([^']*)'")]
    public async Task WhenISubmitAGetClaimsRequestByRewardRnForClaim(string claimName)
    {
        var claim = _rewardClaims[CommonSteps.Prefix(claimName)];

        var request = new GetClaimsByFilterRequest
        {
            RewardRn = claim.RewardId
        };

        _getClaimsResponse = await _claimApiClient.GetAllAsync<GetClaimsByFilterRequest, GetClaimsResponse>(request);
    }


    [Then(@"GetClaims response should return result")]
    public void ThenGetClaimsResponseShouldReturnResult(Table table)
    {
        _getClaimsResponse.ShouldNotBeNull();
        _getClaimsResponse.ItemCount.Should().BeGreaterOrEqualTo(int.Parse(table.Rows[0]["row_count"]));
        _getClaimsResponse.Items
            .Should()
            .Contain(c => c.CustomerId == CommonSteps.Prefix(table.Rows[0]["customer_id"]));
    }

    [Then(@"the claims are in following order of their update times")]
    public void ThenTheClaimsAreInFollowingOrderOfTheirUpdateTimes(Table table)
    {
        _getClaimsResponse.ShouldNotBeNull();
        var claims = _getClaimsResponse.Items.ToList();
        var updateTimes = claims.Select(c => c.UpdatedOn).ToList();

        CollectionAssert.AllItemsAreUnique(updateTimes);
        if (table.Rows[0]["sort_order"] == "ascending")
        {
            CollectionAssert.IsOrdered(updateTimes);

        }
        else if (table.Rows[0]["sort_order"] == "descending")
        {
            CollectionAssert.IsOrdered(updateTimes, Comparer<DateTime>.Create(
                (d1, d2) => d2.CompareTo(d1)));
        }
    }

    [Then(@"GetClaims response should return betoutcome status")]
    public void ThenGetClaimsResponseShouldReturnBetoutcomeStatus(Table table)
    {
        _getClaimsResponse.ShouldNotBeNull();

        var expectedClaim = _getClaimsResponse.Items
            .SingleOrDefault(c => c.RewardName == CommonSteps.Prefix(table.Rows[0][TableColumns.Name]));

        var expectedBetoutcomeStatus = string.IsNullOrWhiteSpace(table.Rows[0]["bet_outcome_status"])
                                           ? null
                                           : table.Rows[0]["bet_outcome_status"];
        Assert.NotNull(expectedClaim);
        expectedClaim.BetOutcomeStatus.Should().Be(expectedBetoutcomeStatus);
    }

    [When(@"I submit GetClaims request for empty criteria")]
    public async Task WhenISubmitGetClaimsRequestForEmptyCriteria()
    {
        var request = new GetClaimsByFilterRequest();

        _getClaimsResponse = await _claimApiClient.GetAllAsync<GetClaimsByFilterRequest, GetClaimsResponse>(request);
    }

    [Then(@"the cache for template key '(.*)' is deleted")]
    public async Task ThenTheCacheForTemplateKeyIsDeleted(string templateKey)
    {
        var testCustomerIds = await GetCustomersByPromotionTemplateKey(CommonSteps.Prefix(templateKey));

        foreach (var customerId in testCustomerIds)
        {
            //await this.cachingService.ClearAsync(customerId);
        }
    }

    private static string TryGetTableRowValue(TableRow row, string columnHeader, string prefix)
    {
        return row.GetValue(columnHeader, prefix);
    }

    public async Task<IReadOnlyCollection<string>> GetCustomersByPromotionTemplateKey(string templateKey)
    {
        var contextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<RewardsDbContext>>();
        await using var context = await contextFactory.CreateDbContextAsync();

        return await context.RewardTemplateCustomers
            .AsNoTracking()
            .Where(t => t.PromotionTemplateKey.ToLower() == templateKey.ToLower())
            .Select(t => t.CustomerId)
            .ToListAsync();
    }
}
