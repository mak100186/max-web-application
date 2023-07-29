namespace Kindred.Rewards.Core.Models.Messages.Reward;

public class MultiConfig
{
    public int? MinStages { get; set; }
    public int? MaxStages { get; set; }
    public int? MinCombinations { get; set; }
    public int? MaxCombinations { get; set; }
    public IEnumerable<string> FilterFormulae { get; set; }
}
