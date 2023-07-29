namespace Kindred.Rewards.Core.Models.Messages;

public abstract class BaseResult
{
    //key is position, value are the competitor keys for that position. i.e. { Position1, [Competitor1, Competitor2] }
    public Dictionary<string, string[]> Results;
}
