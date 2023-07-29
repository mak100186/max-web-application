using System.ComponentModel.DataAnnotations;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models.RewardClaims.Bet;

namespace Kindred.Rewards.Core.Extensions;

public static class BetDomainModelExtensions
{
    public static BetTypes DeduceBetType(this BetDomainModel bet)
    {
        var legCount = bet.Stages.Count;

        return DeduceBetType(legCount, bet.Formula);
    }

    public static BetTypes DeduceBetType(int legCount, string formula)
    {
        return legCount switch
        {
            < 1 => throw new ValidationException("Bet has no stages"),
            1 => BetTypes.SingleLeg,
            _ => formula.IsStandardMulti() ? BetTypes.StandardMultiLeg : BetTypes.SystemMultiLeg
        };
    }
}
