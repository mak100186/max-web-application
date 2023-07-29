using Kindred.Infrastructure.Hosting.WebApi.Models;

namespace Kindred.Rewards.Core.ExceptionHandling;

/// <inheritdoc />
public class RewardsApiError : Error
{
    /// <inheritdoc />
    public RewardsApiError(string code, string description)
    {
        Code = $"{DomainConstants.RewardsApiErrorCodePrefix}.{code}";
        Description = description;
    }
}
