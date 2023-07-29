using Kindred.Rewards.Core.Extensions;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Extensions;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[Category("Unit")]
public class NumberExtensionsTests
{
    [TestCase(1, 3, 2, true)]
    [TestCase(1, 10, 5, true)]
    [TestCase(1, 3, 3, false)]
    [TestCase(1, 3, 1, false)]
    public void IsInRangeExclusive_ReturnsExpectedValue_WhenCalled(int min, int max, int subject, bool expectedResult)
    {
        //arrange & act
        var actualResult = subject.IsInRangeExclusive(min, max);

        //assert
        Assert.AreEqual(expectedResult, actualResult);
    }
}
