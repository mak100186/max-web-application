using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Plugin.Claim.Services.Strategies.Base;

using Microsoft.Extensions.DependencyInjection;

namespace Kindred.Rewards.Plugin.Claim.Services.Strategies;

public interface IRewardClaimStrategyFactory
{
    IRewardStrategy GetRewardStrategy(RewardType rewardType);
}

public class RewardClaimStrategyFactory : IRewardClaimStrategyFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RewardClaimStrategyFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public IRewardStrategy GetRewardStrategy(RewardType rewardType)
    {
        var scope = _serviceScopeFactory.CreateScope();

        return rewardType switch
        {
            RewardType.Freebet => scope.ServiceProvider.GetService<FreeBetClaimStrategy>(),
            RewardType.Uniboost => scope.ServiceProvider.GetService<UniBoostClaimStrategy>(),
            RewardType.UniboostReload => scope.ServiceProvider.GetService<UniBoostReloadClaimStrategy>(),
            RewardType.Profitboost => scope.ServiceProvider.GetService<ProfitBoostClaimStrategy>(),
            _ => null
        };
    }
}
