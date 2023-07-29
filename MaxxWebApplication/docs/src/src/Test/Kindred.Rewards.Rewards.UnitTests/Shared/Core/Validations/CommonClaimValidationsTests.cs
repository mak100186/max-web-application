using FluentAssertions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.Validations;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Validations;

[TestFixture]
[Category("Unit")]
[Parallelizable(ParallelScope.All)]
public class CommonClaimValidationsTests
{

    [Test]
    public void ValidateBetType_ReturnTrue_WhenBetTypeIsInApplicableToList()
    {
        //arrange
        var rewardClaimDomainModel = ObjectBuilder.CreateRewardClaim(RewardType.Profitboost, 1, 2.0m);
        var batchClaimItemDomainModel = ObjectBuilder.CreateBatchClaimItemDomainModel(rewardClaimDomainModel.Bet, rewardClaimDomainModel.Hash, ObjectBuilder.CreateString());
        var claimResult = new ClaimResultDomainModel
        {
            Claim = rewardClaimDomainModel
        };

        //act
        CommonClaimValidations.ValidateBetType(rewardClaimDomainModel, batchClaimItemDomainModel, claimResult);

        //assert
        claimResult.Success.Should().BeTrue();
    }

    //Background: We are not allowing Formula/System Multis for Profit boost
    [Test]
    public void ValidateBetType_ReturnFalse_WhenBetTypeIsNotInApplicableToList()
    {
        //arrange
        const int stages = 2;
        const string formula = "canadian";

        var rewardClaimDomainModel = ObjectBuilder.CreateRewardClaim(RewardType.Profitboost, stages, 2.0m);
        rewardClaimDomainModel.Bet.Formula = formula;

        var batchClaimItemDomainModel = ObjectBuilder.CreateBatchClaimItemDomainModel(rewardClaimDomainModel.Bet, rewardClaimDomainModel.Hash, ObjectBuilder.CreateString());
        var claimResult = new ClaimResultDomainModel
        {
            Claim = rewardClaimDomainModel
        };

        //act
        CommonClaimValidations.ValidateBetType(rewardClaimDomainModel, batchClaimItemDomainModel, claimResult);

        //assert
        claimResult.Success.Should().BeFalse();
    }

    //Background: We are not allowing Formula/System Multis for Profit boost
    [Test]
    public void ValidateBetType_ReturnFalse_WhenUnknownBetTypeIsPassedInApplicableToList()
    {
        //arrange
        const string unknownBetType = "unknownBetType";

        var rewardClaimDomainModel = ObjectBuilder.CreateRewardClaim(RewardType.Profitboost, 1, 2.0m);
        rewardClaimDomainModel.Terms.RewardParameters[RewardParameterKey.AllowedFormulae] = unknownBetType;

        var batchClaimItemDomainModel = ObjectBuilder.CreateBatchClaimItemDomainModel(rewardClaimDomainModel.Bet, rewardClaimDomainModel.Hash, ObjectBuilder.CreateString());
        var claimResult = new ClaimResultDomainModel
        {
            Claim = rewardClaimDomainModel
        };

        //act
        CommonClaimValidations.ValidateBetType(rewardClaimDomainModel, batchClaimItemDomainModel, claimResult);

        //assert
        claimResult.Success.Should().BeFalse();
        claimResult.ErrorMessage.Should().Contain("Bet type of SingleLeg is not an allowed bet type for this reward.");
    }

    [Test]
    public void ValidateSingleBetType_ReturnsTrue_IfBetHasSingleStage()
    {
        // Arrange
        var rewardClaimDomainModel = ObjectBuilder.CreateRewardClaim(RewardType.Uniboost, 1, 2.5m);
        rewardClaimDomainModel.Terms.RewardParameters.Add(RewardParameterKey.MinStakeAmount, "2.0");

        var batchClaimItemDomainModel = ObjectBuilder.CreateBatchClaimItemDomainModel(
            rewardClaimDomainModel.Bet,
            rewardClaimDomainModel.Hash,
            ObjectBuilder.CreateString());

        var claimResult = new ClaimResultDomainModel
        {
            Claim = rewardClaimDomainModel
        };

        // Act
        CommonClaimValidations.ValidateSingleBetType(rewardClaimDomainModel, batchClaimItemDomainModel, claimResult);

        // Assert
        claimResult.Success.Should().BeTrue();
    }

    [Test]
    public void ValidateSingleBetType_ReturnsFalse_IfBetIsNotSingleStage([Random(2, 999, 4)] int stageCount)
    {
        // Arrange
        var rewardClaimDomainModel = ObjectBuilder.CreateRewardClaim(RewardType.Uniboost, stageCount, 2.5m);
        rewardClaimDomainModel.Terms.RewardParameters.Add(RewardParameterKey.MinStakeAmount, "2.0");

        var batchClaimItemDomainModel = ObjectBuilder.CreateBatchClaimItemDomainModel(
            rewardClaimDomainModel.Bet,
            rewardClaimDomainModel.Hash,
            ObjectBuilder.CreateString());

        var claimResult = new ClaimResultDomainModel
        {
            Claim = rewardClaimDomainModel
        };

        // Act
        CommonClaimValidations.ValidateSingleBetType(rewardClaimDomainModel, batchClaimItemDomainModel, claimResult);

        // Assert
        claimResult.Success.Should().BeFalse();
    }

