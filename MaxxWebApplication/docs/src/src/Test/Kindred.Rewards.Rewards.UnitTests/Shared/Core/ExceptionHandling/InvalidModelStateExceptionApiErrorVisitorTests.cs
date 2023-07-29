using System.Net;

using AutoFixture;

using FluentAssertions;

using Kindred.Rewards.Core.ExceptionHandling;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Rewards.UnitTests.Common;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.ExceptionHandling;

[TestFixture]
public class InvalidModelStateExceptionApiErrorVisitorTests : TestBase
{
    [Test]
    public void ShouldCreateApiErrorInternal()
    {
        var exception = Fixture.Create<InvalidModelStateException>();

        var target = Fixture.Create<InvalidModelStateExceptionApiErrorVisitor>();
        var result = target.CreateResponse(exception);

        result.Should().NotBeNull();
        result.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Count.Should().Be(exception.Errors.Count);
    }
}
