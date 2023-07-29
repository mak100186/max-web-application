using System.ComponentModel.DataAnnotations;
using System.Net;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Helpers;
using Kindred.Rewards.Plugin.Discovery.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.Discovery.Controllers;

[ApiController]
[Route("[controller]")]
public class DiscoveryController : ControllerBase
{
    private readonly ILogger<DiscoveryController> _logger;
    public DiscoveryController(ILogger<DiscoveryController> logger)
    {
        _logger = logger;
    }

    [HttpGet("countries")]
    [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Countries()
    {
        _logger.LogInformation("DiscoveryController.Countries() called");

        var response = RegionHelper.GetCountriesByIso3166().Select(c => new CountryResponse
        {
            Name = c.EnglishName,
            Code = c.ThreeLetterISORegionName
        });

        return Ok(response);
    }

    [HttpGet]
    [Route("purposes")]
    [Route("/purposes")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ICollection<PurposeType>))]
    public IActionResult GetAllPurposes()
    {
        _logger.LogInformation("DiscoveryController.GetAllPurposes() called");

        var response = Enum.GetValues<PurposeType>().Select(x => new
        {
            DisplayName = x.GetAttributeValue<DisplayAttribute>("Description"),
            Value = x.ToString()

        }).ToList();

        return Ok(response);
    }

    [HttpGet]
    [Route("purposes/{purpose}")]
    [Route("/purposes/{purpose}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ICollection<PurposeType>))]
    public IActionResult GetSubPurposeByPurpose(PurposeType purpose)
    {
        _logger.LogInformation($"DiscoveryController.GetSubPurposeByPurpose({purpose}) called");

        var response = purpose.GetSubPurpose().Select(x => new
        {
            DisplayName = x.GetAttributeValue<DisplayAttribute>("Description"),
            Value = x.ToString()

        }).ToList();

        return Ok(response);
    }

}
