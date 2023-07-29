using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Rewards.Tests.Common;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Extensions;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[Category("Unit")]
public class DecimalExtensionsTests
{
    [TestCase(null, 3, "1.111")]
    [TestCase(null, 2, "1.11")]
    [TestCase("JOD", 2, "1.111")]
    [TestCase(TestConstants.DefaultCurrencyCode, 2, "1.11")]
    public void RoundToDecimalPlaces_ReturnsExpectedResult_WhenCalled(string currencyCode, int fallbackDecimalPlaces,
        string expectedResultString)
    {
        //arrange
        const decimal amount = 1.11111m;
        var expectedResult = decimal.Parse(expectedResultString);

        //act
        var actualResult = amount.RoundToDecimalPlaces(currencyCode, fallbackDecimalPlaces);

        //assert
        Assert.AreEqual(expectedResult, actualResult);
    }
}
