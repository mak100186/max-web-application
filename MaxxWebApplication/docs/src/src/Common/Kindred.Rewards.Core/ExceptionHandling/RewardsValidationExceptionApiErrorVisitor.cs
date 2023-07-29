using System.Net;

using Kindred.Infrastructure.Hosting.WebApi.ExceptionHandling;
using Kindred.Infrastructure.Hosting.WebApi.Models;

namespace Kindred.Rewards.Core.ExceptionHandling;

/// <inheritdoc />
public class RewardsValidationExceptionApiErrorVisitor<T> : ExceptionVisitorBase<T>
    where T : Exception
{
    /// <inheritdoc />
    public override ApiError CreateResponse(Exception exception)
    {
        var errors = new List<Error>
            {
                new RewardsApiError("Validation Error", exception.Message)
            };

        return new() { HttpStatusCode = HttpStatusCode.BadRequest, Errors = errors };
    }
}
