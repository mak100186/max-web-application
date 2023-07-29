using Kindred.Rewards.Core.WebApi.Enums;

namespace Kindred.Rewards.Core.WebApi.Payloads;

public class ProfitBoostParametersApiModel : RewardParameterApiModelBase
{
    public decimal? MaxStakeAmount { get; set; }
    public Dictionary<int, decimal> LegTable { get; set; }
    public override string Type => nameof(RewardType.Profitboost);
}
