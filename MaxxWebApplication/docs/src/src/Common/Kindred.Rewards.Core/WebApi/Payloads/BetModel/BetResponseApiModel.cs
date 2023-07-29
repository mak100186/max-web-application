namespace Kindred.Rewards.Core.WebApi.Payloads.BetModel;

public class BetResponseApiModel
{
    public string Rn { get; set; }
    public decimal RequestedStake { get; set; }
    public decimal? AcceptedStake { get; set; }
    public string Formula { get; set; }
    public IEnumerable<CompoundStageResponseApiModel> Stages { get; set; }
    public IEnumerable<CombinationApiModel> AcceptedCombinations { get; set; }
    public IEnumerable<CombinationApiModel> RequestedCombinations { get; set; }
    public string Status { get; set; }
}
