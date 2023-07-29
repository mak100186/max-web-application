namespace Kindred.Rewards.Core.WebApi.Payloads.BetModel;

public class CombinationApiModel
{
    public string Rn { get; set; }
    public IEnumerable<SelectionApiModel> Selections { get; set; }
}
