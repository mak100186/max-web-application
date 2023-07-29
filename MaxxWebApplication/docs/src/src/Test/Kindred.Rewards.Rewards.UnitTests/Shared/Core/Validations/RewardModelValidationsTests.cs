using System.Collections;

using FluentAssertions;

using Kindred.Infrastructure.Core.Extensions.Extensions;

using Kindred.Rewards.Core;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.Validations;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.UnitTests.Common;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Validations;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[Category("Unit")]
public class RewardModelValidationsTests : TestBase
{
    #region ThrowIfReloadConfigurationsAreInvalid

    private static IEnumerable ValidReloadConfigurationFactory
    {
        get
        {
            yield return new TestCaseData(RewardBuilder.CreateReward<RewardDomainModel>(RewardType.UniboostReload, reward => { reward.Terms.Restrictions.Reload = null; }))
                .SetName("ThrowIfReloadIsNull")
                .Returns("Reload configuration is required if ClaimsPerInterval is not null");

            yield return new TestCaseData(RewardBuilder.CreateReward<RewardDomainModel>(RewardType.UniboostReload, reward => { reward.Terms.Restrictions.Reload.MaxReload = 0; }))
                .SetName("ThrowIfMaxReloadIsZero")
                .Returns("MaxReload should be null or greater than 0");

            yield return new TestCaseData(RewardBuilder.CreateReward<RewardDomainModel>(RewardType.UniboostReload, reward => { reward.Terms.Restrictions.Reload.StopOnMinimumWinBets = 0; }))
                .SetName("StopOnMinimumWinBetsIsZero")
                .Returns("StopOnMinimumWinBets should be greater than 0");

            yield return new TestCaseData(RewardBuilder.CreateReward<RewardDomainModel>(RewardType.UniboostReload, reward => { reward.Terms.Restrictions.ClaimsPerInterval = null; }))
                .SetName("ThrowIfClaimsPerIntervalIsNullButReloadIsNot")
                .Returns("Reload configuration is not required if ClaimsPerInterval is null");
        }
    }

    [Test]
    [TestCaseSource(typeof(RewardModelValidationsTests), nameof(ValidReloadConfigurationFactory))]
    public string ThrowsIfConditionIsMet(RewardDomainModel reward)
    {
        //arrange //act
        var exception = Assert.Throws<RewardsValidationException>(reward.ThrowIfReloadConfigurationsAreInvalid);

        //assert
        return exception.Message;
    }

    [Test]
    public void ThrowIfReloadConfigurationsAreInvalid_DoesNotThrow_WhenValidModelIsPassed()
    {
        //arrange
        var reward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.UniboostReload, null);

