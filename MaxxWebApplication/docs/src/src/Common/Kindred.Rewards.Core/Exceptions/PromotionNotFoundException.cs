namespace Kindred.Rewards.Core.Exceptions;

public class PromotionNotFoundException : Exception
{
    public PromotionNotFoundException(string rewardRn)
        : base($"Could not find a promotion with reward key {rewardRn}")
    {
    }
}
