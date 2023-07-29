namespace Kindred.Rewards.Core.WebApi.Payloads;

public class RewardInfoApiModel
{
    public string Rn { get; set; }
    public string RewardId { get; set; }
    public string Type { get; set; }
    public string Category { get; set; }
    public string CustomerId { get; set; }
    public Dictionary<string, string> Attributes { get; set; }
    public RewardParameterApiModelBase RewardParameters { get; set; }
}
