using FluentAssertions;
using FluentAssertions.Execution;

using Kindred.Rewards.Core.WebApi.Enums;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Responses;
using Kindred.Rewards.Rewards.FunctionalTests.Common;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Helpers;
using Kindred.Rewards.Rewards.FunctionalTests.Common.Setup;

using Newtonsoft.Json;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Kindred.Rewards.Rewards.FunctionalTests.Steps;

[Binding]
public class SortingSteps : AcceptanceScenarioBase
{
    private readonly IApiClient _rewardsApiClient;

    public SortingSteps(ScenarioContext scenarioContext, FeatureContext featureContext)
        : base(scenarioContext, featureContext)
    {
        _rewardsApiClient = new ApiClient("rewards", RestClientFactory, scenarioContext, SerializerSettings.GetJsonSerializerSettings);
    }

    public GetRewardsResponse ResponseGetRewards
    {
        get
        {
            if (!ScenarioContext.ContainsKey(nameof(GetRewardsResponse)))
            {
                ScenarioContext.Set(JsonConvert.SerializeObject(new GetRewardsResponse()), nameof(GetRewardsResponse));
            }

            return JsonConvert.DeserializeObject<GetRewardsResponse>(ScenarioContext.Get<string>(nameof(GetRewardsResponse)));
        }

        set => ScenarioContext.Set(JsonConvert.SerializeObject(value), nameof(GetRewardsResponse));
    }

    [When(@"I submit GetRewards request with the following criteria")]
    public async Task WhenISubmitGetRewardsRequestWithTheFollowingCriteria(Table table)
    {
        Assert.IsTrue(table.RowCount == 1);

        var fieldName = Enum.Parse<SortableRewardFields>(table.Rows[0][TableColumns.SortByFieldName]).ToString();
        var customerId = table.Rows[0][TableColumns.CustomerId];

        fieldName = table.Rows[0][TableColumns.SortOrder].ToUpperInvariant() switch
        {
            "DESCENDING" => fieldName.Insert(0, "-"),
            _ => fieldName
        };

        ResponseGetRewards = await _rewardsApiClient.GetAllAsync<GetRewardsRequest, GetRewardsResponse>(new()
        {
            SortBy = fieldName,
            CustomerId = CommonSteps.PrefixCustomerId(customerId).ToString()
        });
    }

    [Then(@"I expect the results in the following order for sort field '([^']*)'")]
    public void ThenIExpectTheResultsInTheFollowingOrderForSortField(string sortField, Table table)
    {
        Assert.AreEqual(table.RowCount, ResponseGetRewards.ItemCount);

        for (var i = 0; i < table.RowCount; i++)
        {
            var expectedRow = table.Rows[i];
            var actualRow = ResponseGetRewards.Items.ElementAt(i);

            using (new AssertionScope())
            {
                actualRow.Reporting.Name.Should().Be(CommonSteps.Prefix(expectedRow[TableColumns.Name]));
                actualRow.CustomerId.Should().Be(CommonSteps.PrefixCustomerId(expectedRow[TableColumns.CustomerId]).ToString());
            }

            switch (sortField)
            {
                case TableColumns.Jurisdiction:
                    actualRow.PlatformRestrictions.Jurisdiction.Should().Be(expectedRow[sortField]);
                    break;

                case TableColumns.CountryCode:
                    actualRow.PlatformRestrictions.CountryCode.Should().Be(expectedRow[sortField]);
                    break;

                case TableColumns.RewardType:
                    actualRow.Type.Should().Be(expectedRow[sortField]);
                    break;

                case TableColumns.CreatedBy:
                    actualRow.Reporting.CreatedBy.Should().Be(expectedRow[sortField]);
                    break;

                case TableColumns.UpdatedBy:
                    actualRow.Reporting.UpdatedBy.Should().Be(expectedRow[sortField]);
                    break;

                case TableColumns.StartDateTime:
                    actualRow.DateTimeRestrictions.StartDateTime.Should().Be(DateTime.Parse(expectedRow[sortField]));
                    break;

                case TableColumns.Status:
                    actualRow.Status.Should().Be(expectedRow[sortField]);
                    break;
            }
        }
    }

}
