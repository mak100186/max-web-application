using System.Reflection;

using FluentAssertions;

using Kindred.Rewards.Core.Infrastructure.Net;
using Kindred.Rewards.Rewards.Tests.Common;

using Newtonsoft.Json;

using RestSharp;

using TechTalk.SpecFlow;

namespace Kindred.Rewards.Rewards.FunctionalTests.Common.Helpers;

public interface IApiClient
{
    Task<TResponse> GetAsync<TResponse>(params string[] args);

    Task<TResponse> GetAsync<TResponse>(string resource, List<Tuple<string, string>> queryString);

    Task<TResponse> GetAllAsync<TResponse>();

    Task<TResponse> GetAllAsync<TRequest, TResponse>(TRequest request);

    Task PostAsync<TRequest>(TRequest request) where TRequest : class;

    Task<TResponse> PostAsync<TRequest, TResponse>(TRequest request, ICollection<KeyValuePair<string, string>> headers) where TRequest : class;

    Task<TResponse> PostAsync<TRequest, TResponse>(TRequest request) where TRequest : class;

    Task<TResponse> PatchAsync<TRequest, TResponse>(string resource, TRequest request) where TRequest : class;

    Task<TResponse> PostAsync<TRequest, TResponse>(string resource, TRequest request) where TRequest : class;

    Task PutAsync<TRequest>(string resource, TRequest request) where TRequest : class;

    Task PutAsync<TRequest>(string resource, TRequest request,
        ICollection<KeyValuePair<string, string>> headers) where TRequest : class;

    Task DeleteAsync(string resource);

    Task<RestResponse> GetAsync(string resource);
}

public class ApiClient : IApiClient
{
    private readonly IRestClientWrapper _restClient;
    private readonly ScenarioContext _scenarioContext;

    public ApiClient(string urlSuffix, Func<string, IRestClientWrapper> restClientFactory, ScenarioContext scenarioContext,
        Func<JsonSerializerSettings> jsonSerializerSettings)
    {
        _scenarioContext = scenarioContext;

        _restClient = restClientFactory(urlSuffix);

        JsonConvert.DefaultSettings = jsonSerializerSettings;
    }


    public async Task<RestResponse> GetAsync(string resource)
    {
        var request = new RestRequest
        {
            Resource = resource
        };

        var response = await _restClient.GetAsync(request);
        SaveResponse(response);

        return response;
    }

    public async Task<TResponse> GetAsync<TResponse>(params string[] args)
    {
        var request = new RestRequest { Resource = args[0] };

        var response = await _restClient.GetAsync(request);

        SaveResponse(response);

        return JsonConvert.DeserializeObject<TResponse>(response.Content);
    }

    public async Task<TResponse> GetAsync<TResponse>(string resource, List<Tuple<string, string>> queryString)
    {
        var request = new RestRequest
        {
            Resource = resource
        };

        foreach (var key in queryString)
        {
            request.AddParameter(key.Item1.ToLowerInvariant(), key.Item2);
        }

        var response = await _restClient.GetAsync(request);

        SaveResponse(response);

        return JsonConvert.DeserializeObject<TResponse>(response.Content);
    }

    public async Task<TResponse> GetAllAsync<TResponse>()
    {
        var request = new RestRequest();

        var response = await _restClient.GetAsync(request);

        SaveResponse(response);

        return JsonConvert.DeserializeObject<TResponse>(response.Content);
    }

    public async Task<TResponse> GetAllAsync<TRequest, TResponse>(TRequest request)
    {
        var filterRequest = new RestRequest();

        var props = new List<PropertyInfo>();

        var currType = request.GetType();
        while (currType != typeof(object))
        {
            props.AddRange(currType.Properties().ToArray());
            currType = currType.BaseType;
        }

        foreach (var prop in props)
        {
            var key = prop.Name.ToLowerInvariant();

            if (prop.PropertyType == typeof(DateTime) ||
                prop.PropertyType == typeof(DateTime?))
            {
                var value = prop.GetValue(request, null) as DateTime?;
                filterRequest.AddQueryParameter(key, value?.ToString("O"));
            }
            else if (prop.PropertyType == typeof(DateTimeOffset) ||
                     prop.PropertyType == typeof(DateTimeOffset?))
            {
                var value = prop.GetValue(request, null) as DateTimeOffset?;
                filterRequest.AddQueryParameter(key, value?.ToString("O"));
            }
            else
            {
                var value = prop.GetValue(request, null)?.ToString();
                filterRequest.AddQueryParameter(key, value);
            }
        }

        var response = await _restClient.GetAsync(filterRequest);

        SaveResponse(response);

        return JsonConvert.DeserializeObject<TResponse>(response.Content);
    }

