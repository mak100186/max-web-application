using Kindred.Rewards.Rewards.FunctionalTests.Steps.Context.Models;

namespace Kindred.Rewards.Rewards.FunctionalTests.Steps.Context;
public class BetContext
{
    public BetContext()
    {
        ReferenceBets = new();
    }

    public List<Bet> ReferenceBets { get; set; }
}
