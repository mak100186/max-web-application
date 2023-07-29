using System.Runtime.Serialization;

using Kindred.Infrastructure.Hosting.WebApi.Models;
using Kindred.Rewards.Core.Extensions;

namespace Kindred.Rewards.Core.Exceptions;

[Serializable]
public class InvalidModelException : Exception
{
    public InvalidModelException(List<Error> errors)
    {
        Errors = errors;
    }

    protected InvalidModelException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // The serialization constructor
    }

    public override string Message => Errors.ToCsv("|| ");

    public List<Error> Errors { get; }
}