    public async Task<TResponse> PatchAsync<TRequest, TResponse>(string resource, TRequest request) where TRequest : class
    {
        var patchRequest = new RestRequest
        {
            Resource = resource,
            RequestFormat = DataFormat.Json,
            Method = Method.Patch
        };
        patchRequest.AddJsonBody(request);

        var response = await _restClient.PatchAsync(patchRequest);

        SaveResponse(response);

        return JsonConvert.DeserializeObject<TResponse>(response.Content!);
    }

    public async Task PostAsync<TRequest>(TRequest request) where TRequest : class
    {
        var postRequest = new RestRequest
        {
            RequestFormat = DataFormat.Json,
            Method = Method.Post
        };
        postRequest.AddJsonBody(request);
        var response = await _restClient.PostAsync(postRequest);

        SaveResponse(response);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(TRequest request,
        ICollection<KeyValuePair<string, string>> headers) where TRequest : class
    {
        var postRequest = new RestRequest
        {
            RequestFormat = DataFormat.Json,
            Method = Method.Post
        };
        postRequest.AddJsonBody(request);

        var response = await _restClient.PostAsync(postRequest);
        postRequest.AddHeaders(headers);

        SaveResponse(response);

        return JsonConvert.DeserializeObject<TResponse>(response.Content);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(TRequest request) where TRequest : class
    {
        return await PostAsync<TRequest, TResponse>(null, request);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string resource, TRequest request) where TRequest : class
    {
        var postRequest = new RestRequest
        {
            Resource = resource,
            RequestFormat = DataFormat.Json,
            Method = Method.Post
        };
        postRequest.AddJsonBody(request);

        var response = await _restClient.PostAsync(postRequest);

        SaveResponse(response);

        return JsonConvert.DeserializeObject<TResponse>(response.Content);
    }

    public async Task PutAsync<TRequest>(string resource, TRequest request) where TRequest : class
    {
        var putRequest = new RestRequest
        {
            Resource = resource,
            RequestFormat = DataFormat.Json,
            Method = Method.Put
        };
        putRequest.AddJsonBody(request);

        var response = await _restClient.PutAsync(putRequest);

        SaveResponse(response);
    }

    public async Task PutAsync<TRequest>(string resource, TRequest request,
        ICollection<KeyValuePair<string, string>> headers) where TRequest : class
    {
        var putRequest = new RestRequest
        {
            Resource = resource,
            RequestFormat = DataFormat.Json,
            Method = Method.Put
        };
        putRequest.AddJsonBody(request);
        putRequest.AddHeaders(headers);

        var response = await _restClient.PutAsync(putRequest);

        SaveResponse(response);
    }

    public async Task DeleteAsync(string resource)
    {
        var deleteRequest = new RestRequest
        {
            Resource = resource,
            RequestFormat = DataFormat.Json,
            Method = Method.Delete
        };

        var response = await _restClient.DeleteAsync(deleteRequest);

        SaveResponse(response);
    }

    private void SaveResponse(RestResponse response)
    {
        if (_scenarioContext == null)
        {
            return;
        }

        _scenarioContext[TestConstants.RestResponseSharedState] = response.StatusCode;
        //TODO Use RestResponse everywhere rather than individual properties stored to clean up a bunch of code.
        _scenarioContext[TestConstants.RestResponse] = response;

        if (!response.IsSuccessful)
        {
            _scenarioContext[TestConstants.RestResponseSharedError] = response.Content;
        }
    }
}
