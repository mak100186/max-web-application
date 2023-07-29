using Kindred.Rewards.Core.WebApi.Payloads.BetModel;

namespace Kindred.Rewards.Core.WebApi.Payloads;

public class ClaimApiModel
{
    // Reward key
    public string Rn { get; set; }
    public string Hash { get; set; }
    public BetApiModel Bet { get; set; }
}
