using FluentAssertions;

using Kindred.Rewards.Core.Models.Rn;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Models.Rn;

[TestFixture]
public class MarketRnParserTests
{
    [TestCase("football", "201711202200", "watford_vs_west_ham", "1x2", "bg", "ksp", "market", 1, "ksp:market.1:[football:201711202200:watford_vs_west_ham]:1x2:bg", 5)]
    public void MarketRn_ReturnsExpectedObject_WhenValueConstructorIsCalled(string type, string dateTime, string identifier, string propositionKey, string tenant, string expectedNamespace, string expectedEntityType, int expectedSchemeVersion, string expectedToString, int expectedSegmentCount)
    {
        //arrange & act
        var actualObject = new MarketRn(new(type, dateTime, identifier), propositionKey, tenant);

        //assert
        actualObject.Namespace.Should().Be(expectedNamespace);
        actualObject.EntityType.Should().Be(expectedEntityType);
        actualObject.SchemeVersion.Should().Be(expectedSchemeVersion);
        actualObject.EntityScheme.Should().Be($"{expectedEntityType}.{expectedSchemeVersion}");
        actualObject.ToString().Should().Be(expectedToString);
        actualObject.EntitySegments.Count.Should().Be(expectedSegmentCount);
    }

    [TestCase("ksp:market.1:[football:201711202200:watford_vs_west_ham]:1x2:bg", "ksp", "market", 1, "football", "201711202200", "watford_vs_west_ham", "1x2", "bg", "ksp:market.1:[football:201711202200:watford_vs_west_ham]:1x2:bg", 5)]
    public void MarketRn_ReturnsExpectedObject_WhenStringConstructorIsCalled(string testString, string expectedNamespace, string expectedEntityType, int expectedSchemeVersion, string expectedContestType, string expectedDateTime, string expectedIdentifier, string expectedPropositionKey, string expectedTenant, string expectedToString, int expectedSegmentCount)
    {
        //arrange & act
        var actualObject = new MarketRn(testString);

        //assert
        actualObject.Namespace.Should().Be(expectedNamespace);
        actualObject.EntityType.Should().Be(expectedEntityType);
        actualObject.SchemeVersion.Should().Be(expectedSchemeVersion);
        actualObject.EntityScheme.Should().Be($"{expectedEntityType}.{expectedSchemeVersion}");
        actualObject.ContestKey.Type.Should().Be(expectedContestType);
        actualObject.ContestKey.DateTime.Should().Be(expectedDateTime);
        actualObject.ContestKey.Identifier.Should().Be(expectedIdentifier);
        actualObject.PropositionKey.Should().Be(expectedPropositionKey);
        actualObject.Tenant.Should().Be(expectedTenant);

        actualObject.ToString().Should().Be(expectedToString);
        actualObject.EntitySegments.Count.Should().Be(expectedSegmentCount);
    }

    [TestCase("ksp:market.1", "Rn cannot have 2 or less segments")]
    [TestCase("kkk:market.1:[football:201711202200:watford_vs_west_ham]:1x2:bg", "Invalid namespace")]
    [TestCase("ksp:market.a:[football:201711202200:watford_vs_west_ham]:1x2:bg", "Invalid scheme version")]
    [TestCase("ksp:market.1:[football:201711202200:watford_vs_west_ham]:1x2:bg:1", "Extra or missing values")]
    [TestCase("ksp:market.1:[football:201711202200:watford_vs_west_ham]:1x2:xx", "Extra or missing values")]
    [TestCase("ksp:market:[football:201711202200:watford_vs_west_ham]:1x2:bg", "Invalid entity scheme")]
    [TestCase("ksp:market.1.1:[football:201711202200:watford_vs_west_ham]:1x2:bg", "Invalid entity scheme")]
    [TestCase("ksp:variant.1:[football:201711202200:watford_vs_west_ham]:1x2:bg", "Invalid entity type")]
    public void MarketRn_ThrowsException_WhenCalled(string testString, string expectedErrorMessages)
    {
        //arrange act assert
        Assert.Throws<AggregateException>(() => new MarketRn(testString),
            expectedErrorMessages);
    }
}
