using Kindred.Rewards.Core.Extensions;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Extensions;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[Category("Unit")]
public class ObjectExtensionsTests
{
    [Test]
    public void ThrowIfNull_ThrowsArgumentNullException_WhenNull()
    {
        //arrange
        var collection = null as List<string>;

        //act //assert
        Assert.Throws<ArgumentNullException>(() => collection.ThrowIfNull(nameof(collection))
            , "Value cannot be null. (Parameter 'collection')");
    }

    [Test]
    public void ThrowIfNull_DoesNotThrowsArgumentNullException_WhenNotNull()
    {
        //arrange
        var collection = new List<string>();

        //act //assert
        Assert.DoesNotThrow(() => collection.ThrowIfNull(nameof(collection))
            , "Value cannot be null. (Parameter 'collection')");
    }

}
