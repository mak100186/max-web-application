using System.Net;

using AutoMapper;

using Kindred.Infrastructure.Hosting.WebApi.Extensions;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Responses;
using Kindred.Rewards.Plugin.Claim.Models;
using Kindred.Rewards.Plugin.Claim.Models.Requests;
using Kindred.Rewards.Plugin.Claim.Models.Responses;
using Kindred.Rewards.Plugin.Claim.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.Claim.Controllers;

[ApiController]
[Route("[controller]")]
[Route("RewardClaims")]
public class ClaimsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IClaimService _service;
    private readonly ILogger<ClaimsController> _logger;
    private readonly IClaimSettleService _claimSettleService;

    public ClaimsController(ILogger<ClaimsController> logger,
        IMapper mapper,
        IClaimService service,
        IClaimSettleService claimSettleService)
    {
        _mapper = mapper;
        _logger = logger;
        _service = service;
        _claimSettleService = claimSettleService;
    }

    /// <summary>
    /// Batch claim all the rewards for a customer on the specified bet coupon.
    /// This operation is atomic, so if any claim requests fail, all will fail
    /// </summary>
    /// <param name="batchClaimRequest">Batch claim request for a customer</param>
    /// <returns>either 200, 404 or 50x</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BatchClaimResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerRequestExample(typeof(BatchClaimRequest), typeof(BatchClaimRequestExamples))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [Route("batchclaim")]
    public async Task<IActionResult> BatchClaimAsync([FromBody] BatchClaimRequest batchClaimRequest)
    {
        if (batchClaimRequest?.Claims != null && batchClaimRequest.Claims.Count == 0)
        {
            _logger.LogWarning("BatchClaim call made on an empty claim collection");
        }

        var batchClaimDomainModel = _mapper.Map<BatchClaimDomainModel>(batchClaimRequest);
        var batchClaimResultDomainModel = await _service.ClaimEntitlementsAsync(batchClaimDomainModel);
        var batchClaimResponse = _mapper.Map<BatchClaimResponse>(batchClaimResultDomainModel);

        return !batchClaimResultDomainModel.AllRewardsFound
            ? NotFound(batchClaimResponse)
            : batchClaimResultDomainModel.ClaimResults.Any(r => !r.Success)
                ? BadRequest(batchClaimResponse)
                : Ok(batchClaimResponse);
    }

    /// <summary>
    /// Returns all the reward claims for the given filter criteria request
    /// </summary>
    /// <param name="request">filter request</param>
    /// <returns>List of reward claims</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetClaimsResponse), (int)HttpStatusCode.OK)]
    [Route("")]
    public async Task<IActionResult> GetClaimsAsync([FromQuery] GetClaimsByFilterRequest request, [FromQuery] PagedRequest pagination)
    {
        var filter = _mapper.Map<RewardClaimFilterDomainModel>(request) ?? new RewardClaimFilterDomainModel();

        var claims = await _service.GetClaimsAsync(filter, pagination);

        var response = _mapper.Map<GetClaimsResponse>(claims);

        return Ok(response);
    }

    /// <summary>
    /// Gets a collection of active rewards which a customer is entitled to
    /// </summary>
    /// <param name="customerId">Customer Id</param>
    /// <param name="templateKeys">List of Promotion Template Keys that customer is entitled to</param>
    /// <returns>Returns a collection of active rewards which a customer is entitled to</returns>
    [HttpGet]
    [ProducesResponseType(typeof(CustomerEntitlementsResponse), (int)HttpStatusCode.OK)]
    [Route("entitlements/{customerId}")]
    public async Task<IActionResult> GetEntitlementsAsync([FromRoute] string customerId, [FromQuery] List<string> templateKeys)
    {
        var response = new CustomerEntitlementsResponse
        {
            Entitlements = new List<RewardEntitlementApiModel>()
        };

        var rewards = await _service.GetEntitlementsAsync(customerId, templateKeys);

        foreach (var reward in rewards)
        {
            var r = _mapper.Map<RewardEntitlementApiModel>(reward);
            r.CustomerId = customerId;

            response.Entitlements.Add(r);
        }

        return Ok(response);
    }

    /// <summary>
    /// Accepts request that contains information related to a combination settlement occurrence and corresponding rewards that were claimed.
    /// </summary>
    /// <param name="request">combination settlement occurrence</param>
    /// <returns>Response containing reward payout as a payoff or a reward.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SettleClaimResponse), (int)HttpStatusCode.OK)]
    [Route("settle")]
    public async Task<IActionResult> SettleClaimAsync([FromBody] SettleClaimRequest request)
    {
        var response = await _claimSettleService.SettleClaimAsync(request);

        return Ok(response);
    }
}
