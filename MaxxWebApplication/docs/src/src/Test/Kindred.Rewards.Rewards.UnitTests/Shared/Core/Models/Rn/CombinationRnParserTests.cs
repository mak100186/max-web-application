using FluentAssertions;

using Kindred.Rewards.Core.Models.Rn;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Models.Rn;

[TestFixture]
public class CombinationRnParserTests
{
    [TestCase("123e4567-e89b-12d3-a456-426614174000", 2, 1, "ksp", "combination", 1, "123e4567-e89b-12d3-a456-426614174000", 2, 1, "ksp:combination.1:123e4567-e89b-12d3-a456-426614174000:2:1", 3)]
    public void CombinationRn_ReturnsExpectedObject_WhenValueConstructorIsCalled(string guidValue, int betIindex, int stageIndex, string expectedNamespace, string expectedEntityType, int expectedSchemeVersion, string expectedGuidValue, int expectedBetIndex, int expectedStageIndex, string expectedToString, int expectedSegmentCount)
    {
        //arrange & act
        var actualObject = new CombinationRn(Guid.Parse(guidValue), betIindex, stageIndex);

        //assert
        actualObject.Namespace.Should().Be(expectedNamespace);
        actualObject.EntityType.Should().Be(expectedEntityType);
        actualObject.SchemeVersion.Should().Be(expectedSchemeVersion);
        actualObject.EntityScheme.Should().Be($"{expectedEntityType}.{expectedSchemeVersion}");
        actualObject.GuidValue.Should().Be(expectedGuidValue);
        actualObject.BetIndex.Should().Be(expectedBetIndex);
        actualObject.StageIndex.Should().Be(expectedStageIndex);
        actualObject.ToString().Should().Be(expectedToString);
        actualObject.EntitySegments.Count.Should().Be(expectedSegmentCount);
    }

    [TestCase("ksp:combination.1:123e4567-e89b-12d3-a456-426614174000:1:2", "ksp", "combination", 1, "123e4567-e89b-12d3-a456-426614174000", 1, 2, "ksp:combination.1:123e4567-e89b-12d3-a456-426614174000:1:2", 3)]
    public void CombinationRn_ReturnsExpectedObject_WhenStringConstructorIsCalled(string testString, string expectedNamespace, string expectedEntityType, int expectedSchemeVersion, string expectedGuidValue, int expectedBetIndex, int expectedStageIndex, string expectedToString, int expectedSegmentCount)
    {
        //arrange & act
        var actualObject = new CombinationRn(testString);

        //assert
        actualObject.Namespace.Should().Be(expectedNamespace);
        actualObject.EntityType.Should().Be(expectedEntityType);
        actualObject.SchemeVersion.Should().Be(expectedSchemeVersion);
        actualObject.EntityScheme.Should().Be($"{expectedEntityType}.{expectedSchemeVersion}");
        actualObject.GuidValue.Should().Be(expectedGuidValue);
        actualObject.BetIndex.Should().Be(expectedBetIndex);
        actualObject.StageIndex.Should().Be(expectedStageIndex);
        actualObject.ToString().Should().Be(expectedToString);
        actualObject.EntitySegments.Count.Should().Be(expectedSegmentCount);
    }

    [TestCase("kkk:combination.1", "Rn cannot have 2 or less segments")]
    [TestCase("kkk:combination.1:123e4567-e89b-12d3-a456-426614174000:1:1", "Invalid namespace")]
    [TestCase("ksp:combination.a:123e4567-e89b-12d3-a456-426614174000:0:1", "Invalid scheme version")]
    [TestCase("ksp:combination.1:123e4567-e89b-12d3-a456-426614174000:a:1", "Invalid bet index")]
    [TestCase("ksp:combination.1:123e4567-e89b-12d3-a456-426614174000:1:a", "Invalid stage index")]
    [TestCase("ksp:combination.1:123e4567-e89b-12d3-a456-426614174000:1", "Extra or missing values")]
    [TestCase("ksp:combination.1:123e4567-e89b-12d3-a456:0:0", "Invalid UUID")]
    [TestCase("ksp:combination:123e4567-e89b-12d3-a456-426614174000:0", "Invalid entity scheme")]
    [TestCase("ksp:combination.1.1:123e4567-e89b-12d3-a456-426614174000:0", "Invalid entity scheme")]
    [TestCase("ksp:outcome.1:[football:201711202200:watford_vs_west_ham]:1x2:plain:west_ham_united_fc", "Invalid entity type")]
    public void CombinationRn_ThrowsException_WhenCalled(string testString, string expectedErrorMessages)
    {
        //arrange act assert
        Assert.Throws<AggregateException>(() => new CombinationRn(testString),
            expectedErrorMessages);
    }
}
