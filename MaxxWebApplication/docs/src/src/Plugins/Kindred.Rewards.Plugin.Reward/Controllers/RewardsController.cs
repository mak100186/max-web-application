using System.Net;

using AutoMapper;

using Kindred.Infrastructure.Hosting.WebApi.Extensions;
using Kindred.Infrastructure.Hosting.WebApi.Models;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Infrastructure.Net;
using Kindred.Rewards.Core.Mapping.Converters;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Rewards;
using Kindred.Rewards.Core.Models.Rn;
using Kindred.Rewards.Core.WebApi.Enums;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Responses;
using Kindred.Rewards.Plugin.Reward.Models;
using Kindred.Rewards.Plugin.Reward.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using RestSharp;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.Reward.Controllers;
[ApiController]
[Route("[controller]")]
public class RewardsController : ControllerBase
{
    private readonly ILogger<RewardsController> _logger;
    private readonly IMapper _mapper;
    private readonly IRewardService _service;
    private readonly ITemplateCancellationService _templateRewardService;
    private readonly IConverter<string, RewardRn> _rewardRnConverter;
    private readonly IRestClientWrapper _restClient;

    public RewardsController(ILogger<RewardsController> logger,
        IMapper mapper,
        IConfiguration configuration,
        IRewardService service,
        Func<string, IRestClientWrapper> restClientFactory,
        ITemplateCancellationService templateRewardService,
        IConverter<string, RewardRn> rewardRnConverter)
    {
        _logger = logger;
        _mapper = mapper;
        _service = service;

        var baseUrl = configuration[DomainConstants.WebApiUrl];
        _restClient = restClientFactory($"{baseUrl}/Rewards");
        _templateRewardService = templateRewardService;
        _rewardRnConverter = rewardRnConverter;
    }
    [HttpPost]
    [Route("")]
    [SwaggerRequestExample(typeof(RewardRequest), typeof(SwaggerRewardRequestExamples))]
    [ProducesResponseType(typeof(RewardResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] RewardRequest request)
    {
        RestRequest restRequest = new()
        {
            RequestFormat = DataFormat.Json,
            Method = Method.Post,
            Resource = $"/{request.RewardType}"
        };
        restRequest.AddJsonBody(request);
        var response = await _restClient.PostAsync(restRequest);

        if (response.IsSuccessStatusCode)
        {
            var rewardResponse = (IRewardResponse)JsonConvert.DeserializeObject(response.Content, GetResponseType(request.RewardType));
            return Created(Url.Link(DomainConstants.GetRewardRoute, new { rewardResponse.RewardRn }), rewardResponse);
        }

        try
        {
            var error = JsonConvert.DeserializeObject<ApiError>(response.Content);
            return BadRequest(error);
        }
        catch { }

        response.ThrowIfError();

        return BadRequest(response.ErrorMessage);
    }

    [HttpPatch]
    [Route("{rewardRn}")]
    [ProducesResponseType(typeof(RewardResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PatchAsync(string rewardRn, [FromBody] PatchRewardRequest request)
    {
        // Reward is being retrieved from database here
        // Then we retrieve reward type from reward
        // We need to know the reward type in order to call the right endpoint
        var id = ConvertToIdOrThrow(rewardRn);
        var existingReward = await _service.GetAsync(id);
        var rewardTypeToCall = existingReward.Type;

        // Create REST request
        RestRequest restRequest = new()
        {
            RequestFormat = DataFormat.Json,
            Method = Method.Patch,
            Resource = $"/{rewardTypeToCall}/{rewardRn}"
        };
        restRequest.AddJsonBody(request);

        // Call the REST endpoint
        var response = await _restClient.PatchAsync(restRequest);

        // Process response
        if (response.IsSuccessStatusCode)
        {
            return Ok(JsonConvert.DeserializeObject(response.Content!));
        }

        var apiError = JsonConvert.DeserializeObject<ApiError>(response.Content!);
        response.ThrowIfError();
        return BadRequest(apiError is null ? response.ErrorMessage : apiError);
    }

    [HttpPut]
    [Route("{rewardRn}")]
    [SwaggerRequestExample(typeof(RewardRequest), typeof(SwaggerRewardRequestExamples))]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RewardApiModel))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiError))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiError))]
    public async Task<IActionResult> PutAsync(string rewardRn, [FromBody] RewardRequest request)
    {
        RestRequest restRequest = new()
        {
            RequestFormat = DataFormat.Json,
            Method = Method.Put,
            Resource = $"/{request.RewardType}/{rewardRn}"
        };
        restRequest.AddJsonBody(request);
        var response = await _restClient.PutAsync(restRequest);

        if (response.IsSuccessStatusCode)
        {
            return Ok(JsonConvert.DeserializeObject(response.Content, GetResponseType(request.RewardType)));
        }

        try
        {
            var error = JsonConvert.DeserializeObject<ApiError>(response.Content);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(error);
            }
            return BadRequest(error);
        }
        catch { }

        response.ThrowIfError();

        return BadRequest(response.ErrorMessage);
    }

    [HttpGet]
    [Route("")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetRewardsResponse))]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetRewardsRequest request, [FromQuery] PagedRequest pagination)
    {
        var filter = request ?? new GetRewardsRequest();

        var domain = _mapper.Map<RewardFilterDomainModel>(filter);

        var bonuses = await _service.GetAllAsync(domain, pagination);
        var response = _mapper.Map<GetRewardsResponse>(bonuses);

        return Ok(response);
    }

    [HttpGet]
    [Route("{rewardRn}", Name = DomainConstants.GetRewardRoute)]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetRewardResponse))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiError))]
    public async Task<IActionResult> GetAsync(string rewardRn)
    {
        var id = ConvertToIdOrThrow(rewardRn);

        var bonus = await _service.GetAsync(id);

        var response = _mapper.Map<GetRewardResponse>(bonus);

        return Ok(response);
    }

    [HttpPut]
    [Route("{rewardRn}/cancel")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiError))]
    public async Task<IActionResult> CancelAsync(string rewardRn, [FromBody] CancelRequest request)
    {
        var id = ConvertToIdOrThrow(rewardRn);

        await _service.CancelAsync(id, request?.Reason);
        await _templateRewardService.RemoveCancelledPromotionAsync(id);

        return Ok();
    }

    [HttpGet]
    [Route("{type}/defaults")]
    [ProducesResponseType(typeof(GetAllowedMarketTypesResponse), (int)HttpStatusCode.OK)]
    public IActionResult GetDefaults(string type)
    {
        var allowedMarketTypesForPromo = _service.GetDefaults(type);

        var response = _mapper.Map<GetAllowedMarketTypesResponse>(allowedMarketTypesForPromo);

        return Ok(response);
    }

    [HttpGet]
    [Route("types")]
    [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
    public IActionResult GetAllRewardTypes()
    {
        return Ok(Enum.GetNames(typeof(RewardType)));
    }

    [HttpPost]
    [Route("systemrewards")]
    [SwaggerRequestExample(typeof(RewardRequest), typeof(SwaggerRewardRequestExamples))]
    [ProducesResponseType(typeof(RewardResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateSystemRewardsync([FromBody] RewardRequest request)
    {
        if (request.CustomerId != null)
        {
            return BadRequest();
        }

        var reward = _mapper.Map<RewardDomainModel>(request);

        reward.IsSystemGenerated = true;

        var created = await _service.CreateAsync(reward);
        var apiModel = _mapper.Map<RewardResponse>(created);

        return Created(Url.Link(DomainConstants.GetRewardRoute, new { apiModel.RewardRn }), apiModel);
    }

    [HttpPut]
    [Route("systemrewards/{rewardId}/lock")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiError))]
    public async Task<IActionResult> LockAsync(string rewardId)
    {
        await _service.LockAsync(rewardId);

        return NoContent();
    }

    private string ConvertToIdOrThrow(string rewardRnOrId)
    {
        var id = _rewardRnConverter.Convert(rewardRnOrId)?.GuidValue;
        if (id == null)
        {
            throw new RewardRnInvalidException(rewardRnOrId);
        }

        return id;
    }

    private static Type GetResponseType(string rewardType)
    {
        return rewardType switch
        {
            nameof(RewardType.Freebet) => typeof(FreeBetRewardResponse),
            nameof(RewardType.Uniboost) => typeof(UniBoostRewardResponse),
            nameof(RewardType.UniboostReload) => typeof(UniBoostReloadRewardResponse),
            nameof(RewardType.Profitboost) => typeof(ProfitBoostRewardResponse),
            _ => throw new ArgumentException(nameof(rewardType))
        };
    }
}
