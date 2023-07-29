using Kindred.Rewards.Core.WebApi.Enums;

namespace Kindred.Rewards.Core.WebApi.Payloads;

public class UniBoostParametersApiModel : RewardParameterApiModelBase
{
    public decimal? MaxStakeAmount { get; set; }
    public int OddsLadderOffset { get; set; }
    public override string Type => nameof(RewardType.Uniboost);
}
