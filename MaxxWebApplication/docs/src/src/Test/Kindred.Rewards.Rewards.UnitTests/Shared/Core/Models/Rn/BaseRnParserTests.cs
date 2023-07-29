using Kindred.Rewards.Core.Models.Rn;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Models.Rn;

[TestFixture]
public class BaseRnParserTests
{
    [TestCase("kkk:bet.1", "Rn cannot have 2 or less segments")]
    [TestCase("kkk:bet.1:123e4567-e89b-12d3-a456-426614174000:0", "Invalid namespace")]
    [TestCase("ksp:bet.a:123e4567-e89b-12d3-a456-426614174000:0", "Invalid scheme version")]
    [TestCase("ksp:bet:123e4567-e89b-12d3-a456-426614174000:0", "Invalid entity scheme")]
    [TestCase("ksp:bet.1.1:123e4567-e89b-12d3-a456-426614174000:0", "Invalid entity scheme")]
    public void BaseRn_ThrowsException_WhenCalled(string testString, string expectedErrorMessages)
    {
        //arrange act assert
        Assert.Throws<AggregateException>(() => new BaseRn(testString),
            expectedErrorMessages);
    }
}
