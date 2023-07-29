using Kindred.Rewards.Core.Extensions;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Extensions;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[Category("Unit")]
public class FormulaeExtensionsTests
{
    [TestCase(null, 0)]
    [TestCase("", 0)]
    [TestCase(" ", 0)]
    [TestCase("aaa", 1)]
    [TestCase("aaa,bbb,ccc", 3)]
    public void ExtractValues_ReturnsExpectedCount_WhenCalled(string formulae, int expectedCount)
    {
        //act
        var result = formulae.ExtractValues();

        //assert
        Assert.AreEqual(expectedCount, result.Count());
    }
}

