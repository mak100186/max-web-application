namespace Kindred.Rewards.Core.WebApi.Payloads;
public abstract class RewardParameterApiModelBase
{
    public decimal? MaxExtraWinnings { get; set; }
    public abstract string Type { get; }
}
