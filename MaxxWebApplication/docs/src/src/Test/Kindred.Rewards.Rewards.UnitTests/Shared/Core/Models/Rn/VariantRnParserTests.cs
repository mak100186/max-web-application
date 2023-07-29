using FluentAssertions;

using Kindred.Rewards.Core.Models.Rn;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Models.Rn;

[TestFixture]
public class VariantRnParserTests
{
    [TestCase("football", "201711202200", "watford_vs_west_ham", "1x2", "plain", "ksp", "variant", 1, "ksp:variant.1:[football:201711202200:watford_vs_west_ham]:1x2:plain", 5)]
    public void VariantRn_ReturnsExpectedObject_WhenValueConstructorIsCalled(string type, string dateTime, string identifier, string propositionKey, string variantKey, string expectedNamespace, string expectedEntityType, int expectedSchemeVersion, string expectedToString, int expectedSegmentCount)
    {
        //arrange & act
        var actualObject = new VariantRn(new(type, dateTime, identifier), propositionKey, variantKey);

        //assert
        actualObject.Namespace.Should().Be(expectedNamespace);
        actualObject.EntityType.Should().Be(expectedEntityType);
        actualObject.SchemeVersion.Should().Be(expectedSchemeVersion);
        actualObject.EntityScheme.Should().Be($"{expectedEntityType}.{expectedSchemeVersion}");
        actualObject.ToString().Should().Be(expectedToString);
        actualObject.EntitySegments.Count.Should().Be(expectedSegmentCount);
    }

    [TestCase("ksp:variant.1:[football:201711202200:watford_vs_west_ham]:1x2:plain", "ksp", "variant", 1, "football", "201711202200", "watford_vs_west_ham", "1x2", "plain", "ksp:variant.1:[football:201711202200:watford_vs_west_ham]:1x2:plain", 5)]
    public void VariantRn_ReturnsExpectedObject_WhenStringConstructorIsCalled(string testString, string expectedNamespace, string expectedEntityType, int expectedSchemeVersion, string expectedContestType, string expectedDateTime, string expectedIdentifier, string expectedPropositionKey, string expectedVariantKey, string expectedToString, int expectedSegmentCount)
    {
        //arrange & act
        var actualObject = new VariantRn(testString);

        //assert
        actualObject.Namespace.Should().Be(expectedNamespace);
        actualObject.EntityType.Should().Be(expectedEntityType);
        actualObject.SchemeVersion.Should().Be(expectedSchemeVersion);
        actualObject.EntityScheme.Should().Be($"{expectedEntityType}.{expectedSchemeVersion}");
        actualObject.ContestKey.Type.Should().Be(expectedContestType);
        actualObject.ContestKey.DateTime.Should().Be(expectedDateTime);
        actualObject.ContestKey.Identifier.Should().Be(expectedIdentifier);
        actualObject.PropositionKey.Should().Be(expectedPropositionKey);
        actualObject.VariantKey.Should().Be(expectedVariantKey);

        actualObject.ToString().Should().Be(expectedToString);
        actualObject.EntitySegments.Count.Should().Be(expectedSegmentCount);
    }

    [TestCase("ksp:variant.1", "Rn cannot have 2 or less segments")]
    [TestCase("kkk:variant.1:[football:201711202200:watford_vs_west_ham]:1x2:plain", "Invalid namespace")]
    [TestCase("ksp:variant.a:[football:201711202200:watford_vs_west_ham]:1x2:plain", "Invalid scheme version")]
    [TestCase("ksp:variant.1:[football:201711202200:watford_vs_west_ham]:1x2:plain:1", "Extra or missing values")]
    [TestCase("ksp:variant:[football:201711202200:watford_vs_west_ham]:1x2:plain", "Invalid entity scheme")]
    [TestCase("ksp:variant.1.1:[football:201711202200:watford_vs_west_ham]:1x2:plain", "Invalid entity scheme")]
    [TestCase("ksp:market.1:[football:201711202200:watford_vs_west_ham]:1x2:bg", "Invalid entity type")]
    [TestCase("ksp:variant.1:[football:201711202200:watford_vs_west_ham]:1x2:bg", "Invalid variant key")]
    public void VariantRn_ThrowsException_WhenCalled(string testString, string expectedErrorMessages)
    {
        //arrange act assert
        Assert.Throws<AggregateException>(() => new VariantRn(testString),
            expectedErrorMessages);
    }
}
