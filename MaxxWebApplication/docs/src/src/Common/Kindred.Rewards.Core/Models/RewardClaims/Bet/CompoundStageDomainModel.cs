using Kindred.Rewards.Core.Extensions;

namespace Kindred.Rewards.Core.Models.RewardClaims.Bet;

public class CompoundStageDomainModel
{
    private string _market;

    public string Market
    {
        get { return _market; }
        set
        {
            _market = value;
            ContestKey = value.GetContestKeyFromMarket();
            ContestType = value.GetContestTypeFromMarket();
        }
    }

    public string ContestKey { get; set; }

    public string ContestType { get; set; }

    public decimal RequestedPrice { get; set; }

    public string RequestedOutcome { get; set; }
}
