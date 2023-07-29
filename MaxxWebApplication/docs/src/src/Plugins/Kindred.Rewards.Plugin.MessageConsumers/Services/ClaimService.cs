using AutoMapper;

using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Infrastructure.Kafka;
using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Models.RewardClaims;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Polly;
using Polly.Retry;

namespace Kindred.Rewards.Plugin.MessageConsumers.Services;

public interface IClaimService
{
    Task ReActivateClaimAsync(IEnumerable<string> instanceIds, string correlationId);

}

public class ClaimService : IClaimService
{
    private readonly IDbContextFactory<RewardsDbContext> _contextFactory;

    private readonly IMapper _mapper;
    private readonly ILogger<ClaimService> _logger;
    private readonly IKafkaProducerManager _producerOrchestrator;

    private readonly AsyncRetryPolicy _retryPolicy;

    public ClaimService(
        IDbContextFactory<RewardsDbContext> contextFactory,
        IMapper mapper,
        ILogger<ClaimService> logger,
        IKafkaProducerManager producerOrchestrator
    )
    {
        _contextFactory = contextFactory;
        _mapper = mapper;
        _logger = logger;
        _producerOrchestrator = producerOrchestrator;
        // todo migrate this retry policy to config
        // the magic numbers are from legacy code
        // hiding them in consts doesn't stop them from being magic numbers
        // I've brought them here because the policy is a single complex const
        _retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(30, x => TimeSpan.FromMilliseconds(500));

    }

    private void AddAuditRecord(string rewardRn, AuditActivity status, string createdBy, DbSet<AuditReward> context)
    {
        var audit = new AuditReward
        {
            RewardId = rewardRn,
            Activity = status.ToString(),
            CreatedBy = createdBy
        };

        context.Add(audit);
    }

    public async Task<List<RewardClaim>> GetClaimedOrSettledAsync(RewardsDbContext context, IEnumerable<string> instanceIds)
    {
        return await context.RewardClaims
                   .Where(claim => instanceIds.Contains(claim.InstanceId) &&
                                   (claim.Status == nameof(RewardClaimStatus.Claimed) || claim.Status == nameof(RewardClaimStatus.Settled)))
                   .GroupBy(claim => claim.InstanceId)
                   .Select(claim => claim.OrderByDescending(c => c.CreatedOn).First())
                   .ToListAsync();

    }

    public async Task ReActivateClaimAsync(IEnumerable<string> instanceIds, string correlationId)
    {
        instanceIds = instanceIds.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();

        try
        {
            var itemsNotProcessed = instanceIds.ToHashSet();

            await using var context = await _contextFactory.CreateDbContextAsync();
            var claims = await GetClaimedOrSettledAsync(context, instanceIds);

            foreach (var claim in claims)
            {
                claim.Status = RewardClaimStatus.Revoked.ToString();
                claim.RewardPayoutAmount = null;
                claim.BetOutcomeStatus = null;
                itemsNotProcessed.Remove(claim.InstanceId);

                AddAuditRecord(claim.RewardId, AuditActivity.ClaimRevoked, claim.CreatedBy, context.AuditRewards);
            }

            await context.SaveChangesAsync();

            if (itemsNotProcessed.Any())
            {
                _logger.LogWarning($"{nameof(ReActivateClaimAsync)}:Could not find reward claims with instance Ids {itemsNotProcessed.ToCsv()}");
            }

            if (claims.Any())
            {
                var claimDomainModel = _mapper.Map<IEnumerable<RewardClaimDomainModel>>(claims);
                var messages = _mapper.Map<IEnumerable<Core.Models.Messages.RewardClaimUnsettled>>(claimDomainModel);

                foreach (var message in messages)
                {
                    await _retryPolicy
                    .ExecuteAsync(async () =>
                    {
                        await Publish(message, correlationId);

                        _logger.LogInformation($"Published {nameof(Core.Models.Messages.RewardClaimUnsettled)} for Re-Activate {{@evt}}", message);
                    });
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Reward instanceIds {instanceIds}", instanceIds);
            throw;
        }
    }

    public async Task Publish(Core.Models.Messages.RewardClaimUnsettled claimEvent, string correlationId)
    {
        try
        {
            await _producerOrchestrator.SendAsync(DomainConstants.TopicProfileClaimUpdates, claimEvent, claimEvent.InstanceId, correlationId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ERROR PUBLISHING {@rewardEvent} ", claimEvent);
            throw;
        }
    }

}
