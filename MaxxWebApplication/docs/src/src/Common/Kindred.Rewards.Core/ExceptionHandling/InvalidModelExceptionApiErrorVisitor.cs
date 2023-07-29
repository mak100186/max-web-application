using System.Net;

using Kindred.Infrastructure.Hosting.WebApi.ExceptionHandling;
using Kindred.Infrastructure.Hosting.WebApi.Models;
using Kindred.Rewards.Core.Exceptions;

namespace Kindred.Rewards.Core.ExceptionHandling;

public class InvalidModelExceptionApiErrorVisitor : ExceptionVisitorBase<InvalidModelException>
{
    /// <inheritdoc />
    public override ApiError CreateResponse(Exception exception)
    {
        return new() { HttpStatusCode = HttpStatusCode.BadRequest, Errors = ((InvalidModelException)exception).Errors };
    }
}
