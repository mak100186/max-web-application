using Kindred.Rewards.Core.Helpers;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Helpers;

[TestFixture]
public class RegionHelperTests
{
    [TestCase("US")]
    [TestCase("AU")]
    [TestCase("PK")]
    [TestCase("RU")]
    public void IsIso2Valid_ReturnsTrue_WhenCalled(string code)
    {
        //arrange & act
        var result = RegionHelper.IsIso2Valid(code);

        //assert
        Assert.IsTrue(result);
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("AUS")]
    [TestCase("AUST")]
    [TestCase("101")]
    public void IsIso2Valid_ReturnsFalse_WhenCalled(string code)
    {
        //arrange & act
        var result = RegionHelper.IsIso2Valid(code);

        //assert
        Assert.IsFalse(result);
    }

    [TestCase("USA")]
    [TestCase("AUS")]
    [TestCase("PAK")]
    [TestCase("RUS")]
    public void IsIso3Valid_ReturnsTrue_WhenCalled(string code)
    {
        //arrange & act
        var result = RegionHelper.IsIso3Valid(code);

        //assert
        Assert.IsTrue(result);
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("AU")]
    [TestCase("AUST")]
    [TestCase("101")]
    public void IsIso3Valid_ReturnsFalse_WhenCalled(string code)
    {
        //arrange & act
        var result = RegionHelper.IsIso3Valid(code);

        //assert
        Assert.IsFalse(result);
    }
}

