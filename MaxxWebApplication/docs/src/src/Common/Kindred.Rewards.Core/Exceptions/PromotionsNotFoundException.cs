using Kindred.Infrastructure.Core.Extensions.Extensions;

namespace Kindred.Rewards.Core.Exceptions;

public class PromotionsNotFoundException : Exception
{
    public PromotionsNotFoundException(List<string> rewardsKeys)
        : base($"Could not find promotions with reward keys {rewardsKeys.ToCsv()}")
    {

    }
}
