namespace Kindred.Rewards.Core.WebApi.Payloads.BetModel;

public class CompoundStageApiModel
{
    public string Market { get; set; }
    public OddsApiModel Odds { get; set; }
    public SelectionApiModel AcceptedSelection { get; set; }
    public SelectionApiModel RequestedSelection { get; set; }
}
