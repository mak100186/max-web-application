using FluentAssertions;

using Kindred.Rewards.Core.Models.Rn;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Models.Rn;

[TestFixture]
public class ContestKeyTests
{
    [TestCase("football", "201711202200", "watford_vs_west_ham", "[football:201711202200:watford_vs_west_ham]")]
    public void ContestKey_ReturnsExpectedObject_WhenValueConstructorIsCalled(string type, string dateTime, string identifier, string expectedToString)
    {
        //arrange & act
        var actualObject = new ContestKey(type, dateTime, identifier);

        //assert
        actualObject.ToString().Should().Be(expectedToString);
    }

    [TestCase("[football:201711202200:watford_vs_west_ham]", "football", "201711202200", "watford_vs_west_ham")]
    [TestCase("football:201711202200:watford_vs_west_ham", "football", "201711202200", "watford_vs_west_ham")]
    public void ContestKey_ReturnsExpectedObject_WhenLiteralConstructorIsCalled(string literal, string type, string dateTime, string identifier)
    {
        //arrange & act
        var actualObject = new ContestKey(literal);

        //assert
        actualObject.Type.Should().Be(type);
        actualObject.DateTime.Should().Be(dateTime);
        actualObject.Identifier.Should().Be(identifier);
    }

    [TestCase("football:201711202200", "Contest Key must have 3 segments")]
    [TestCase(":201711202200:watford_vs_west_ham", "Contest Type cannot be empty")]
    [TestCase("football::watford_vs_west_ham", "Contest DateTime cannot be empty")]
    [TestCase("football:20171120:watford_vs_west_ham", "Invalid Contest DateTime")]
    [TestCase("football:201711202200:", "Contest Identifier cannot be empty")]
    public void ContestKey_ThrowsException_WhenCalled(string testString, string expectedErrorMessages)
    {
        //arrange act assert
        Assert.Throws<AggregateException>(() => new ContestKey(testString),
            expectedErrorMessages);
    }
}
