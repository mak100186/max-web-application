using AutoMapper;

using Kindred.Infrastructure.Hosting.WebApi.Exceptions;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Plugin.Claim.Exceptions;
using Kindred.Rewards.Plugin.Claim.Mappings;
using Kindred.Rewards.Plugin.Claim.Models.Dto;
using Kindred.Rewards.Plugin.Claim.Models.Requests;
using Kindred.Rewards.Plugin.Claim.Models.Responses;
using Kindred.Rewards.Plugin.Claim.Models.Responses.SettleClaim;
using Kindred.Rewards.Plugin.Claim.Services.Strategies;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.Claim.Services;

public interface IClaimSettleService
{
    public Task<SettleClaimResponse> SettleClaimAsync(SettleClaimRequest request);
}

public class ClaimSettleService : IClaimSettleService
{
    private readonly RewardsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ClaimSettleService> _logger;
    private readonly IRewardClaimStrategyFactory _claimStrategyFactory;

    public ClaimSettleService(
        RewardsDbContext context,
        IMapper mapper,
        ILogger<ClaimSettleService> logger,
        IRewardClaimStrategyFactory claimStrategyFactory)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _claimStrategyFactory = claimStrategyFactory;
    }

    public async Task<SettleClaimResponse> SettleClaimAsync(SettleClaimRequest request)
    {
        var claim = await GetClaimedAsync(request);

        if (claim == null)
        {
            _logger.LogWarning($"{nameof(SettleClaimAsync)}:Could not find reward claim " +
                               $"with Rn {request.RewardClaims.FirstOrDefault()?.ClaimRn}. " +
                               "Either it doesn't exist or already settled.");

            throw new ClaimNotFoundException(request.RewardClaims.FirstOrDefault()?.ClaimRn ?? string.Empty);
        }

        var prevPayoffs = GetPreviouslyCalculatedRewardPayoffs(claim);

        var result = await SettleCombinations(request, claim);

        return new()
        {
            RewardClaimSettlement = new() { new() { Payoff = result.Select(x => x.Payoff).Sum() } },
            PrevRewardClaimSettlement = new() { new() { Payoff = prevPayoffs.Select(x => x?.CombinationPayoff).Sum() } }
        };
    }

    private async Task<RewardClaimDomainModel?> GetClaimedAsync(SettleClaimRequest request)
    {
        var claimRn = request.RewardClaims.FirstOrDefault()?.ClaimRn;

        if (string.IsNullOrEmpty(request.RewardClaims.FirstOrDefault()?.ClaimRn))
        {
            return null;
        }

        var data = await _context.RewardClaims
            .Where(claim => claim.InstanceId == claimRn && claim.Status == nameof(RewardClaimStatus.Claimed))
            .Include(claim => claim.CombinationRewardPayoffs)
            .OrderByDescending(claim => claim.CreatedOn)
            .FirstOrDefaultAsync() ?? default;

        return data != null ? _mapper.Map<RewardClaimDomainModel>(data) : default;
    }

    private async Task<List<SettleClaimResultDto>> SettleCombinations(SettleClaimRequest request,
        RewardClaimDomainModel claim)
    {
        var result = new List<SettleClaimResultDto>();

        if (!request.AcceptedCombinations.Any())
        {
            _logger.LogWarning("No combinations found to settle for the request: {@request}", request);

            throw new BadRequestException("No combinations were found to settle for the request.");
        }

        var rewardStrategy = _claimStrategyFactory.GetRewardStrategy(claim.Type);

        foreach (var combination in request.AcceptedCombinations)
        {
            var combinationStages = request.Stages
                .Where(stage => combination.Selections
                    .Select(combinationSelection => combinationSelection.Outcome)
                    .Contains(stage.Selection.Outcome));

            var combinationResult =
                await rewardStrategy.ProcessClaimAsync(request.ToDto(combinationStages, combination, claim));

            result.Add(combinationResult);
        }

        return result;
    }

    private IEnumerable<CombinationRewardPayoffDomainModel?> GetPreviouslyCalculatedRewardPayoffs(RewardClaimDomainModel claim)
    {
        return claim.CombinationRewardPayoffs
            .Where(x => x.ClaimInstanceId == claim.InstanceId)
            .GroupBy(z => z.CombinationRn)
            .Select(z => z.MaxBy(comb => comb.CreatedOn))
            .ToList();
    }
}
