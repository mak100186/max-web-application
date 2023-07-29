using Kindred.Rewards.Core.WebApi.Enums;

namespace Kindred.Rewards.Core.WebApi.Payloads;

public class UniBoostReloadParametersApiModel : UniBoostParametersApiModel
{
    public RewardReloadApiModel Reload { get; set; }
    public override string Type => nameof(RewardType.UniboostReload);
}
