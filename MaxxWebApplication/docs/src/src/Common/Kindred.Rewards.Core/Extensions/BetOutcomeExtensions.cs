using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Extensions;

public static class BetOutcomeExtensions
{
    public static bool IsWinningBet(this BetOutcome? value)
    {
        return value is BetOutcome.Winning or BetOutcome.WinningAndPartialRefund;
    }

    public static bool IsLosingBet(this BetOutcome? value)
    {
        return value is BetOutcome.Losing;
    }
}
