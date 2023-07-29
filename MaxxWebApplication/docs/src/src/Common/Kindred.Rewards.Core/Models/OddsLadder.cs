//legacy namespace Kindred.Rewards.Rewards.Logic.Models;
namespace Kindred.Rewards.Core.Models;

public class OddsLadder
{
    public string ContestType { get; set; }

    public List<Odds> PreGameOddsLadder { get; set; }

    public List<Odds> InPlayOddsLadder { get; set; }
}
