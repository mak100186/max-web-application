namespace Kindred.Rewards.Core.ExceptionHandling.Exceptions;

public class RewardNotFoundException : Exception
{
    public RewardNotFoundException()
        : base("Could not find a reward with the provided reward key")
    {
    }

    public RewardNotFoundException(string rewardRn)
        : base($"Could not find a reward with the provided reward key {rewardRn}")
    {
    }
}
