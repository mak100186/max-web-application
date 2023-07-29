namespace Kindred.Rewards.Core.Helpers;

public interface ITimeService
{
    DateTime UtcNow { get; }
}

public class TimeService : ITimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
