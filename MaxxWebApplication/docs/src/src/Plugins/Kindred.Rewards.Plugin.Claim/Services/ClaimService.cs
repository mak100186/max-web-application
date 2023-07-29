using AutoMapper;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Infrastructure.Hosting.WebApi.Extensions;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Helpers;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Infrastructure.Data.Exceptions;
using Kindred.Rewards.Core.Infrastructure.Data.Extensions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.RewardClaims.Bet;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.Models.Rn;
using Kindred.Rewards.Plugin.Claim.Clients.MarketMirror;
using Kindred.Rewards.Plugin.Claim.Clients.MarketMirror.Responses;
using Kindred.Rewards.Plugin.Claim.Services.Strategies;

using LinqKit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Polly;
using Polly.Retry;

namespace Kindred.Rewards.Plugin.Claim.Services;

public interface IClaimService
{
    Task<IList<RewardClaimDomainModel>> GetEntitlementsAsync(string customerId, IReadOnlyCollection<string> promotionTemplateKeys);

    Task<BatchClaimResultDomainModel> ClaimEntitlementsAsync(BatchClaimDomainModel claimRequest);

    Task<PagedResponse<RewardClaimDomainModel>> GetClaimsAsync(RewardClaimFilterDomainModel filter, PagedRequest pagination);
}

public class ClaimService : IClaimService
{
    private readonly RewardsDbContext _context;

    private readonly IMapper _mapper;
    private readonly ILogger<ClaimService> _logger;
    private readonly IRewardClaimStrategyFactory _claimStrategyFactory;
    private readonly IMarketMirrorClient _marketMirrorClient;

