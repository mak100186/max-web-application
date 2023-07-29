using System.Net;

using Kindred.Infrastructure.Hosting.WebApi.ExceptionHandling;
using Kindred.Infrastructure.Hosting.WebApi.Models;
using Kindred.Rewards.Core.Exceptions;

namespace Kindred.Rewards.Core.ExceptionHandling;

/// <inheritdoc />
public class InvalidModelStateExceptionApiErrorVisitor : ExceptionVisitorBase<InvalidModelStateException>
{
    /// <inheritdoc />
    public override ApiError CreateResponse(Exception exception)
    {
        var errorCodes = ((InvalidModelStateException)exception).Errors.Select(x => (Error)new RewardsApiError("InvalidModel", x)).ToList();

        return new() { HttpStatusCode = HttpStatusCode.BadRequest, Errors = errorCodes };
    }
}
