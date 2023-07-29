using System.Net;

using AutoMapper;

using Kindred.Infrastructure.Hosting.WebApi.Extensions;
using Kindred.Infrastructure.Hosting.WebApi.Models;
using Kindred.Rewards.Core.Authorization;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Mapping.Converters;
using Kindred.Rewards.Core.Models.Promotions;
using Kindred.Rewards.Core.Models.Rewards;
using Kindred.Rewards.Core.Models.Rn;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Responses;
using Kindred.Rewards.Plugin.Template.Models;
using Kindred.Rewards.Plugin.Template.Services;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.Template.Controllers;

[ApiController]
[Route("[controller]")]
public class TemplatesController : ControllerBase
{
    private const string GetTemplateRoute = "GetTemplateRoute";

    private readonly IMapper _mapper;
    private readonly ITemplateCrudService _service;
    private readonly IConverter<string, RewardRn> _rewardRnConverter;
    private readonly IAuthorisationService _authorisationService;

    public TemplatesController(
        IMapper mapper,
        ITemplateCrudService service,
        IConverter<string, RewardRn> rewardRnConverter,
        IAuthorisationService authorisationService)
    {
        _mapper = mapper;
        _service = service;
        _rewardRnConverter = rewardRnConverter;
        _authorisationService = authorisationService;
    }

    [HttpGet]
    [Route("")]
    [Route("/RewardTemplates")]
    [ProducesResponseType(typeof(GetPromotionTemplatesResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetPromotionTemplatesRequest request, [FromQuery] PagedRequest pagination)
    {
        var filter = request ?? new GetPromotionTemplatesRequest();
        var domain = _mapper.Map<PromotionTemplateFilterDomainModel>(filter);

        var domainModels = await _service.GetAllAsync(domain, pagination);
        var response = _mapper.Map<GetPromotionTemplatesResponse>(domainModels);
        return Ok(response);
    }

    [HttpGet]
    [Route("{templateKey}", Name = GetTemplateRoute)]
    [Route("/RewardTemplates/{templateKey}")]
    [ProducesResponseType(typeof(RewardTemplateResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAsync(string templateKey, bool? isActive = null)
    {
        var domainModel = await _service.GetAsync(templateKey, isActive);
        var response = _mapper.Map<RewardTemplateResponse>(domainModel);

        return Ok(response);
    }

    [HttpPost]
    [Route("")]
    [Route("/RewardTemplates")]
    [SwaggerRequestExample(typeof(CreateRewardTemplateRequest), typeof(CreateRewardTemplateRequestExamples))]
    [ProducesResponseType(typeof(RewardTemplateApiModel), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.Conflict)]
    [AuthoriseAttribute]
    public async Task<IActionResult> PostAsync([FromBody] CreateRewardTemplateRequest request)
    {
        var template = _mapper.Map<RewardTemplateDomainModel>(request);

        var user = _authorisationService.GetUsername();
        if (user is not null)
        {
            template.CreatedBy = user;
            template.UpdatedBy = user;
        }

        var created = await _service.CreateAsync(template);

        var apiModel = _mapper.Map<RewardTemplateApiModel>(created);

        return Created(Url.Link(GetTemplateRoute, new { templateKey = request.TemplateKey }), apiModel);
    }


    [HttpPut]
    [Route("{templateKey}/mapping/")]
    [Route("/RewardTemplates/{templateKey}/mapping/")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.Conflict)]
    [AuthoriseAttribute]
    public async Task<IActionResult> MapAsync(string templateKey, [FromBody] UpdatePromotionTemplateMapRequest request)
    {
        var user = _authorisationService.GetUsername();

        var (rewardRns, invalidRn) = _rewardRnConverter.Convert(request.RewardRns);

        if (invalidRn.Any())
        {
            throw new RewardRnInvalidException(invalidRn);
        }

        await _service.UpdateMappingAsync(templateKey, rewardRns.Select(x => x.GuidValue).ToList(), user);

        return Ok();
    }

    [HttpDelete]
    [Route("{templateKey}")]
    [Route("/RewardTemplates/{templateKey}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteAsync(string templateKey)
    {
        await _service.DeleteAsync(templateKey);

        return Ok();
    }
}
