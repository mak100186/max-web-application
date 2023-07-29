using AutoMapper;

using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.MessageConsumers.Services;

public interface ICustomerRewardService
{
    Task<RewardDomainModel> GetRewardAsync(string id);
    Task AddCustomerRewardAsync(string customerId, string rewardId);
}
public class CustomerRewardService : ICustomerRewardService
{
    private readonly IDbContextFactory<RewardsDbContext> _contextFactory;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerRewardService> _logger;

    public CustomerRewardService(
        IMapper mapper,
        ILogger<CustomerRewardService> logger,
        IDbContextFactory<RewardsDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<RewardDomainModel> GetRewardAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var data = await context.Rewards
            .AsNoTracking()
            .FirstOrDefaultAsync(reward => reward.Id == id);

        var reward = _mapper.Map<RewardDomainModel>(data);

        return reward;
    }

    public async Task AddCustomerRewardAsync(string customerId, string rewardId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var customerReward = new CustomerReward
        {
            CustomerId = customerId,
            RewardId = rewardId,
            CreatedOn = DateTime.UtcNow
        };


        context.CustomerRewards.Add(customerReward);

        await context.SaveChangesAsync();

        _logger.LogInformation("CustomerId [{customerId}] linked to RewardId [{rewardId}]", customerId, rewardId);
    }
}
