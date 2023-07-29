using Microsoft.EntityFrameworkCore;

namespace Kindred.Rewards.Core.Infrastructure.Data.Exceptions;

/// <summary>
/// RewardClaimedConcurrencyException is fired when same reward is claimed for the same customer 
/// at two or more parallel threads
/// </summary>
public class RewardClaimedConcurrencyException : DbUpdateException
{
}
