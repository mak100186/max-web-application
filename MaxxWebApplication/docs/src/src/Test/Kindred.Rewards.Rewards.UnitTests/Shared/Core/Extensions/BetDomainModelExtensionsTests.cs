using System.Collections;
using System.ComponentModel.DataAnnotations;

using FluentAssertions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models.RewardClaims.Bet;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Extensions;

[TestFixture]
[Category("Unit")]
[Parallelizable(ParallelScope.All)]
public class BetDomainModelExtensionsTests
{
    private static IEnumerable TestCaseDataFactory
    {
        get
        {
            yield return new TestCaseData(
                    new BetDomainModel
                    {
                        Formula = "singles",
                        Stages = new List<CompoundStageDomainModel>
                        {
                            new()
                        }
                    })
                .SetName("Bet with 1 stage and singles formula")
                .Returns(BetTypes.SingleLeg);

            yield return new TestCaseData(
                    new BetDomainModel
                    {
                        Formula = "singles",
                        Stages = new List<CompoundStageDomainModel>
                        {
                            new(),
                            new()
                        }
                    })
                .SetName("Bet with 2 stages and singles formula")
                .Returns(BetTypes.StandardMultiLeg);

            yield return new TestCaseData(
                    new BetDomainModel
                    {
                        Formula = "singles",
                        Stages = new List<CompoundStageDomainModel>
                        {
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("Bet with 3 stages and singles formula")
                .Returns(BetTypes.StandardMultiLeg);

            yield return new TestCaseData(
                    new BetDomainModel
                    {
                        Formula = "fourfolds",
                        Stages = new List<CompoundStageDomainModel>
                        {
                            new(),
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("Bet with 4 stages and fourfolds formula")
                .Returns(BetTypes.StandardMultiLeg);

            yield return new TestCaseData(
                    new BetDomainModel
                    {
                        Formula = "fourfolds",
                        Stages = new List<CompoundStageDomainModel>
                        {
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("Bet with 3 stages and fourfolds formula")
                .Returns(BetTypes.StandardMultiLeg);

            yield return new TestCaseData(
                    new BetDomainModel
                    {
                        Formula = "canadian",
                        Stages = new List<CompoundStageDomainModel>
                        {
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("Bet with 3 stages and canadian formula")
                .Returns(BetTypes.SystemMultiLeg);

            yield return new TestCaseData(
                    new BetDomainModel
                    {
                        Formula = "canadian",
                        Stages = new List<CompoundStageDomainModel>
                        {
                            new(),
                            new(),
                            new()
                        }
                    })
                .SetName("Bet with 3 stages and unrecognized formula")
                .Returns(BetTypes.SystemMultiLeg);
        }
    }


    [TestCaseSource(typeof(BetDomainModelExtensionsTests), nameof(TestCaseDataFactory))]
    public BetTypes DeduceBetType_DeducesBetType_WhenConditionsAreMet(BetDomainModel bet)
    {
        //arrange & act & assert
        return bet.DeduceBetType();
    }

    [Test]
    public void DeduceBetType_Throws_WhenStageCountIsZero()
    {
        //arrange
        var bet = new BetDomainModel
        {
            Stages = new List<CompoundStageDomainModel>()
        };

        //act
        var exception = Assert.Throws<ValidationException>(() => { bet.DeduceBetType(); });

        //assert
        exception.Should().NotBeNull();
        exception.Message.Should().Contain("Bet has no stages");
    }
}
