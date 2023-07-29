namespace Kindred.Rewards.Core.WebApi.Payloads;

public class RewardReloadApiModel
{
    public int? MaxReload { get; set; }

    public int StopOnMinimumWinBets { get; set; }
}
