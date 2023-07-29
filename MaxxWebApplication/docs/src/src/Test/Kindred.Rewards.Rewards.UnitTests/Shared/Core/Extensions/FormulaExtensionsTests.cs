using FluentAssertions;

using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Extensions;

[TestFixture]
[Category("Unit")]
[Parallelizable(ParallelScope.All)]
public class FormulaExtensionsTests
{
    [TestCase("canadian")]
    [TestCase("goliath")]
    [TestCase("heinz")]
    [TestCase("lucky15")]
    [TestCase("lucky31")]
    [TestCase("lucky63")]
    [TestCase("patent")]
    [TestCase("superheinz")]
    [TestCase("trixie")]
    [TestCase("yankee")]
    [TestCase("CANADIAN")]
    [TestCase("GOLIATH")]
    [TestCase("HEINZ")]
    [TestCase("LUCKY15")]
    [TestCase("LUCKY31")]
    [TestCase("LUCKY63")]
    [TestCase("PATENT")]
    [TestCase("SUPERHEINZ")]
    [TestCase("TRIXIE")]
    [TestCase("YANKEE")]
    public void IsSystemMulti_ReturnsExpectedResult_WhenCalled(string formula)
    {
        //arrange & act & assert
        formula.IsSystemMulti().Should().BeTrue();
    }

    [TestCase("doubles")]
    [TestCase("eighteenfolds")]
    [TestCase("eightfolds")]
    [TestCase("elevenfolds")]
    [TestCase("fifteenfolds")]
    [TestCase("fivefolds")]
    [TestCase("fourfolds")]
    [TestCase("fourteenfolds")]
    [TestCase("ninefolds")]
    [TestCase("nineteenfolds")]
    [TestCase("sevenfolds")]
    [TestCase("seventeenfolds")]
    [TestCase("singles")]
    [TestCase("sixfolds")]
    [TestCase("sixteenfolds")]
    [TestCase("tenfolds")]
    [TestCase("thirteenfolds")]
    [TestCase("trebles")]
    [TestCase("twelvefolds")]
    [TestCase("twentyfolds")]
    [TestCase("DOUBLES")]
    [TestCase("EIGHTEENFOLDS")]
    [TestCase("EIGHTFOLDS")]
    [TestCase("ELEVENFOLDS")]
    [TestCase("FIFTEENFOLDS")]
    [TestCase("FIVEFOLDS")]
    [TestCase("FOURFOLDS")]
    [TestCase("FOURTEENFOLDS")]
    [TestCase("NINEFOLDS")]
    [TestCase("NINETEENFOLDS")]
    [TestCase("SEVENFOLDS")]
    [TestCase("SEVENTEENFOLDS")]
    [TestCase("SINGLES")]
    [TestCase("SIXFOLDS")]
    [TestCase("SIXTEENFOLDS")]
    [TestCase("TENFOLDS")]
    [TestCase("THIRTEENFOLDS")]
    [TestCase("TREBLES")]
    [TestCase("TWELVEFOLDS")]
    [TestCase("TWENTYFOLDS")]
    public void IsStandardMulti_ReturnsExpectedResult_WhenCalled(string formula)
    {
        //arrange & act & assert
        formula.IsStandardMulti().Should().BeTrue();
    }

    [Test]
    public void IsStandardMulti_ReturnsFalse_WhenCalledWithUnknownValue()
    {
        //arrange & act & assert
        var formula = ObjectBuilder.CreateString();

        formula.IsStandardMulti().Should().BeFalse();
    }

    [Test]
    public void IsSystemMulti_ReturnsFalse_WhenCalledWithUnknownValue()
    {
        //arrange & act & assert
        var formula = ObjectBuilder.CreateString();

        formula.IsSystemMulti().Should().BeFalse();
    }
}
