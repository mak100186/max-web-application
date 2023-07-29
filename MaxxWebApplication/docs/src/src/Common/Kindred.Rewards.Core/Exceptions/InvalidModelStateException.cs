using Kindred.Rewards.Core.Extensions;

namespace Kindred.Rewards.Core.Exceptions;

/// <inheritdoc />
public class InvalidModelStateException : Exception
{
    /// <inheritdoc />
    public InvalidModelStateException(IList<string> errors) : base(errors.ToCsv(", "))
    {
        Errors = errors;
    }

    /// <inheritdoc />
    public IList<string> Errors { get; }
}
