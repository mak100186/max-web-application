using AutoFixture;

using FluentAssertions;

using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Core.WebApi.Validation;
using Kindred.Rewards.Rewards.Tests.Common.Extensions;
using Kindred.Rewards.Rewards.UnitTests.Common;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.WebApi.Validation;

public class BaseCancelRequestValidatorTests : TestBase
{
    private CancelRequestValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = Fixture.Create<CancelRequestValidator>();
    }

    [TestCase("", false)]
    [TestCase(null, false)]
    [TestCase("12345678912345678912", false)]
    [TestCase("123456789123456789123", true)]
    public void CancelReasonMaxLengthMustBe20(string cancelReason, bool expectedError)
    {
        // Arrange
        var request = new CancelRequest { Reason = cancelReason };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.ShouldNotBeNull();

        if (expectedError)
        {
            result.Errors.Should().HaveCount(1);
            result.Errors.First().ErrorMessage.Should().Be("'Reason' must be between 0 and 20 characters. You entered 21 characters.");
        }
    }
}
