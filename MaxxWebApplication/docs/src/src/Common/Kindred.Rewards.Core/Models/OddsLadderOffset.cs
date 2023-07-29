// legacy namespace Kindred.Rewards.Rewards.Logic.Models;
namespace Kindred.Rewards.Core.Models;


public class OddsLadderOffset
{
    public int Offset { get; set; }
    public int Limit { get; set; }
    public int ItemCount { get; set; }
    public List<OddsLadder> Items { get; set; }
}
