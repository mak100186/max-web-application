using Kindred.Rewards.Core.Mapping.Converters;

using Newtonsoft.Json;

using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Kindred.Rewards.Core.Infrastructure.Net;
public interface IRestClientWrapper
{
    public string BaseUrl { get; }

    Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request) => ExecuteAsync<T>(request, CancellationToken.None);
    Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken);
    Task<RestResponse> GetAsync(RestRequest request);
    Task<RestResponse> PostAsync(RestRequest request);
    Task<RestResponse> PutAsync(RestRequest request);
    Task<RestResponse> PatchAsync(RestRequest request);
    Task<RestResponse> DeleteAsync(RestRequest request);
}

public class RestClientWrapper : IRestClientWrapper
{
    private readonly RestClient _client;

    public string BaseUrl => _client.Options.BaseUrl?.AbsoluteUri;

    public RestClientWrapper(string baseUrl, int maxTimeout = default)
    {
        _client = maxTimeout == default ?
            new(new RestClientOptions { BaseUrl = new(baseUrl) }) :
            new(new RestClientOptions { BaseUrl = new(baseUrl), MaxTimeout = maxTimeout });

        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new RewardParameterApiModelBaseConverter()
            },
        };

        _client.UseNewtonsoftJson(settings);
    }

    public async Task<RestResponse> DeleteAsync(RestRequest request)
    {
        return await _client.DeleteAsync(request);
    }

    public async Task<RestResponse> PatchAsync(RestRequest request)
    {
        return await _client.PatchAsync(request);
    }

    public async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request) => await ExecuteAsync<T>(request, CancellationToken.None);

    public async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken)
    {
        return await _client.ExecuteAsync<T>(request, cancellationToken);
    }

    public async Task<RestResponse> GetAsync(RestRequest request)
    {
        return await _client.ExecuteGetAsync(request);
    }

    public async Task<RestResponse> PostAsync(RestRequest request)
    {
        return await _client.ExecutePostAsync(request);
    }

    public async Task<RestResponse> PutAsync(RestRequest request)
    {
        return await _client.ExecutePutAsync(request);
    }
}
