using Kindred.Rewards.Core.Extensions;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Extensions;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[Category("Unit")]
public class DictionaryExtensionsTests
{
    private readonly IDictionary<string, string> _subject = new Dictionary<string, string>
    {
        {"int", "1"},
        {"double", "1.002"},
        {"float", "1.01"},
        {"decimal", "1.003230"},
        {"string", "value"},
        {"char", "c"}
    };

    [Test]
    public void GetValue_ConvertsValueToInt_WhenCalled()
    {
        //arrange & act
        var result = _subject.GetValue<int>("int");

        //assert
        Assert.AreEqual(result, 1);
    }

    [Test]
    public void GetValue_ConvertsValueToDouble_WhenCalled()
    {
        //arrange & act
        var result = _subject.GetValue<double>("double");

        //assert
        Assert.AreEqual(result, 1.002d);
    }

    [Test]
    public void GetValue_ConvertsValueToFloat_WhenCalled()
    {
        //arrange & act
        var result = _subject.GetValue<float>("float");

        //assert
        Assert.AreEqual(result, 1.01f);
    }

    [Test]
    public void GetValue_ConvertsValueToDecimal_WhenCalled()
    {
        //arrange & act
        var result = _subject.GetValue<decimal>("decimal");

        //assert
        Assert.AreEqual(result, 1.003230m);
    }

    [Test]
    public void GetValue_ConvertsValueToString_WhenCalled()
    {
        //arrange & act
        var result = _subject.GetValue<string>("string");

        //assert
        Assert.AreEqual(result, "value");
    }

    [Test]
    public void GetValue_ConvertsValueToChar_WhenCalled()
    {
        //arrange & act
        var result = _subject.GetValue<char>("char");

        //assert
        Assert.AreEqual(result, 'c');
    }

    [Test]
    public void TryGetValue_ReturnsDefaultIntValue_WhenCalled()
    {
        //arrange & act
        var result = _subject.TryGetValue<int>("unknownKey", out var value);

        //assert
        Assert.IsFalse(result);
        Assert.AreEqual(value, default(int));
    }

    [Test]
    public void TryGetValue_ReturnsDefaultFloatValue_WhenCalled()
    {
        //arrange & act
        var result = _subject.TryGetValue<float>("unknownKey", out var value);

        //assert
        Assert.IsFalse(result);
        Assert.AreEqual(value, default(float));
    }

    [Test]
    public void TryGetValue_ReturnsDefaultDoubleValue_WhenCalled()
    {
        //arrange & act
        var result = _subject.TryGetValue<double>("unknownKey", out var value);

        //assert
        Assert.IsFalse(result);
        Assert.AreEqual(value, default(double));
    }

    [Test]
    public void TryGetValue_ReturnsDefaultDecimalValue_WhenCalled()
    {
        //arrange & act
        var result = _subject.TryGetValue<decimal>("unknownKey", out var value);

        //assert
        Assert.IsFalse(result);
        Assert.AreEqual(value, default(decimal));
    }

    [Test]
    public void TryGetValue_ReturnsDefaultStringValue_WhenCalled()
    {
        //arrange & act
        var result = _subject.TryGetValue<string>("unknownKey", out var value);

        //assert
        Assert.IsFalse(result);
        Assert.AreEqual(value, default(string));
    }

    [Test]
    public void TryGetValue_ReturnsDefaultCharValue_WhenCalled()
    {
        //arrange & act
        var result = _subject.TryGetValue<char>("unknownKey", out var value);

        //assert
        Assert.IsFalse(result);
        Assert.AreEqual(value, default(char));
    }
}
