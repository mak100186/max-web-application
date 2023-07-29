namespace Kindred.Rewards.Core.Exceptions;

public class RewardsValidationException : Exception
{
    public RewardsValidationException(string message) : base(message)
    {
    }

    public RewardsValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
