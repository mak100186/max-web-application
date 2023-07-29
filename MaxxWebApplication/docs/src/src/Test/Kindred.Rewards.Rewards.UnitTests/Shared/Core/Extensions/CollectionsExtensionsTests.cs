using Kindred.Rewards.Core.Extensions;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Extensions;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[Category("Unit")]
public class CollectionsExtensionsTests
{
    [Test]
    public void ThrowIfNullOrEmpty_ThrowsArgumentNullException_WhenNull()
    {
        //arrange
        var collection = null as List<string>;

        //act //assert
        Assert.Throws<ArgumentNullException>(() => collection.ThrowIfNullOrEmpty(nameof(collection))
            , "Value cannot be null. (Parameter 'collection')");
    }

    [Test]
    public void ThrowIfNullOrEmpty_ThrowsArgumentNullException_WhenEmpty()
    {
        //arrange
        var collection = new List<string>();

        //act //assert
        Assert.Throws<ArgumentNullException>(() => collection.ThrowIfNullOrEmpty(nameof(collection))
            , "Value cannot be null. (Parameter 'collection')");
    }


    [TestCase("1, 2, 3", 1)]
    public void MinOrDefault_ReturnsMinOrDefaultValue_WhenCalled(string csvElements, int expectedValue)
    {
        //arrange
        var collection = csvElements.Split(",").Select(x => Convert.ToInt32(x)).ToList();

        //act
        var actualResult = collection.MinOrDefault();

        //assert
        Assert.AreEqual(expectedValue, actualResult);
    }

    [TestCase("1", "2", "3", "1,2,3")]
    [TestCase(null, null, null, "")]
    public void ToCsv_ReturnsCommaSeparatedString_WhenCalled(string elem1, string elem2, string elem3, string expectedResult)
    {
        //arrange
        var collection = !string.IsNullOrWhiteSpace(elem1) && !string.IsNullOrWhiteSpace(elem2) && !string.IsNullOrWhiteSpace(elem3)
            ? new List<string> { elem1, elem2, elem3 }
            : (IList<string>)null;

        //act
        var actualResult = collection.ToCsv(",");

        //assert
        Assert.AreEqual(expectedResult, actualResult);
    }

    [TestCase("1,2,3", "1,2,3", true)]
    [TestCase("3,2,1", "1,2,3", true)]
    [TestCase("2,3", "1,2", false)]
    [TestCase(null, "1,2,3", false)]
    [TestCase(null, null, true)]
    public void OrderedSequenceEqual_ReturnsExpectedResult_WhenCalled(string seq1, string seq2, bool expectedResult)
    {
        //arrange
        var seq1Collection = string.IsNullOrWhiteSpace(seq1) ? null : seq1.Split(",").ToList();
        var seq2Collection = string.IsNullOrWhiteSpace(seq2) ? null : seq2.Split(",").ToList();

        //act
        var actualResult = seq1Collection.OrderedSequenceEqual(seq2Collection);

        //assert
        Assert.AreEqual(expectedResult, actualResult);
    }
    [Test]
    public void IsEmptyOrContainsWhiteSpace_ReturnsExpectedResult_WhenListIsNull()
    {
        //arrange
        var tags = null as List<string>;

        //act
        var actualResult = tags.ElementsContainEmptyOrWhiteSpace();

        //assert
        Assert.AreEqual(false, actualResult);
    }

    [Test]
    public void IsEmptyOrContainsWhiteSpace_ReturnsExpectedResult_WhenListIsEmpty()
    {
        //arrange
        var tags = new List<string>();

        //act
        var actualResult = tags.ElementsContainEmptyOrWhiteSpace();

        //assert
        Assert.AreEqual(false, actualResult);
    }

    [TestCase("abc", "def", "ghi", false)]
    [TestCase("a bc", "def", "ghi", true)]
    [TestCase("ab c", "d ef", " ", true)]
    [TestCase("abc", "def", "", true)]
    [TestCase("abc", "def", null, true)]
    [TestCase(" ", null, null, true)]
    [TestCase(null, null, null, true)]
    [TestCase("", " ", "", true)]
    public void IsEmptyOrContainsWhiteSpace_ReturnsExpectedResult_WhenCalled(string one, string two, string three, bool expectedResult)
    {
        //arrange
        var tags = new List<string> { one, two, three };

        //act
        var actualResult = tags.ElementsContainEmptyOrWhiteSpace();

        //assert
        Assert.AreEqual(expectedResult, actualResult);
    }
}
