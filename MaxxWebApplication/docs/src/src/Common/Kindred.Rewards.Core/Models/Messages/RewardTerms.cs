namespace Kindred.Rewards.Core.Models.Messages;

public class RewardTerms
{
    public RewardSettlement SettlementTerms { get; set; }

    public Dictionary<string, string> RewardParameters { get; set; }
}
