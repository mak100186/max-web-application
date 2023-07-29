using FluentAssertions;

using Kindred.Rewards.Core.Models.Rn;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.Models.Rn;

[TestFixture]
public class CouponRnParserTests
{
    [TestCase("123e4567-e89b-12d3-a456-426614174000", 2, "ksp", "coupon", 1, "123e4567-e89b-12d3-a456-426614174000", "ksp:coupon.1:123e4567-e89b-12d3-a456-426614174000", 1)]
    public void CouponRn_ReturnsExpectedObject_WhenValueConstructorIsCalled(string guidValue, int index, string expectedNamespace, string expectedEntityType, int expectedSchemeVersion, string expectedGuidValue, string expectedToString, int expectedSegmentCount)
    {
        //arrange & act
        var actualObject = new CouponRn(Guid.Parse(guidValue));

        //assert
        actualObject.Namespace.Should().Be(expectedNamespace);
        actualObject.EntityType.Should().Be(expectedEntityType);
        actualObject.SchemeVersion.Should().Be(expectedSchemeVersion);
        actualObject.EntityScheme.Should().Be($"{expectedEntityType}.{expectedSchemeVersion}");
        actualObject.GuidValue.Should().Be(expectedGuidValue);
        actualObject.ToString().Should().Be(expectedToString);
        actualObject.EntitySegments.Count.Should().Be(expectedSegmentCount);
    }

    [TestCase("ksp:coupon.1:123e4567-e89b-12d3-a456-426614174000", "ksp", "coupon", 1, "123e4567-e89b-12d3-a456-426614174000", "ksp:coupon.1:123e4567-e89b-12d3-a456-426614174000", 1)]
    public void CouponRn_ReturnsExpectedObject_WhenStringConstructorIsCalled(string testString, string expectedNamespace, string expectedEntityType, int expectedSchemeVersion, string expectedGuidValue, string expectedToString, int expectedSegmentCount)
    {
        //arrange & act
        var actualObject = new CouponRn(testString);

        //assert
        actualObject.Namespace.Should().Be(expectedNamespace);
        actualObject.EntityType.Should().Be(expectedEntityType);
        actualObject.SchemeVersion.Should().Be(expectedSchemeVersion);
        actualObject.EntityScheme.Should().Be($"{expectedEntityType}.{expectedSchemeVersion}");
        actualObject.GuidValue.Should().Be(expectedGuidValue);
        actualObject.ToString().Should().Be(expectedToString);
        actualObject.EntitySegments.Count.Should().Be(expectedSegmentCount);
    }

    [TestCase("kkk:coupon.1", "Rn cannot have 2 or less segments")]
    [TestCase("kkk:coupon.1:123e4567-e89b-12d3-a456-426614174000", "Invalid namespace")]
    [TestCase("ksp:coupon.a:123e4567-e89b-12d3-a456-426614174000", "Invalid scheme version")]
    [TestCase("ksp:coupon.1:123e4567-e89b-12d3-a456-426614174000:1", "Extra or missing values")]
    [TestCase("ksp:coupon.1:123e4567-e89b-12d3-a456", "Invalid UUID")]
    [TestCase("ksp:coupon:123e4567-e89b-12d3-a456-426614174000", "Invalid entity scheme")]
    [TestCase("ksp:coupon.1.1:123e4567-e89b-12d3-a456-426614174000", "Invalid entity scheme")]
    [TestCase("ksp:outcome.1:[football:201711202200:watford_vs_west_ham]:1x2:plain:west_ham_united_fc", "Invalid entity type")]
    public void CouponRn_ThrowsException_WhenCalled(string testString, string expectedErrorMessages)
    {
        //arrange act assert
        Assert.Throws<AggregateException>(() => new CouponRn(testString),
            expectedErrorMessages);
    }
}
