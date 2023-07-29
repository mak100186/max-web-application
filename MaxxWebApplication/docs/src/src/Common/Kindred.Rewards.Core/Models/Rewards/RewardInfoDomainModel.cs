namespace Kindred.Rewards.Core.Models.Rewards;

public class RewardInfoDomainModel
{
    public IReadOnlyCollection<string> OptionalParameterKeys { get; set; }

    public IReadOnlyCollection<string> RequiredParameterKeys { get; set; }
}
