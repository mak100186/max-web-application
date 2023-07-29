using Kindred.Rewards.Core.WebApi.Enums;

namespace Kindred.Rewards.Core.WebApi.Payloads;

public class FreeBetParametersApiModel : RewardParameterApiModelBase
{
    public decimal Amount { get; set; }
    public override string Type => nameof(RewardType.Freebet);
}
