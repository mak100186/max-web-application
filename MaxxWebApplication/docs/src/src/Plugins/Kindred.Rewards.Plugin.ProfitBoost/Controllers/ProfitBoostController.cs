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
using Kindred.Rewards.Plugin.ProfitBoost.Models;
using Kindred.Rewards.Plugin.ProfitBoost.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.ProfitBoost.Controllers;

[ApiController]
[Route("Rewards/[controller]")]
public class ProfitBoostController : ControllerBase
{
    private readonly ILogger<ProfitBoostController> _logger;
    private readonly IMapper _mapper;
    private readonly IProfitBoostService _service;
    private readonly IConverter<string, RewardRn> _rewardRnConverter;

    public ProfitBoostController(ILogger<ProfitBoostController> logger,
        IMapper mapper,
        IProfitBoostService service,
        IConverter<string, RewardRn> rewardRnConverter)
    {
        _logger = logger;
        _mapper = mapper;
        _service = service;
        _rewardRnConverter = rewardRnConverter;
    }

    [HttpPost]
    [SwaggerRequestExample(typeof(ProfitBoostRewardRequest), typeof(CreateProfitBoostRewardRequestExamples))]
    [ProducesResponseType(typeof(ProfitBoostRewardResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] ProfitBoostRewardRequest request)
    {
        _logger.LogInformation("PostAsync() called");

        var createdDomainModel = await _service.CreateAsync(_mapper.Map<RewardDomainModel>(request));
        var response = _mapper.Map<ProfitBoostRewardResponse>(createdDomainModel);

        return Created(Url.Link(DomainConstants.GetRewardRoute, new { response.RewardRn }), response);
    }

    [HttpPut]
    [Route("{rewardRn}")]
    [SwaggerRequestExample(typeof(ProfitBoostRewardRequest), typeof(CreateProfitBoostRewardRequestExamples))]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProfitBoostRewardResponse))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiError))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiError))]
    public async Task<IActionResult> PutAsync(string rewardRn, [FromBody] ProfitBoostRewardRequest request)
    {
        _logger.LogInformation("PutAsync() called");

        var id = ConvertToIdOrThrow(rewardRn);

        var bonus = _mapper.Map<RewardDomainModel>(request);
        bonus.RewardId = id;

        return Ok(_mapper.Map<ProfitBoostRewardResponse>(await _service.UpdateAsync(bonus)));
    }

    [HttpPost]
    [Route("systemrewards")]
    [SwaggerRequestExample(typeof(ProfitBoostRewardRequest), typeof(CreateProfitBoostRewardRequestExamples))]
    [ProducesResponseType(typeof(ProfitBoostRewardResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateSystemRewardAsync([FromBody] ProfitBoostRewardRequest request)
    {
        if (request.CustomerId != null)
        {
            return BadRequest();
        }

        var reward = _mapper.Map<RewardDomainModel>(request);

        reward.IsSystemGenerated = true;

        var createdDomainModel = await _service.CreateAsync(reward);
        var response = _mapper.Map<ProfitBoostRewardResponse>(createdDomainModel);

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
