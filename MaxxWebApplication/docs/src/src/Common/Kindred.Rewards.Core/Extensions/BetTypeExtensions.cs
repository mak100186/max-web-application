using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Extensions;

public static class BetTypeExtensions
{
    public static bool ContainsMultiBetTypes(this IReadOnlyCollection<BetTypes> betTypes)
    {
        return betTypes.Contains(BetTypes.StandardMultiLeg) || betTypes.Contains(BetTypes.SystemMultiLeg);
    }
}
