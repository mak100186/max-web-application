using System.Collections.Concurrent;

using Kindred.Rewards.Core.Infrastructure.Net;
using Kindred.Rewards.Core.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Polly;
using Polly.Retry;

using RestSharp;

namespace Kindred.Rewards.Core.Client;

public interface IOddsLadderClient
{
    Task Initialise();

    void Clear(string contestType);

    Task AddOrUpdate(string contestType, OddsLadder oddsLadder = null);

    Task<OddsLadder> GetOddsLadder(string contestType);
}

public class OddsLadderClient : IOddsLadderClient
{
    private static readonly ConcurrentDictionary<string, OddsLadder> s_cache = new();

    private const int RetryLimit = 30;
    private const int RetryIntervalMilliseconds = 500;

    private readonly ILogger<OddsLadderClient> _logger;
    private readonly IRestClientWrapper _restClient;

    private readonly AsyncRetryPolicy _apiRetryPolicy;
    private readonly bool _shouldFetchOnDemand = true;

    public OddsLadderClient(
        ILogger<OddsLadderClient> logger,
        Func<string, int, IRestClientWrapper> restClientFactory,
        IConfiguration config)
    {
        _logger = logger;
        var baseUrl = config[DomainConstants.OfferPriceManagerBaseUrl];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new($"BaseUrl passed to OddsLadderClient is empty. = {baseUrl}");
        }

        _logger.LogInformation("OddsLadders will be fetched from = {baseUrl}", baseUrl);

        _restClient = restClientFactory(baseUrl, 60 * 1000);

        if (bool.TryParse(config[DomainConstants.ShouldFetchOnDemand], out var fetchOnDemand))
        {
            _shouldFetchOnDemand = fetchOnDemand;
        }

        _apiRetryPolicy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(RetryLimit, _ => TimeSpan.FromMilliseconds(RetryIntervalMilliseconds));

    }

    public async Task Initialise()
    {
        if (_restClient == null)
        {
            return;
        }

        /*
         * The algorithm is as follows:
         * 1. We get the default ladder and cache it.
         * 2. When a contest specific ladder is sought and:
         *      a. If a ladder is not found in cache when accessed, then we fetch the ladder from the api and cache it.
         *      b. If a ladder is found, we return it. 
         */
        await GetOddsLadderFromClient("default");

        if (!_shouldFetchOnDemand)
        {
            await GetAllContestSpecificOddsLadder();
        }
    }

    public async Task AddOrUpdate(string contestType, OddsLadder oddsLadder = null)
    {
        contestType = contestType.ToLower();

        Clear(contestType);

        if (oddsLadder == null)
        {
            await GetOddsLadderFromClient(contestType);

            return;
        }

        OrderKeysAndCache(oddsLadder, DomainConstants.GetOddsLadderContestCacheKey(contestType));
    }

    public async Task<OddsLadder> GetOddsLadder(string contestType)
    {
        contestType = contestType.ToLower();

        var cacheKey = DomainConstants.GetOddsLadderContestCacheKey(contestType);

        if (!s_cache.ContainsKey(cacheKey))
        {
            await GetOddsLadderFromClient(contestType);
        }

        return s_cache.ContainsKey(cacheKey) ? s_cache[cacheKey] : null;
    }

    private async Task GetOddsLadderFromClient(string contestType)
    {
        contestType = contestType.ToLower();

        var routeAction = $"oddsLadders/{contestType}";

        var oddsLadder = await GetOddsLadderFromOfferManagerAsync<OddsLadder>(routeAction);

        if (oddsLadder == null)
        {
            _logger.LogWarning("Could not fetch default odds ladder");
            return;
        }

        _logger.LogInformation($"Received 1 odds ladder for the following contest: [{contestType}]");

        OrderKeysAndCache(oddsLadder, DomainConstants.GetOddsLadderContestCacheKey(contestType));
    }

    private async Task GetAllContestSpecificOddsLadder()
    {
        const int itemsToFetch = 10;
        int totalItems;
        var offset = 0;

        List<OddsLadder> oddsLadders = new();

        do
        {
            var routeAction = $"oddsLadders?Offset={offset++}&Limit={itemsToFetch}";
            var oddsLadderOffset = await GetOddsLadderFromOfferManagerAsync<OddsLadderOffset>(routeAction);

            // Failed to retrieve odds ladder, we break
            if (oddsLadderOffset == null)
            {
                break;
            }

            totalItems = oddsLadderOffset.ItemCount;

            oddsLadders.AddRange(oddsLadderOffset.Items);

        } while (totalItems > oddsLadders.Count);

        if (oddsLadders.Count == 0)
        {
            _logger.LogWarning("Could not fetch all contest odds ladders");
            return;
        }

        _logger.LogInformation($"Received {oddsLadders.Count} odds ladder for the following contests: [{string.Join(", ", oddsLadders.Select(x => x.ContestType))}]");

        foreach (var oddsLadder in oddsLadders)
        {
            OrderKeysAndCache(oddsLadder, DomainConstants.GetOddsLadderContestCacheKey(oddsLadder.ContestType));
        }
    }

    private async Task<TResult> GetOddsLadderFromOfferManagerAsync<TResult>(string routeAction)
    {
        var route = $"{_restClient.BaseUrl}{routeAction}";
        _logger.LogInformation("Fetching Odds Ladder: {route}", route);

        try
        {
            RestResponse<TResult> result = null;

            await _apiRetryPolicy.ExecuteAsync(async () =>
            {
                result = await _restClient.ExecuteAsync<TResult>(new()
                {
                    Resource = routeAction
                });
            });

            return result.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to process odds ladder results from Event Manager. Exception={@exception}", ex);
            return default;
        }
    }

    #region Internal Cache

    public void Clear(string contestType)
    {
        contestType = contestType.ToLower();

        var cacheKey = DomainConstants.GetOddsLadderContestCacheKey(contestType);

        if (s_cache.ContainsKey(cacheKey))
        {
            _logger.LogInformation($"Purging cached odds ladder for contest type: {contestType.ToLower()}");

            s_cache.TryRemove(cacheKey, out _);
        }
    }

    private void OrderKeysAndCache(OddsLadder oddsLadder, string cacheKey)
    {
        try
        {
            var contestType = oddsLadder.ContestType ?? "default";
            _logger.LogInformation($"Caching odds ladder for contest type: {contestType.ToLower()}");

            oddsLadder.InPlayOddsLadder = oddsLadder.InPlayOddsLadder.OrderBy(o => o.Key).ToList();
            oddsLadder.PreGameOddsLadder = oddsLadder.PreGameOddsLadder.OrderBy(o => o.Key).ToList();

            s_cache.TryAdd(cacheKey, oddsLadder);
        }
        catch (Exception e)
        {
            _logger.LogWarning("Error while trying to set [{cacheKey}] in the Cache. Exception = {@exception}", cacheKey, e);
        }
    }
    #endregion


}
