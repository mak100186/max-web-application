using System.Net;

using Kindred.Infrastructure.Hosting.WebApi.ExceptionHandling;
using Kindred.Infrastructure.Hosting.WebApi.Models;

namespace Kindred.Rewards.Core.ExceptionHandling;

/// <inheritdoc />
public class NotFoundExceptionApiErrorVisitor<T> : ExceptionVisitorBase<T>
    where T : Exception
{
    /// <inheritdoc />
    public override ApiError CreateResponse(Exception exception)
    {
        var errors = new List<Error> { new RewardsApiError("Not Found", exception.Message) };

        return new() { HttpStatusCode = HttpStatusCode.NotFound, Errors = errors };
    }
}