    [TestCase(1, true)]
    [TestCase(2, true)]
    [TestCase(20, true)]
    [TestCase(21, false)]
    public void ValidateMinNumberOfLegs_ReturnExpectedResult_WhenCalled(int stageCount, bool expectedResult)
    {
        //arrange
        var rewardClaimDomainModel = ObjectBuilder.CreateRewardClaim(RewardType.Profitboost, stageCount, 2.0m);
        var batchClaimItemDomainModel = ObjectBuilder.CreateBatchClaimItemDomainModel(rewardClaimDomainModel.Bet, rewardClaimDomainModel.Hash, ObjectBuilder.CreateString());
        var claimResult = new ClaimResultDomainModel
        {
            Claim = rewardClaimDomainModel
        };

        //act
        CommonClaimValidations.ValidateMinNumberOfLegs(rewardClaimDomainModel, batchClaimItemDomainModel, claimResult);

        //assert
        claimResult.Success.Should().Be(expectedResult);
    }

    [TestCase("1.0,2.0", false)]
    [TestCase("2.0,3.0", true)]
    [TestCase("2.0", true)]
    [TestCase("0.5", false)]
    [TestCase("0.5,2.0,2.0", false)]
    [TestCase("3.0,3.0,3.0", true)]
    public void ValidateMinimumStageOdds_ReturnExpectedResult_WhenCalled(string stageData, bool expectedResult)
    {
        // Arrange
        const string minimumStageOdds = "2.0";

        var stages = stageData.Split(",");
        var stageCount = stages.Length;
        var rewardClaimDomainModel = ObjectBuilder.CreateRewardClaim(RewardType.Uniboost, stageCount, 2.0m);

        for (var i = 0; i < stages.Length; i++)
        {
            var odds = decimal.Parse(stages[i]);
            rewardClaimDomainModel.Bet.Stages.ElementAt(i).RequestedPrice = odds;
        }

        rewardClaimDomainModel.Terms.RewardParameters.Add(RewardParameterKey.MinimumStageOdds, minimumStageOdds);

        var batchClaimItemDomainModel = ObjectBuilder.CreateBatchClaimItemDomainModel(rewardClaimDomainModel.Bet, rewardClaimDomainModel.Hash, ObjectBuilder.CreateString());
        var claimResult = new ClaimResultDomainModel
        {
            Claim = rewardClaimDomainModel
        };

        // Act
        CommonClaimValidations.ValidateMinimumStageOdds(rewardClaimDomainModel, batchClaimItemDomainModel, claimResult);

        // Assert
        claimResult.Success.Should().Be(expectedResult);
    }

    [TestCase("1.0,2.0", true)]
    [TestCase("1.0,1.9999", false)]
    [TestCase("2.0,3.0", true)]
    [TestCase("2.0", true)]
    [TestCase("0.5", false)] //we don't test single bet
    [TestCase("0.5,2.0,2.0", true)]
    [TestCase("0.4999,2.0,2.0", false)]
    [TestCase("3.0,3.0,3.0", true)]
    public void ValidateMinimumCompoundOdds_ReturnExpectedResult_WhenCalled(string stageData, bool expectedResult)
    {
        //arrange
        const string minimumCompoundOdds = "2.0";

        var stages = stageData.Split(",");
        var stageCount = stages.Length;
        var rewardClaimDomainModel = ObjectBuilder.CreateRewardClaim(RewardType.Profitboost, stageCount, 2.0m);

        for (var i = 0; i < stages.Length; i++)
        {
            var odds = decimal.Parse(stages[i]);
            rewardClaimDomainModel.Bet.Stages.ElementAt(i).RequestedPrice = odds;
        }

        rewardClaimDomainModel.Terms.RewardParameters.Add(RewardParameterKey.MinimumCompoundOdds, minimumCompoundOdds);

        var batchClaimItemDomainModel = ObjectBuilder.CreateBatchClaimItemDomainModel(rewardClaimDomainModel.Bet, rewardClaimDomainModel.Hash, ObjectBuilder.CreateString());
        var claimResult = new ClaimResultDomainModel
        {
            Claim = rewardClaimDomainModel
        };

        //act
        CommonClaimValidations.ValidateMinimumCompoundOdds(rewardClaimDomainModel, batchClaimItemDomainModel, claimResult);

        //assert
        claimResult.Success.Should().Be(expectedResult);
    }
}
