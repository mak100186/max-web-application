using System.Net;

using AutoFixture;

using FluentAssertions;

using Kindred.Rewards.Core.ExceptionHandling;
using Kindred.Rewards.Core.Exceptions;
using Kindred.Rewards.Rewards.UnitTests.Common;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Shared.Core.ExceptionHandling;

[TestFixture]
public class RewardNotFoundExceptionApiErrorVisitorTests : TestBase
{
    [Test]
    public void ShouldCreateApiErrorInternal()
    {
        var exception = new RewardNotFoundException();

        var target = Fixture.Create<NotFoundExceptionApiErrorVisitor<RewardNotFoundException>>();
        var result = target.CreateResponse(exception);

        result.Should().NotBeNull();
        result.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Errors.Count.Should().Be(1);
        result.Errors.Should().Contain(e => e.Code == "RewardsApi.Not Found");
    }
}
