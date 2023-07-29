namespace Kindred.Rewards.Core.WebApi.Payloads;

public class MultiConfigApiModel
{
    public int? MinStages { get; set; }
    public int? MaxStages { get; set; }
    public int? MinCombinations { get; set; }
    public int? MaxCombinations { get; set; }
    public IEnumerable<string> FilterFormulae { get; set; }

    public override int GetHashCode()
    {
        return HashCode.Combine(MinStages, MaxStages, MinCombinations, MaxCombinations);
    }
}
