using AutoFixture;
using AutoFixture.AutoMoq;

using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models.Events;
using Kindred.Rewards.Rewards.UnitTests.Common;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Extensions;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[Category("Unit")]
public class RewardBetExtensionsTests : TestBase
{
    [SetUp]
    public void Setup()
    {
        Fixture.Customize(new AutoMoqCustomization());
    }

    [TestCase(1, true)]
    [TestCase(10, false)]
    public void IsSingleBet_ReturnsExpectedResult_WhenCalled(int betLegCount, bool expectedResults)
    {
        //arrange
        var rewardBet = Fixture.Create<RewardBet>();
        var compoundStages = Fixture.CreateMany<CompoundStage>(betLegCount).ToList();
        rewardBet.Stages = compoundStages;

        //act
        var actualResult = rewardBet.IsSingleBet();

        //assert
        Assert.AreEqual(expectedResults, actualResult);
    }

    [TestCase(1, false)]
    [TestCase(10, true)]
    public void IsMultiBet_ReturnsExpectedResult_WhenCalled(int betLegCount, bool expectedResults)
    {
        //arrange
        var rewardBet = Fixture.Create<RewardBet>();
        var stages = Fixture.CreateMany<CompoundStage>(betLegCount).ToList();
        rewardBet.Stages = stages;

        //act
        var actualResult = rewardBet.IsMultiBet();

        //assert
        Assert.AreEqual(expectedResults, actualResult);
    }
}
