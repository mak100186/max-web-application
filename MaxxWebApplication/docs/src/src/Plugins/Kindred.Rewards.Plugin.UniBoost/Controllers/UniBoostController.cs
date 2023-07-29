using System.Net;

using AutoMapper;

using Kindred.Infrastructure.Hosting.WebApi.Models;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Mapping.Converters;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Rewards;
using Kindred.Rewards.Core.Models.Rn;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Plugin.UniBoost.Models;
using Kindred.Rewards.Plugin.UniBoost.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.UniBoost.Controllers;
[ApiController]
[Route("Rewards/[controller]")]
public class UniBoostController : ControllerBase
{
    private readonly ILogger<UniBoostController> _logger;
    private readonly IMapper _mapper;
    private readonly IUniBoostService _service;
    private readonly IConverter<string, RewardRn> _rewardRnConverter;

    public UniBoostController(ILogger<UniBoostController> logger,
        IMapper mapper,
        IUniBoostService service,
        IConverter<string, RewardRn> rewardRnConverter)
    {
        _logger = logger;
        _mapper = mapper;
        _service = service;
        _rewardRnConverter = rewardRnConverter;
    }

    [HttpPost]
    [SwaggerRequestExample(typeof(UniBoostRewardRequest), typeof(CreateUniBoostRewardRequestExamples))]
    [ProducesResponseType(typeof(UniBoostRewardResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] UniBoostRewardRequest request)
    {
        _logger.LogInformation("PostAsync() called");

        var createdDomainModel = await _service.CreateAsync(_mapper.Map<RewardDomainModel>(request));
        var response = _mapper.Map<UniBoostRewardResponse>(createdDomainModel);

        return Created(Url.Link(DomainConstants.GetRewardRoute, new { response.RewardRn }), response);
    }

    [HttpPut]
    [Route("{rewardRn}")]
    [SwaggerRequestExample(typeof(UniBoostRewardRequest), typeof(CreateUniBoostRewardRequestExamples))]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UniBoostRewardResponse))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiError))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiError))]
    public async Task<IActionResult> PutAsync(string rewardRn, [FromBody] UniBoostRewardRequest request)
    {
        _logger.LogInformation("PutAsync() called");

        var id = ConvertToIdOrThrow(rewardRn);

        var bonus = _mapper.Map<RewardDomainModel>(request);
        bonus.RewardId = id;

        return Ok(_mapper.Map<UniBoostRewardResponse>(await _service.UpdateAsync(bonus)));
    }

    [HttpPost]
    [Route("systemrewards")]
    [SwaggerRequestExample(typeof(UniBoostRewardRequest), typeof(CreateUniBoostRewardRequestExamples))]
    [ProducesResponseType(typeof(UniBoostRewardResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateRewardAsync([FromBody] UniBoostRewardRequest request)
    {
        if (request.CustomerId != null)
        {
            return BadRequest();
        }

        var reward = _mapper.Map<RewardDomainModel>(request);

        reward.IsSystemGenerated = true;

        var createdDomainModel = await _service.CreateAsync(reward);
        var response = _mapper.Map<UniBoostRewardResponse>(createdDomainModel);

        return Created(Url.Link(DomainConstants.GetRewardRoute, new { response.RewardRn }), response);
    }


    [HttpPatch]
    [Route("{rewardRn}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RewardApiModel))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiError))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiError))]
    public async Task<IActionResult> PatchAsync(string rewardRn, [FromBody] PatchRewardRequest request)
    {
        var id = ConvertToIdOrThrow(rewardRn);
        var patchDomainModel = _mapper.Map<RewardPatchDomainModel>(request);

        var updated = await _service.PatchAsync(id, patchDomainModel);

        var apiModel = _mapper.Map<RewardApiModel>(updated);
        return Ok(apiModel);
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
}
