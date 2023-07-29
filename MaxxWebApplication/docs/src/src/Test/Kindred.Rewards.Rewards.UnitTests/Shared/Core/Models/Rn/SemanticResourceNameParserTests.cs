using System.Collections;

using FluentAssertions;

using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Helpers;
using Kindred.Rewards.Core.Models.Rn;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Models.Rn;

[TestFixture]
public class SemanticResourceNameParserTests
{
    public static IEnumerable RnDataFactory
    {
        get
        {
            yield return new TestCaseData(
                    "ksp:reward.1:123e4567-e89b-12d3-a456-426614174000",
                    typeof(RewardRn),
                    DomainConstants.Rn.EntityTypes.Reward)
                    .SetName("Rn convertion to RewardRn");

            yield return new TestCaseData(
                    "ksp:bet.1:123e4567-e89b-12d3-a456-426614174000:1",
                    typeof(BetRn),
                    DomainConstants.Rn.EntityTypes.Bet)
                    .SetName("Rn convertion to BetRn");

            yield return new TestCaseData(
                    "ksp:claim.1:123e4567-e89b-12d3-a456-426614174000",
                    typeof(ClaimRn),
                    DomainConstants.Rn.EntityTypes.Claim)
                    .SetName("Rn convertion to ClaimRn");

            yield return new TestCaseData(
                    "ksp:combination.1:123e4567-e89b-12d3-a456-426614174000:1:2",
                    typeof(CombinationRn),
                    DomainConstants.Rn.EntityTypes.Combination)
                    .SetName("Rn convertion to CombinationRn");

            yield return new TestCaseData(
                    "ksp:coupon.1:123e4567-e89b-12d3-a456-426614174000",
                    typeof(CouponRn),
                    DomainConstants.Rn.EntityTypes.Coupon)
                    .SetName("Rn convertion to CouponRn");

            yield return new TestCaseData(
                    "ksp:market.1:[football:201711202200:watford_vs_west_ham]:1x2:bg",
                    typeof(MarketRn),
                    DomainConstants.Rn.EntityTypes.Market)
                    .SetName("Rn convertion to MarketRn");

            yield return new TestCaseData(
                    "ksp:outcome.1:[football:201711202200:watford_vs_west_ham]:1x2:plain:west_ham_united_fc",
                    typeof(OutcomeRn),
                    DomainConstants.Rn.EntityTypes.Outcome)
                    .SetName("Rn convertion to OutcomeRn");


            yield return new TestCaseData(
                    "ksp:variant.1:[football:201711202200:watford_vs_west_ham]:1x2:plain",
                    typeof(VariantRn),
                    DomainConstants.Rn.EntityTypes.Variant)
                    .SetName("Rn convertion to VariantRn");


        }
    }

    [Test]
    [TestCaseSource(typeof(SemanticResourceNameParserTests), nameof(RnDataFactory))]
    public void StagesAboveMinimumOddRestriction_ShouldReturn_AllStagesAboveMinimumStageOdds(string literal, Type type, string typeName)
    {
        // Act
        var result = SemanticResourceNameParser.Convert(literal);

        // Assert
        result.GetType().Should().Be(type);
        result.EntityType.Should().Be(typeName);
    }
}
