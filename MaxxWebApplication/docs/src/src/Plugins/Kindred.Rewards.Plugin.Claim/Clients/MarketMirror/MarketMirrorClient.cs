using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Infrastructure.Net;
using Kindred.Rewards.Plugin.Claim.Clients.MarketMirror.Responses;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Polly;
using Polly.Retry;

using RestSharp;

namespace Kindred.Rewards.Plugin.Claim.Clients.MarketMirror;

public interface IMarketMirrorClient
{
    public Task<GetContestsResponse> GetContests(IEnumerable<string> contestKeys);
}

public class MarketMirrorClient : IMarketMirrorClient
{
    private const int RetryLimit = 3;
    private const int TimeoutMilliseconds = 60000;

    private const string ContestsPath = "/contests";

    private readonly ILogger<MarketMirrorClient> _logger;
    private readonly IRestClientWrapper _restClient;
    private readonly AsyncRetryPolicy<RestResponse<GetContestsResponse>> _apiRetryPolicy;

    public MarketMirrorClient(
        ILogger<MarketMirrorClient> logger,
        Func<string, int, IRestClientWrapper> restClientFactory,
        IConfiguration config)
    {
        var baseUrl = config[DomainConstants.RewardsMarketMirrorBaseUrl];
        _logger = logger;
        _restClient = restClientFactory(baseUrl, TimeoutMilliseconds);

        _apiRetryPolicy = Policy
            .HandleResult<RestResponse<GetContestsResponse>>(r => RetryStatusCodes.StatusCodesWorthRetrying.ContainsKey(r.StatusCode))
            .WaitAndRetryAsync(
                RetryLimit,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (result, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning("Retrying request to Market Mirror due to the status code: {statusCode}", result.Result.StatusCode);
                });
    }

    public async Task<GetContestsResponse> GetContests(IEnumerable<string> contestKeys)
    {
        var pathWithQuery = QueryHelpers.AddQueryString(ContestsPath, "contestKeys", contestKeys.ToCsv(","));

        var request = new RestRequest(pathWithQuery);

        var result = await _apiRetryPolicy.ExecuteAsync(
            async () => await _restClient.ExecuteAsync<GetContestsResponse>(request));

        if (!result.IsSuccessful)
        {
            _logger.LogInformation("Market Mirror request failed with the status code: {statusCode} for the keys {keys}", result.StatusCode, contestKeys.ToCsv(","));
        }

        return result.IsSuccessful ? result.Data : new();
    }
}
