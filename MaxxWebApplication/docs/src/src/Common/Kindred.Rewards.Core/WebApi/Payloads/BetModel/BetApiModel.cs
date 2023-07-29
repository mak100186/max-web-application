namespace Kindred.Rewards.Core.WebApi.Payloads.BetModel;

public class BetApiModel
{
    public string Rn { get; set; }
    public decimal RequestedStake { get; set; }
    public decimal? AcceptedStake { get; set; }
    public string Formula { get; set; }
    public IEnumerable<CompoundStageApiModel> Stages { get; set; }
    public IEnumerable<CombinationApiModel> AcceptedCombinations { get; set; }
    public IEnumerable<CombinationApiModel> RequestedCombinations { get; set; }
    public string Status { get; set; }
}
