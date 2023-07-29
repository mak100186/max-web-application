namespace Kindred.Rewards.Core.WebApi.Payloads.BetModel;

public class CompoundStageResponseApiModel
{
    public string Market { get; set; }
    public OddsResponseApiModel Odds { get; set; }
    public SelectionApiModel AcceptedSelection { get; set; }
    public SelectionApiModel RequestedSelection { get; set; }
}
