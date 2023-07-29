using FluentAssertions;

using Kindred.Rewards.Core.Models.Rn;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Models.Rn;

[TestFixture]
public class OutcomeRnParserTests
{
    [TestCase("football", "201711202200", "watford_vs_west_ham", "1x2", "plain", "west_ham_united_fc", "ksp", "outcome", 1, "ksp:outcome.1:[football:201711202200:watford_vs_west_ham]:1x2:plain:west_ham_united_fc", 6)]
    public void OutcomeRn_ReturnsExpectedObject_WhenValueConstructorIsCalled(string type, string dateTime, string identifier, string propositionKey, string variantKey, string optionKey, string expectedNamespace, string expectedEntityType, int expectedSchemeVersion, string expectedToString, int expectedSegmentCount)
    {
        //arrange & act
        var actualObject = new OutcomeRn(new(type, dateTime, identifier), propositionKey, variantKey, optionKey);

        //assert
        actualObject.Namespace.Should().Be(expectedNamespace);
        actualObject.EntityType.Should().Be(expectedEntityType);
        actualObject.SchemeVersion.Should().Be(expectedSchemeVersion);
        actualObject.EntityScheme.Should().Be($"{expectedEntityType}.{expectedSchemeVersion}");
        actualObject.ToString().Should().Be(expectedToString);
        actualObject.EntitySegments.Count.Should().Be(expectedSegmentCount);
    }

    [TestCase("ksp:outcome.1:[football:201711202200:watford_vs_west_ham]:1x2:plain:west_ham_united_fc", "ksp", "outcome", 1, "football", "201711202200", "watford_vs_west_ham", "1x2", "plain", "west_ham_united_fc", "ksp:outcome.1:[football:201711202200:watford_vs_west_ham]:1x2:plain:west_ham_united_fc", 6)]
    public void OutcomeRn_ReturnsExpectedObject_WhenStringConstructorIsCalled(string testString, string expectedNamespace, string expectedEntityType, int expectedSchemeVersion, string expectedContestType, string expectedDateTime, string expectedIdentifier, string expectedPropositionKey, string expectedVariantKey, string expectedOptionKey, string expectedToString, int expectedSegmentCount)
    {
        //arrange & act
        var actualObject = new OutcomeRn(testString);

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
        actualObject.OptionKey.Should().Be(expectedOptionKey);

        actualObject.ToString().Should().Be(expectedToString);
        actualObject.EntitySegments.Count.Should().Be(expectedSegmentCount);
    }

    [TestCase("ksp:outcome.1", "Rn cannot have 2 or less segments")]
    [TestCase("kkk:outcome.1:[football:201711202200:watford_vs_west_ham]:1x2:plain:west_ham_united_fc", "Invalid namespace")]
    [TestCase("ksp:outcome.a:[football:201711202200:watford_vs_west_ham]:1x2:plain:west_ham_united_fc", "Invalid scheme version")]
    [TestCase("ksp:outcome.1:[football:201711202200:watford_vs_west_ham]:1x2:plain:west_ham_united_fc:1", "Extra or missing values")]
    [TestCase("ksp:outcome:[football:201711202200:watford_vs_west_ham]:1x2:plain:west_ham_united_fc", "Invalid entity scheme")]
    [TestCase("ksp:outcome.1.1:[football:201711202200:watford_vs_west_ham]:1x2:plain:west_ham_united_fc", "Invalid entity scheme")]
    [TestCase("ksp:variant.1:[football:201711202200:watford_vs_west_ham]:1x2:plain:west_ham_united_fc", "Invalid entity type")]
    public void OutcomeRn_ThrowsException_WhenCalled(string testString, string expectedErrorMessages)
    {
        //arrange act assert
        Assert.Throws<AggregateException>(() => new OutcomeRn(testString),
            expectedErrorMessages);
    }
}
