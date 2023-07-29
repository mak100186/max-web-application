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
using Kindred.Rewards.Plugin.UniBoostReload.Models;
using Kindred.Rewards.Plugin.UniBoostReload.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.UniBoostReload.Controllers;

[ApiController]
[Route("Rewards/[controller]")]
public class UniBoostReloadController : ControllerBase
{
    private readonly ILogger<UniBoostReloadController> _logger;
    private readonly IMapper _mapper;
    private readonly IUniBoostReloadService _reloadService;
    private readonly IConverter<string, RewardRn> _rewardRnConverter;

    public UniBoostReloadController(ILogger<UniBoostReloadController> logger,
        IMapper mapper,
        IUniBoostReloadService reloadService,
        IConverter<string, RewardRn> rewardRnConverter)
    {
        _logger = logger;
        _mapper = mapper;
        _reloadService = reloadService;
        _rewardRnConverter = rewardRnConverter;
    }

    [HttpPost]
    [SwaggerRequestExample(typeof(UniBoostReloadRewardRequest), typeof(CreateUniBoostReloadRewardRequestExamples))]
    [ProducesResponseType(typeof(UniBoostReloadRewardResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] UniBoostReloadRewardRequest request)
    {
        _logger.LogInformation("PostAsync() called");

        var createdDomainModel = await _reloadService.CreateAsync(_mapper.Map<RewardDomainModel>(request));
        var response = _mapper.Map<UniBoostReloadRewardResponse>(createdDomainModel);

        return Created(Url.Link(DomainConstants.GetRewardRoute, new { response.RewardRn }), response);
    }

    [HttpPut]
    [Route("{rewardRn}")]
    [SwaggerRequestExample(typeof(UniBoostReloadRewardRequest), typeof(CreateUniBoostReloadRewardRequestExamples))]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UniBoostReloadRewardResponse))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiError))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiError))]
    public async Task<IActionResult> PutAsync(string rewardRn, [FromBody] UniBoostReloadRewardRequest request)
    {
        _logger.LogInformation("PutAsync() called");

        var id = ConvertToIdOrThrow(rewardRn);

        var bonus = _mapper.Map<RewardDomainModel>(request);
        bonus.RewardId = id;

        return Ok(_mapper.Map<UniBoostReloadRewardResponse>(await _reloadService.UpdateAsync(bonus)));
    }

    [HttpPost]
    [Route("systemrewards")]
    [SwaggerRequestExample(typeof(UniBoostReloadRewardRequest), typeof(CreateUniBoostReloadRewardRequestExamples))]
    [ProducesResponseType(typeof(UniBoostReloadRewardResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateSystemRewardAsync([FromBody] UniBoostReloadRewardRequest request)
    {
        if (request.CustomerId != null)
        {
            return BadRequest();
        }

        var reward = _mapper.Map<RewardDomainModel>(request);

        reward.IsSystemGenerated = true;

        var createdDomainModel = await _reloadService.CreateAsync(reward);
        var response = _mapper.Map<UniBoostReloadRewardResponse>(createdDomainModel);

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

        var updated = await _reloadService.PatchAsync(id, patchDomainModel);

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
