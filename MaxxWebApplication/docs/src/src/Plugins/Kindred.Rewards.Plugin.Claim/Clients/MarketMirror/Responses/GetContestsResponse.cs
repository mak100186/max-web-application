namespace Kindred.Rewards.Plugin.Claim.Clients.MarketMirror.Responses;

public class GetContestsResponse
{
    public IEnumerable<ContestDetails> Contests { get; set; }
}

public class ContestDetails
{
    public string ContestKey { get; set; }
    public ContestStatus ContestStatus { get; set; }
}

public enum ContestStatus
{
    PreGame,
    InPlay,
    Concluded,
    Cancelled
}
