using System.Net;

using AutoFixture;

using FluentAssertions;

using Kindred.Rewards.Core.ExceptionHandling;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Rewards.UnitTests.Common;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.ExceptionHandling;

[TestFixture]
public class RewardsValidationExceptionApiErrorVisitorTests : TestBase
{
    [Test]
    public void ShouldCreateApiErrorInternal()
    {
        var exception = new RewardsValidationException("Error Message");

        var target = Fixture.Create<RewardsValidationExceptionApiErrorVisitor<RewardsValidationException>>();
        var result = target.CreateResponse(exception);

        result.Should().NotBeNull();
        result.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Count.Should().Be(1);
        result.Errors.Should().Contain(e => e.Code == "RewardsApi.Validation Error" && e.Description == "Error Message");
    }
}
