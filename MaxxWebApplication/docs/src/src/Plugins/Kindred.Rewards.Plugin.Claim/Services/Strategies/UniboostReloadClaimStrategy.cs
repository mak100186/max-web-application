using AutoMapper;

using Kindred.Rewards.Core.Client;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Infrastructure.Data;
using Kindred.Rewards.Plugin.Claim.Clients.MarketMirror;

using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Plugin.Claim.Services.Strategies;

public class UniBoostReloadClaimStrategy : UniBoostClaimStrategy
{
    public UniBoostReloadClaimStrategy(RewardsDbContext context, IMapper mapper, ILogger<UniBoostReloadClaimStrategy> logger, IOddsLadderClient oddsLadderLogic, IMarketMirrorClient client)
        : base(context, mapper, logger, oddsLadderLogic, client)
    {
        Features[FeatureType.Reload].Enabled = true;
        RewardName = nameof(RewardType.UniboostReload);
    }
}