    public ClaimService(RewardsDbContext context, IMapper mapper, ILogger<ClaimService> logger,
        IRewardClaimStrategyFactory claimStrategyFactory, IMarketMirrorClient marketMirrorClient)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _claimStrategyFactory = claimStrategyFactory;
        _marketMirrorClient = marketMirrorClient;
    }

    public async Task<PagedResponse<RewardClaimDomainModel>> GetClaimsAsync(RewardClaimFilterDomainModel filter, PagedRequest pagination)
    {
        _logger.LogInformation("Fetching claims for given filter request {@request}", filter);

        var claims = await GetByFilter(filter, pagination);

        return claims;
    }

    public async Task<IList<RewardClaimDomainModel>> GetEntitlementsAsync(string customerId, IReadOnlyCollection<string> promotionTemplateKeys)
    {
        var rewardClaims = new List<RewardClaimDomainModel>();
        var rewardIntervals = new List<long>();

        var bonuses = await FindActiveRewardsAsync(customerId, DateTime.UtcNow);

        var promotions = await FindActiveRewardsAsync(promotionTemplateKeys, DateTime.UtcNow);

        var customerRewards = await FindActiveCustomerRewardsAsync(customerId, DateTime.UtcNow);

        await UpdateRewardClaimsAsync(customerId, bonuses, rewardIntervals, rewardClaims);
        await UpdateRewardClaimsAsync(customerId, promotions, rewardIntervals, rewardClaims, true);
        await UpdateRewardClaimsAsync(customerId, customerRewards, rewardIntervals, rewardClaims);

        await GetRetryPolicy("Could not save customer promotion templates", customerId, promotionTemplateKeys)
            .ExecuteAsync(
                async () =>
                {
                    await UpdateCustomerPromotionTemplatesAsync(promotionTemplateKeys, customerId);
                    await _context.SaveChangesAsync();
                });

        return GetFilteredResult(rewardClaims);
    }

    private DateTime GetNextInterval(RewardRestriction restrictions)
    {
        return new(CronService.GetNextIntervalInAllowedPeriod(restrictions.ClaimInterval, restrictions.ClaimAllowedPeriod,
            restrictions.TimezoneId, DateTime.UtcNow, restrictions.StartDateTime, restrictions.ExpiryDateTime), DateTimeKind.Utc);
    }

    private async Task<RewardClaimUsageDomainModel> GetClaimIntervalUsages(string customerId, RewardDomainModel reward, ICollection<long> rewardIntervals)
    {
        var interval = CronService.GetNextInterval(reward.Terms.Restrictions.ClaimInterval);
        rewardIntervals.Add(Math.Min(interval, reward.Terms.Restrictions.ExpiryDateTime.ToUniversalTime().Ticks));

        var usage = await ReadClaimIntervalUsages(customerId, reward, interval);

        return usage;
    }

    private async Task<RewardClaimUsageDomainModel> ReadClaimIntervalUsages(string customerId, RewardDomainModel reward, long interval)
    {
        var activeClaims = await GetNumberOfActiveUsagesAsync(reward.RewardId, customerId, interval);

        _logger.LogDebug(
            "Reading claim interval from database. rewardRn={rewardRn}, custId={customerId}, interval={interval}, usages={@usages}",
            reward.RewardId, customerId, interval, activeClaims);

        activeClaims.IntervalId = interval;
        activeClaims.CurrentUsageId = activeClaims.LastUsageId;

        var remainingClaims = _claimStrategyFactory.GetRewardStrategy(reward.Type).CalculateRemainingClaims(reward, activeClaims);

        return remainingClaims;
    }

    private static List<RewardClaimDomainModel> GetFilteredResult(List<RewardClaimDomainModel> claims)
    {
        return claims
            .Where(r => r.Terms.Restrictions.StartDateTime <= DateTime.UtcNow &&
                        CronService.IsSatisfiedBy(r.Terms.Restrictions.ClaimAllowedPeriod, r.Terms.Restrictions.TimezoneId))
            .ToList();
    }

    private AsyncRetryPolicy GetRetryPolicy(string errorMsg, string customerId, object data)
    {
        return Policy
            .Handle<DbUpdateException>()
            .WaitAndRetryAsync(
                5,
                retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(5, retryAttempt)),
                (ex, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                    ex,
                    "Message={errorMsg}. RetryAttempt={retryCount}. CustomerId={customerId}. Data={@data}",
                    errorMsg,
                    retryCount,
                    customerId,
                    data);
                });
    }

    public async Task<BatchClaimResultDomainModel> ClaimEntitlementsAsync(BatchClaimDomainModel claimRequest)
    {
        var rewardClaims = new List<RewardClaimDomainModel>();
        var result = new BatchClaimResultDomainModel { ClaimResults = new(), AllRewardsFound = true };

        _logger.LogInformation("ClaimEntitlementsAsync:Request={@request}", claimRequest);

        try
        {
            foreach (var claim in claimRequest.Claims)
            {
                var rewardClaim = await GetRewardClaimAsync(claim, result);

                if (rewardClaim == null)
                {
                    continue;
                }

                rewardClaim.CustomerId = claimRequest.CustomerId;
                rewardClaim.CouponRn = claimRequest.CouponRn;
                rewardClaim.CurrencyCode = claimRequest.CurrencyCode;
                rewardClaim.BetRn = claim.Bet.Rn;
                rewardClaim.Bet = claim.Bet;

                var claimResult = await ValidateRequest(rewardClaim, claim);

                claimResult.Claim = rewardClaim;
                result.ClaimResults.Add(claimResult);

                // don't skip, add a list of errors and don't process
                if (!claimResult.Success)
                {
                    continue;
                }

                rewardClaims.Add(rewardClaim);
            }

            if (result.ClaimResults.Any(c => !c.Success))
            {
                return result;
            }

            await TryClaimAsync(claimRequest.CustomerId, rewardClaims, result);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Failed to claim entitlements {@request}", claimRequest);
            throw;
        }

        return result;
    }
    private async Task<ClaimResultDomainModel> ValidateRequest(RewardClaimDomainModel reward, BatchClaimItemDomainModel claim)
    {
        var claimResult = new ClaimResultDomainModel
        {
            Claim = reward
        };

        if (!reward.Hash.Equals(claim.Hash, StringComparison.InvariantCultureIgnoreCase))
        {
            claimResult.ErrorMessage =
                "Reward hash mismatch - reward has been modified since entitlements were retrieved";
            return claimResult;
        }

        if (reward.Terms.Restrictions.ExpiryDateTime.NotInTheFuture())
        {
            claimResult.ErrorMessage = "Reward is expired";
            return claimResult;
        }

        if (!CronService.IsSatisfiedBy(reward.Terms.Restrictions.ClaimAllowedPeriod, reward.Terms.Restrictions.TimezoneId))
        {
            claimResult.ErrorMessage = $"Reward claim interval do not satisfy allowed time period {reward.Terms.Restrictions.ClaimAllowedPeriod}";
            return claimResult;
        }


        var strategy = _claimStrategyFactory.GetRewardStrategy(reward.Type);
        var result = strategy.ValidateClaim(reward, claim);
        if (result is { Success: false })
        { // report failure immediately
            return result;
        }

        if (reward.Terms.Restrictions.AllowedContestRefs.IsNotNullAndNotEmpty() &&
            claim.Bet.Stages.Any(
                 s => !reward.Terms.Restrictions.AllowedContestRefs.Any(
                          r => r.Equals(s.ContestKey, StringComparison.InvariantCultureIgnoreCase))))
        {
            var events = reward.Terms.Restrictions.AllowedContestRefs.Select(e => e).ToCsv();

            claimResult.ErrorMessage = $"Following contest refs are allowed [{events}]";
            return claimResult;
        }

        if (reward.Terms.Restrictions.AllowedContestTypes.IsNotNullAndNotEmpty() &&
            claim.Bet.Stages.Any(s => !reward.Terms.Restrictions.AllowedContestTypes.Contains(
                s.ContestType, StringComparer.InvariantCultureIgnoreCase)))
        {
            var contestTypes = reward.Terms.Restrictions.AllowedContestTypes.Select(e => e).ToCsv();

            claimResult.ErrorMessage = $"Following contest types are allowed [{contestTypes}]";
            return claimResult;
        }

        if (reward.Terms.Restrictions.AllowedOutcomes.IsNotNullAndNotEmpty())
        {
            foreach (var selection in claim.Bet.Stages)
            {
                var outcome = new OutcomeRn(selection.RequestedOutcome);

                if (selection.RequestedOutcome.IsNullOrEmpty() ||
                    !reward.Terms.Restrictions.AllowedOutcomes.Any(x => x.Equals(outcome.VariantKey + ":" + outcome.OptionKey, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var outcomeRefs = reward.Terms.Restrictions.AllowedOutcomes.Select(c => c).ToCsv();
                    claimResult.ErrorMessage = $"Following outcomes are allowed [{outcomeRefs}]";
                }
            }
        }

        if (reward.Terms.Restrictions?.OddLimits?.MinimumStageOdds != null)
        {
            if (!claim.Bet.Stages.Any(x => x.RequestedPrice >= reward.Terms.Restrictions?.OddLimits?.MinimumStageOdds))
            {
                claimResult.ErrorMessage = $"No stages adhere to the configured minimum stage odds: {reward.Terms.Restrictions?.OddLimits?.MinimumStageOdds}";
                return claimResult;
            }
        }

        if (reward.Terms.Restrictions?.OddLimits?.MinimumCompoundOdds != null)
        {
            var compoundOdds = claim.Bet.Stages
                .Select(x => x.RequestedPrice)
                .Aggregate((a, b) => a * b);

            if (compoundOdds < reward.Terms.Restrictions?.OddLimits?.MinimumCompoundOdds)
            {
                claimResult.ErrorMessage = $"Cumulative odds fall below configured minimum: {reward.Terms.Restrictions?.OddLimits?.MinimumCompoundOdds}";
                return claimResult;
            }
        }

        if (reward.Terms.Restrictions?.AllowedContestStatuses != null)
        {
            var contestDetails = await _marketMirrorClient.GetContests(claim.Bet.Stages.Select(x => x.ContestKey).Distinct());
            if (!AllContestKeysRetrieved(contestDetails, claim.Bet.Stages))
            {
                claimResult.ErrorMessage = $"Unable to validate adherence to the ContestStatus restriction {reward.Terms.Restrictions?.AllowedContestStatuses}";
                return claimResult;
            }

            if (contestDetails.Contests.All(x => x.ContestStatus.ToString() != reward.Terms.Restrictions?.AllowedContestStatuses.ToString()))
            {
                claimResult.ErrorMessage = $"All stages do not adhere to the ContestStatus restriction {reward.Terms.Restrictions?.AllowedContestStatuses}";
                return claimResult;
            }
        }

        return claimResult;
    }
    private bool AllContestKeysRetrieved(GetContestsResponse response, ICollection<CompoundStageDomainModel> stages)
    {
        if (response?.Contests == null || !response.Contests.Any())
        {
            _logger.LogInformation("Failing Batch Claim because no Contests were retrieved for the keys: {keys}", stages.Select(x => x.ContestKey).ToCsv());
            return false;
        }

        var set = new HashSet<string>(response.Contests.Select(x => x.ContestKey));
        return set.SetEquals(stages.Select(x => x.ContestKey));
    }

    private async Task TryClaimAsync(string customerId, List<RewardClaimDomainModel> claims, BatchClaimResultDomainModel result)
    {
        await GetRetryPolicy("Could not claim rewards", customerId, claims).ExecuteAsync(
            async () =>
            {
                var usages = new Dictionary<string, RewardClaimUsageDomainModel>();
                foreach (var claim in claims)
                {
                    if (!usages.TryGetValue(claim.RewardId, out var usage))
                    {
                        var reward = _mapper.Map<RewardDomainModel>(claim);
                        var interval = CronService.GetNextInterval(reward.Terms.Restrictions.ClaimInterval);
                        usage = await ReadClaimIntervalUsages(claim.CustomerId, reward, interval);
                        usages[claim.RewardId] = usage;
                    }

                    if (usage == null || usage.ClaimRemaining <= 0)
                    {
                        var res = result.ClaimResults.Find(c => c.Claim.InstanceId == claim.InstanceId);
                        res.ErrorMessage = "Claim limit exceeded";
                        continue;
                    }

                    var claimResult = await _claimStrategyFactory
                        .GetRewardStrategy(claim.Type)
                        .CalculateClaimResultAsync(new()
                        {
                            CombinationStages = claim.Bet.Stages,
                            Formula = claim.Bet.Formula,
                            CombinationStake = claim.Bet.RequestedStake / claim.Bet.Stages.Count,
                            Claim = claim
                        });

                    claim.IntervalId = usage.IntervalId;
                    claim.UsageId = ++usage.CurrentUsageId;
                    claim.ClaimLimit = --usage.ClaimRemaining;
                    claim.Status = RewardClaimStatus.Claimed;
                    claim.PayoffMetadata = claimResult.PayoffMetadata;
                }

                if (result.AllRewardsFound && result.ClaimResults.All(c => c.Success))
                {
                    await AddRangeAsync(claims);
                    _logger.LogInformation("Claim successful {@claims}", claims);
                }
            });
    }

    private async Task AddRangeAsync(List<RewardClaimDomainModel> claims)
    {
        if (claims == null)
        {
            throw new ArgumentNullException("Claims cannot be null");
        }

        List<RewardClaimDomainModel> savedClaims = new();

        try
        {
            foreach (var claim in claims)
            {
                var data = _mapper.Map<RewardClaim>(claim);
                _context.RewardClaims.Add(data);

                OnDataCreated(claim);
                savedClaims.Add(_mapper.Map<RewardClaimDomainModel>(data));
            }

            await _context.SaveChangesAsync();

        }
        catch (DbUpdateConcurrencyException)
        {
            throw new RewardClaimedConcurrencyException();
        }
    }

    private void OnDataCreated(RewardClaimDomainModel domain)
    {
        AuditReward audit = new()
        {
            RewardId = domain.RewardId,
            Activity = nameof(AuditActivity.ClaimCreated),
            CreatedBy = domain.CreatedBy
        };

        _context.AuditRewards.Add(audit);
    }

    private async Task<RewardClaimDomainModel> GetRewardClaimAsync(BatchClaimItemDomainModel claimRequest, BatchClaimResultDomainModel result)
    {
        RewardClaimDomainModel reward = null;

        var rewardDomainModel = await GetAsync(claimRequest.RewardId);

        if (IsRewardFound(claimRequest.RewardId, rewardDomainModel, result))
        {
            reward = _mapper.Map<RewardClaimDomainModel>(rewardDomainModel);
        }

        return reward;
    }
    private async Task<RewardDomainModel> GetAsync(string id)
    {
        var data = await _context.Rewards
            .Include(b => b.RewardTags).ThenInclude(bt => bt.Tag)
            .Include(p => p.RewardTemplateReward).ThenInclude(pt => pt.RewardTemplate)
            .AsNoTracking()
            .FirstOrDefaultAsync(reward => reward.Id == id);

        // Make sure there are no circular dependencies otherwise auto-mapper does recursive mapping
        if (data is { RewardTemplateReward: { } })
        {
            foreach (var promotionTemplate in data.RewardTemplateReward.Select(pt => pt.RewardTemplate))
            {
                promotionTemplate.RewardTemplateReward.ForEach(ptp => ptp.Reward = null);
            }
        }

        var domain = _mapper.Map<RewardDomainModel>(data);

        return domain;
    }


    private static bool IsRewardFound(string id, RewardDomainModel reward, BatchClaimResultDomainModel result)
    {
        if (reward != null)
        {
            return true;
        }

        result.AllRewardsFound = false;
        result.ClaimResults.Add(new()
        {
            ErrorMessage = "Reward not found",
            Claim = new() { RewardId = id }
        });
        return false;
    }


    private async Task<IEnumerable<RewardDomainModel>> FindActiveRewardsAsync(
        string customerId,
        DateTimeOffset currentDateTime)
    {
        var utc = currentDateTime.UtcDateTime;
        var data = await _context.Rewards
            .AsNoTracking()
            .Where(reward =>
                reward.CustomerId == customerId &&
                reward.ExpiryDateTime >= utc &&
                !reward.IsCancelled)
            .ToListAsync();

        var domain = _mapper.Map<IEnumerable<RewardDomainModel>>(data);

        return domain;
    }

    private async Task<IEnumerable<RewardDomainModel>> FindActiveRewardsAsync(
        IEnumerable<string> rewardTemplateKeys,
        DateTimeOffset currentDateTime)
    {
        var rewards = await _context.Rewards
            .AsNoTracking()
            .Where(p => p.ExpiryDateTime >= currentDateTime.UtcDateTime &&
                        !p.IsCancelled &&
                        p.RewardTemplateReward.Select(ptp => ptp.RewardTemplate)
                            .Any(t => rewardTemplateKeys.Contains(t.Key) && t.Enabled))
            .ToListAsync();

        var domain = _mapper.Map<IEnumerable<RewardDomainModel>>(rewards);

        return domain;
    }

    private async Task<IEnumerable<RewardDomainModel>> FindActiveCustomerRewardsAsync(
        string customerId,
        DateTimeOffset currentDateTime)
    {
        var utc = currentDateTime.UtcDateTime;
        var data = await _context.CustomerRewards
            .Include(cr => cr.Reward)
            .AsNoTracking()
            .Where(cr =>
                cr.CustomerId == customerId &&
                cr.Reward.ExpiryDateTime >= utc &&
                !cr.Reward.IsCancelled)
            .Select(cr => cr.Reward)
            .ToListAsync();

        var domain = _mapper.Map<IEnumerable<RewardDomainModel>>(data);

        return domain;
    }

    private async Task UpdateCustomerPromotionTemplatesAsync(
        IEnumerable<string> templateKeys,
        string customerId)
    {
        var repository = _context.RewardTemplateCustomers;

        repository.Remove(p => p.CustomerId == customerId);

        foreach (var templateKey in templateKeys)
        {
            RewardTemplateCustomer data = new()
            {
                PromotionTemplateKey = templateKey,
                CustomerId = customerId
            };

            repository.Add(data);
        }
    }

    public async Task<RewardClaimUsageDomainModel> GetNumberOfActiveUsagesAsync(
        string rewardRn,
        string customerId,
        long intervalId)
    {
        var repoClaim = _context.RewardClaims;

        var usages = await repoClaim
            .AsNoTracking()
            .Where(claim => claim.RewardId == rewardRn &&
                            claim.CustomerId == customerId &&
                            claim.IntervalId == intervalId)
            .ToListAsync();

        return new()
        {
            ActiveUsagesCount = usages.Count(u => u.Status != nameof(RewardClaimStatus.Revoked)),
            LastUsageId = usages.Any() ? usages.Max(u => u.UsageId) : 0,
            IntervalId = intervalId,
            BetOutcomeStatuses = usages.Where(u => u.Status != nameof(RewardClaimStatus.Revoked))
                           .Select(u => u.BetOutcomeStatus.ToEnumSafe<BetOutcome>())
                           .ToList()
        };
    }
    public async Task<PagedResponse<RewardClaimDomainModel>> GetByFilter(RewardClaimFilterDomainModel filter,
        PagedRequest pagination)
    {
        var predicate = PredicateBuilder.New<RewardClaim>(true);

        if (!string.IsNullOrWhiteSpace(filter.InstanceId))
        {
            predicate = predicate.And(r => r.InstanceId == filter.InstanceId);
        }

        if (!string.IsNullOrWhiteSpace(filter.CustomerId))
        {
            predicate = predicate.And(r => r.CustomerId == filter.CustomerId);
        }

        if (!string.IsNullOrWhiteSpace(filter.RewardRn))
        {
            predicate = predicate.And(r => r.RewardId == filter.RewardRn);
        }

        if (!string.IsNullOrWhiteSpace(filter.RewardId))
        {
            predicate = predicate.And(r => r.RewardId == filter.RewardId);
        }

        if (!string.IsNullOrWhiteSpace(filter.RewardName))
        {
            predicate = predicate.And(r => r.RewardName == filter.RewardName);
        }

        if (!string.IsNullOrWhiteSpace(filter.CouponRn))
        {
            predicate = predicate.And(r => r.CouponRn == filter.CouponRn);
        }

        if (!string.IsNullOrWhiteSpace(filter.BetRn))
        {
            predicate = predicate.And(r => r.BetRn == filter.BetRn);
        }

        if (!string.IsNullOrWhiteSpace(filter.RewardType))
        {
            predicate = predicate.And(r => r.RewardType == filter.RewardType);
        }

        if (!string.IsNullOrWhiteSpace(filter.ClaimStatus))
        {
            predicate = predicate.And(r => r.Status == filter.ClaimStatus);
        }

        if (!string.IsNullOrWhiteSpace(filter.BetOutcomeStatus))
        {
            predicate = predicate.And(r => r.BetOutcomeStatus == filter.BetOutcomeStatus);
        }

        if (filter.UpdatedDateFromUtc.HasValue)
        {
            predicate = predicate.And(r => r.UpdatedOn >= filter.UpdatedDateFromUtc.Value);
        }

        if (filter.UpdatedDateToUtc.HasValue)
        {
            predicate = predicate.And(r => r.UpdatedOn <= filter.UpdatedDateToUtc.Value);
        }

        var query = await _context.RewardClaims
            .Where(predicate)
            .ToListAsync();

        var totalRecords = query.Count;

        var data = query
            .AsQueryable()
            .SkipWithPaging(pagination, filter).Queryable
            .AsNoTracking()
            .ToList();

        var domain = _mapper.Map<List<RewardClaimDomainModel>>(data);

        return new()
        {
            ItemCount = totalRecords,
            Items = domain,
            Limit = pagination.Limit,
            Offset = pagination.Offset
        };
    }

    private async Task UpdateRewardClaimsAsync(string customerId, IEnumerable<RewardDomainModel> rewards, List<long> rewardIntervals, List<RewardClaimDomainModel> rewardClaims, bool mapCustomerId = false)
    {
        foreach (var reward in rewards)
        {
            var rewardClaim = _mapper.Map<RewardClaimDomainModel>(reward);

            if (mapCustomerId)
            {
                rewardClaim.CustomerId = customerId;
            }

            var claimUsage = await GetClaimIntervalUsages(customerId, reward, rewardIntervals);

            if (claimUsage == null)
            {
                continue;
            }

            _mapper.Map(claimUsage, rewardClaim);

            rewardClaim.NextInterval = GetNextInterval(rewardClaim.Terms.Restrictions);
            rewardClaims.Add(rewardClaim);
        }
    }

}