        //act //assert
        Assert.DoesNotThrow(() => reward.ThrowIfReloadConfigurationsAreInvalid());

    }

    #endregion

    #region GetApplicableBetTypes

    private static IEnumerable ValidBetTypeFactory
    {
        get
        {
            yield return new TestCaseData(RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, reward => { reward.Terms.RewardParameters[RewardParameterKey.AllowedFormulae] = "singles"; }))
                .SetName("ThrowIfBetTypesAreInvalid_ParsesSingleItem_SingleLeg")
                .Returns(new List<BetTypes> { BetTypes.StandardMultiLeg, BetTypes.SingleLeg });

            yield return new TestCaseData(RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, reward => { reward.Terms.RewardParameters[RewardParameterKey.AllowedFormulae] = "doubles"; }))
                .SetName("ThrowIfBetTypesAreInvalid_ParsesSingleItem_StandardMultiLeg")
                .Returns(new List<BetTypes> { BetTypes.StandardMultiLeg });

            yield return new TestCaseData(RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, reward => { reward.Terms.RewardParameters[RewardParameterKey.AllowedFormulae] = "canadian"; }))
                .SetName("ThrowIfBetTypesAreInvalid_ParsesSingleItem_SystemMultiLeg")
                .Returns(new List<BetTypes> { BetTypes.SystemMultiLeg });

            yield return new TestCaseData(RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, reward => { reward.Terms.RewardParameters[RewardParameterKey.AllowedFormulae] = "singles,doubles"; }))
                .SetName("ThrowIfBetTypesAreInvalid_ParsesTwoItems_SingleStandardMulti")
                .Returns(new List<BetTypes> { BetTypes.StandardMultiLeg, BetTypes.SingleLeg });

            yield return new TestCaseData(RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, reward => { reward.Terms.RewardParameters[RewardParameterKey.AllowedFormulae] = "singles,canadian"; }))
                .SetName("ThrowIfBetTypesAreInvalid_ParsesTwoItems_SingleSystemMulti")
                .Returns(new List<BetTypes> { BetTypes.StandardMultiLeg, BetTypes.SingleLeg, BetTypes.SystemMultiLeg });

            yield return new TestCaseData(RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, reward => { reward.Terms.RewardParameters[RewardParameterKey.AllowedFormulae] = "canadian,doubles"; }))
                .SetName("ThrowIfBetTypesAreInvalid_ParsesTwoItems_SystemMultiStandardMulti")
                .Returns(new List<BetTypes> { BetTypes.SystemMultiLeg, BetTypes.StandardMultiLeg });

            yield return new TestCaseData(RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, reward => { reward.Terms.RewardParameters[RewardParameterKey.AllowedFormulae] = "singles,doubles,canadian"; }))
                .SetName("ThrowIfBetTypesAreInvalid_ParsesThreeItems_SingleStandardMultiSystemMulti")
                .Returns(new List<BetTypes> { BetTypes.StandardMultiLeg, BetTypes.SingleLeg, BetTypes.SystemMultiLeg });
        }
    }

    [Test]
    [TestCaseSource(typeof(RewardModelValidationsTests), nameof(ValidBetTypeFactory))]
    public IReadOnlyCollection<BetTypes> ThrowIfBetTypesAreInvalid_DoesNotThrow_WhenValidValuesArePassed(RewardDomainModel reward)
    {
        //arrange
        IReadOnlyCollection<BetTypes> parsedValues = null;

        //act & assert
        Assert.DoesNotThrow(() =>
        {
            parsedValues = reward.Terms.RewardParameters.GetApplicableBetTypes();
        });

        return parsedValues;
    }

    private static IEnumerable InvalidBetTypeFactory
    {
        get
        {
            yield return new TestCaseData(RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, reward =>
                {
                    reward.Terms.RewardParameters[RewardParameterKey.AllowedFormulae] = "InvalidBetType";
                }))
                .SetName("ThrowIfBetTypesAreInvalid_ThrowsOnInvalidType_SingleItem");

            yield return new TestCaseData(RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, reward =>
                {
                    reward.Terms.RewardParameters[RewardParameterKey.AllowedFormulae] = "InvalidBetType1, InvalidBetType2";
                }))
                .SetName("ThrowIfBetTypesAreInvalid_ThrowsOnInvalidType_MultipleItems");
        }
    }

    [Test]
    [TestCaseSource(typeof(RewardModelValidationsTests), nameof(InvalidBetTypeFactory))]
    public void ThrowIfBetTypesAreInvalid_Throws_WhenInvalidValuesArePassed(RewardDomainModel reward)
    {
        //arrange & act & assert
        Assert.Throws<RewardsValidationException>(() =>
        {
            reward.ThrowIfFormulaeAreInvalid(RewardParameterKey.AllowedFormulae);
        },
        $"Reward parameter {RewardParameterKey.AllowedFormulae} must contain a valid formula. Valid values are: [{FormulaExtensions.GetAllFormulae().ToCsv()}]");
    }

    #endregion

    #region ThrowIfLegTableIsInvalid

    [Test]
    public void ThrowIfLegTableIsInvalid_Throws_WhenBetTypesAreNull()
    {
        //arrange
        var reward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, null);

        var betTypes = null as List<BetTypes>;

        //act & assert
        Assert.Throws<RewardsValidationException>(() =>
        {
            reward.ThrowIfLegTableIsInvalid(betTypes);
        }, $"{RewardParameterKey.LegTable} is invalid. See inner exception");
    }

    [TestCase("")]
    [TestCase("{}")]
    [TestCase(null)]
    public void ThrowIfLegTableIsInvalid_Throws_WhenLegTableIsNullOrEmpty(string tableValue)
    {
        //arrange
        var reward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, reward =>
        {
            reward.Terms.RewardParameters[RewardParameterKey.LegTable] = tableValue;
        });

        var betTypes = new List<BetTypes> {
            BetTypes.SingleLeg
        };

        //act & assert
        Assert.Throws<RewardsValidationException>(() =>
        {
            reward.ThrowIfLegTableIsInvalid(betTypes);
        }, $"Leg definitions for the following bet types are expected: [{betTypes.ToCsv()}]");
    }

    [TestCase(BetTypes.SingleLeg, "{\"1\":\"0\"}")]
    [TestCase(BetTypes.StandardMultiLeg, "{\"1\":\"0\",\"2\":\"0\"}")]
    [TestCase(BetTypes.SystemMultiLeg, "{\"1\":\"0\",\"2\":\"0\"}")]
    public void ThrowIfLegTableIsInvalid_DoesNotThrow_WhenExpectedLegDefinitionsAreFound(BetTypes betType, string legTable)
    {
        //arrange
        var reward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, reward =>
        {
            reward.Terms.RewardParameters[RewardParameterKey.LegTable] = legTable;
        });

        var betTypes = new List<BetTypes> {
            betType
        };

        //act & assert
        Assert.DoesNotThrow(() =>
        {
            reward.ThrowIfLegTableIsInvalid(betTypes);
        });

    }

    [TestCase(BetTypes.SingleLeg, "{\"2\":\"0\"}", "For BetTypes.SingleLeg, a leg definition for 1 leg was expected")]
    [TestCase(BetTypes.StandardMultiLeg, "{\"1\":\"0\"}", "For BetTypes.{StandardMultiLeg}, at least one leg definition for 2 and up to 20 was expected")]
    [TestCase(BetTypes.SystemMultiLeg, "{\"21\":\"0\"}", "For BetTypes.{SystemMultiLeg}, at least one leg definition for 2 and up to 20 was expected")]
    public void ThrowIfLegTableIsInvalid_Throws_WhenExpectedLegDefinitionsAreNotFound(BetTypes betType, string legTable, string errorMessage)
    {
        //arrange
        var reward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Freebet, reward =>
        {
            reward.Terms.RewardParameters[RewardParameterKey.LegTable] = legTable;
        });

        var betTypes = new List<BetTypes> {
            betType
        };

        //act & assert
        Assert.Throws<RewardsValidationException>(() =>
        {
            reward.ThrowIfLegTableIsInvalid(betTypes);
        }, errorMessage);
    }
    #endregion

    #region ThrowIfMultiConfigsAreInvalid


    [TestCase(BetTypes.SingleLeg, false)]
    [TestCase(BetTypes.StandardMultiLeg, false)]
    [TestCase(BetTypes.SystemMultiLeg, true)]
    public void ThrowIfMultiConfigsAreInvalid_Throws_WhenBetTypeIsNotOneOfAllowedBetTypes(BetTypes betType, bool shouldThrow)
    {
        //arrange
        var rewardDomainModel = BonusDomainModelBuilder.Create<RewardDomainModel>(RewardType.Profitboost, reward =>
        {
            if (betType == BetTypes.StandardMultiLeg)
            {
                reward.Terms.RewardParameters[RewardParameterKey.MinStages] = $"{DomainConstants.MinNumberOfLegsInMulti}";
            }
        });

        var applicableTo = new List<BetTypes>
        {
            betType
        };

        var allowedBetTypes = new List<BetTypes> { BetTypes.SingleLeg, BetTypes.StandardMultiLeg };

        //act && assert
        if (shouldThrow)
        {
            Assert.Throws<RewardsValidationException>(() =>
                {
                    rewardDomainModel.ThrowIfMultiConfigsAreInvalid(applicableTo, allowedBetTypes);
                },
                $"Only one of the following bet types are allowed: {allowedBetTypes.ToCsv()}");
        }
        else
        {
            rewardDomainModel.ThrowIfMultiConfigsAreInvalid(applicableTo, allowedBetTypes);
        }
    }

    [TestCase("1", "20", "minStages must be between 2 and 20")]
    [TestCase("1", "21", "minStages must be between 2 and 20")]
    [TestCase("3", "21", "maxStages must be between 2 and 20")]
    [TestCase("3", "a", "maxStages contains an invalid value")]
    [TestCase("a", "3", "minStages contains an invalid value")]
    [TestCase("18", "3", "minStages must be less than maxStages")]
    [TestCase("2", "20", null)]
    public void ThrowIfMultiConfigsAreInvalid_Throws_WhenMinAndMaxStagesAreOutOfBounds(string minStages, string maxStages, string errorMessage)
    {
        //arrange
        var rewardDomainModel = BonusDomainModelBuilder.Create<RewardDomainModel>(RewardType.Profitboost, reward =>
        {
            reward.Terms.RewardParameters[RewardParameterKey.MinStages] = minStages;
            reward.Terms.RewardParameters[RewardParameterKey.MaxStages] = maxStages;
        });

        var applicableTo = new List<BetTypes>
        {
            BetTypes.StandardMultiLeg
        };

        var allowedBetTypes = new List<BetTypes> { BetTypes.SingleLeg, BetTypes.StandardMultiLeg };

        //act && assert
        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            var exception = Assert.Throws<RewardsValidationException>(() =>
                {
                    rewardDomainModel.ThrowIfMultiConfigsAreInvalid(applicableTo, allowedBetTypes);
                });

            exception.Should().NotBeNull();
            exception?.Message.Should().Be(errorMessage);
        }
        else
        {
            rewardDomainModel.ThrowIfMultiConfigsAreInvalid(applicableTo, allowedBetTypes);
        }
    }
    #endregion
}
