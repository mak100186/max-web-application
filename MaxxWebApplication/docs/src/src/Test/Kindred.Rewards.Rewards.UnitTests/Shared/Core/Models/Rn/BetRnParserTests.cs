using FluentAssertions;

using Kindred.Rewards.Core.Models.Rn;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Models.Rn;

[TestFixture]
public class BetRnParserTests
{
    [TestCase("123e4567-e89b-12d3-a456-426614174000", 2, "ksp", "bet", 1, "123e4567-e89b-12d3-a456-426614174000", 2, "ksp:bet.1:123e4567-e89b-12d3-a456-426614174000:2", 2)]
    public void BetRn_ReturnsExpectedObject_WhenValueConstructorIsCalled(string guidValue, int index, string expectedNamespace, string expectedEntityType, int expectedSchemeVersion, string expectedGuidValue, int expectedIndex, string expectedToString, int expectedSegmentCount)
    {
        //arrange & act
        var actualObject = new BetRn(Guid.Parse(guidValue), index);

        //assert
        actualObject.Namespace.Should().Be(expectedNamespace);
        actualObject.EntityType.Should().Be(expectedEntityType);
        actualObject.SchemeVersion.Should().Be(expectedSchemeVersion);
        actualObject.EntityScheme.Should().Be($"{expectedEntityType}.{expectedSchemeVersion}");
        actualObject.GuidValue.Should().Be(expectedGuidValue);
        actualObject.Index.Should().Be(expectedIndex);
        actualObject.ToString().Should().Be(expectedToString);
        actualObject.EntitySegments.Count.Should().Be(expectedSegmentCount);
    }

    [TestCase("ksp:bet.1:123e4567-e89b-12d3-a456-426614174000:1", "ksp", "bet", 1, "123e4567-e89b-12d3-a456-426614174000", 1, "ksp:bet.1:123e4567-e89b-12d3-a456-426614174000:1", 2)]
    public void BetRn_ReturnsExpectedObject_WhenStringConstructorIsCalled(string testString, string expectedNamespace, string expectedEntityType, int expectedSchemeVersion, string expectedGuidValue, int expectedIndex, string expectedToString, int expectedSegmentCount)
    {
        //arrange & act
        var actualObject = new BetRn(testString);


        //assert
        actualObject.Namespace.Should().Be(expectedNamespace);
        actualObject.EntityType.Should().Be(expectedEntityType);
        actualObject.SchemeVersion.Should().Be(expectedSchemeVersion);
        actualObject.EntityScheme.Should().Be($"{expectedEntityType}.{expectedSchemeVersion}");
        actualObject.GuidValue.Should().Be(expectedGuidValue);
        actualObject.Index.Should().Be(expectedIndex);
        actualObject.ToString().Should().Be(expectedToString);
        actualObject.EntitySegments.Count.Should().Be(expectedSegmentCount);
    }

    [TestCase("kkk:bet.1", "Rn cannot have 2 or less segments")]
    [TestCase("kkk:bet.1:123e4567-e89b-12d3-a456-426614174000:0", "Invalid namespace")]
    [TestCase("ksp:bet.a:123e4567-e89b-12d3-a456-426614174000:0", "Invalid scheme version")]
    [TestCase("ksp:bet.1:123e4567-e89b-12d3-a456-426614174000:a", "Invalid bet index")]
    [TestCase("ksp:bet.1:123e4567-e89b-12d3-a456-426614174000:1:1", "Extra or missing values")]
    [TestCase("ksp:bet.1:123e4567-e89b-12d3-a456:0", "Invalid UUID")]
    [TestCase("ksp:bet:123e4567-e89b-12d3-a456-426614174000:0", "Invalid entity scheme")]
    [TestCase("ksp:bet.1.1:123e4567-e89b-12d3-a456-426614174000:0", "Invalid entity scheme")]
    [TestCase("ksp:outcome.1:[football:201711202200:watford_vs_west_ham]:1x2:plain:west_ham_united_fc", "Invalid entity type")]
    public void BetRn_ThrowsException_WhenCalled(string testString, string expectedErrorMessages)
    {
        //arrange act assert
        Assert.Throws<AggregateException>(() => new BetRn(testString),
            expectedErrorMessages);
    }
}
