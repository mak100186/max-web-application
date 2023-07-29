using Kindred.Rewards.Core.Abstractions;
using Kindred.Rewards.Core.Models.Rn;

namespace Kindred.Rewards.Core.Helpers;

public class SemanticResourceNameParser
{
    /// <summary>
    /// Parses the base elements of the literal
    /// </summary>
    /// <param name="rnLiteral"></param>
    /// <returns>parsed BaseRn</returns>
    public static IResourceName Parse(string rnLiteral)
    {
        return new BaseRn(rnLiteral);
    }

    /// <summary>
    /// Converts the literal to its relevant type. See code example on how to use. 
    /// </summary>
    /// <example>
    /// <code>
    /// public string GetRewardGuid(string literal)
    /// {
    ///    var rn = Convert(literal);
    ///    if (rn is RewardRn rewardRn)
    ///    {
    ///        return rewardRn.GuidValue;
    ///    }
    ///
    ///    return null;
    /// }
    /// </code>
    /// </example>
    /// <param name="rnLiteral"></param>
    /// <returns>the actual object of the type it belongs to</returns>
    public static IResourceName Convert(string rnLiteral)
    {
        IResourceName rn = new BaseRn(rnLiteral);

        return rn.EntityType switch
        {
            DomainConstants.Rn.EntityTypes.Reward => new RewardRn(rnLiteral),
            DomainConstants.Rn.EntityTypes.Variant => new VariantRn(rnLiteral),
            DomainConstants.Rn.EntityTypes.Market => new MarketRn(rnLiteral),
            DomainConstants.Rn.EntityTypes.Bet => new BetRn(rnLiteral),
            DomainConstants.Rn.EntityTypes.Outcome => new OutcomeRn(rnLiteral),
            DomainConstants.Rn.EntityTypes.Claim => new ClaimRn(rnLiteral),
            DomainConstants.Rn.EntityTypes.Combination => new CombinationRn(rnLiteral),
            DomainConstants.Rn.EntityTypes.Coupon => new CouponRn(rnLiteral),
            _ => null,
        };
    }
}
