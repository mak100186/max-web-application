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
using Kindred.Rewards.Plugin.FreeBet.Models;
using Kindred.Rewards.Plugin.FreeBet.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.FreeBet.Controllers;
[ApiController]
[Route("Rewards/[controller]")]
public class FreeBetController : ControllerBase
{
    private readonly ILogger<FreeBetController> _logger;
    private readonly IMapper _mapper;
    private readonly IFreeBetService _service;
    private readonly IConverter<string, RewardRn> _rewardRnConverter;

    public FreeBetController(ILogger<FreeBetController> logger,
        IMapper mapper,
        IFreeBetService service,
        IConverter<string, RewardRn> rewardRnConverter)
    {
        _logger = logger;
        _mapper = mapper;
        _service = service;
        _rewardRnConverter = rewardRnConverter;
    }

    [HttpPost]
    [SwaggerRequestExample(typeof(FreeBetRewardRequest), typeof(CreateFreeBetRewardRequestExamples))]
    [ProducesResponseType(typeof(FreeBetRewardResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] FreeBetRewardRequest request)
    {
        _logger.LogInformation("PostAsync() called");

        var createdDomainModel = await _service.CreateAsync(_mapper.Map<RewardDomainModel>(request));
        var response = _mapper.Map<FreeBetRewardResponse>(createdDomainModel);

        return Created(Url.Link(DomainConstants.GetRewardRoute, new { response.RewardRn }), response);
    }

    [HttpPut]
    [Route("{rewardRn}")]
    [SwaggerRequestExample(typeof(FreeBetRewardRequest), typeof(CreateFreeBetRewardRequestExamples))]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FreeBetRewardResponse))]
    [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ApiError))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiError))]
    public async Task<IActionResult> PutAsync(string rewardRn, [FromBody] FreeBetRewardRequest request)
    {
        _logger.LogInformation("PutAsync() called");

        var id = ConvertToIdOrThrow(rewardRn);

        var bonus = _mapper.Map<RewardDomainModel>(request);
        bonus.RewardId = id;

        return Ok(_mapper.Map<FreeBetRewardResponse>(await _service.UpdateAsync(bonus)));
    }

    [HttpPost]
    [Route("systemrewards")]
    [SwaggerRequestExample(typeof(FreeBetRewardRequest), typeof(CreateFreeBetRewardRequestExamples))]
    [ProducesResponseType(typeof(FreeBetRewardResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateSystemRewardAsync([FromBody] FreeBetRewardRequest request)
    {
        if (request.CustomerId != null)
        {
            return BadRequest();
        }

        var reward = _mapper.Map<RewardDomainModel>(request);

        reward.IsSystemGenerated = true;

        var createdDomainModel = await _service.CreateAsync(reward);
        var response = _mapper.Map<FreeBetRewardResponse>(createdDomainModel);

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
