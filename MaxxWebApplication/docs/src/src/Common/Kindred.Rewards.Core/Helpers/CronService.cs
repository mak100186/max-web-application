using Kindred.Rewards.Core.Exceptions;

using Quartz;

namespace Kindred.Rewards.Core.Helpers;

public static class CronService
{
    public static bool IsValidCron(string cronExpression)
    {
        return string.IsNullOrWhiteSpace(cronExpression) || CronExpression.IsValidExpression(cronExpression);
    }

    public static long GetNextIntervalInAllowedPeriod(string intervalCronExpression, string allowedPeriodCronExpression,
                                                      string timezoneId, DateTime fromTime, DateTime? startTime = null,
                                                      DateTime? expiryTime = null)
    {
        startTime ??= DateTime.MinValue;
        expiryTime ??= DateTime.MaxValue;

        if (fromTime >= expiryTime)
        {
            return DateTime.MinValue.Ticks;
        }

        fromTime = (startTime.Value > fromTime) ? startTime.Value : fromTime;

        if (string.IsNullOrEmpty(allowedPeriodCronExpression))
        {
            return CoerceWithExpiry(GetNextInterval(intervalCronExpression, fromTime), expiryTime.Value);
        }

        if (string.IsNullOrEmpty(intervalCronExpression))
        {
            return CoerceWithExpiry(GetFirstTickOfNextInterval(DomainConstants.InfiniteCronInterval, fromTime), expiryTime.Value);
        }

        if (!CronExpression.IsValidExpression(allowedPeriodCronExpression))
        {
            throw new RewardsValidationException($"Cron expression '{allowedPeriodCronExpression}' is not valid");
        }

        if (!IsValidCron(intervalCronExpression))
        {
            throw new RewardsValidationException($"Cron expression '{intervalCronExpression}' is not valid");
        }


        //first find the next interval. Irrespective of the AllowedPeriod, rewards claims are always 
        //reset at the start of the nextInterval, even if the nextInterval falls in non-AllowedPeriod.
        //For intervalCronExpression, we dont use timezoneId.
        var nextIntervalLong = GetFirstTickOfNextInterval(intervalCronExpression, fromTime);

        //now just check if nextInterval is already within allowedPeriod.
        //If not then we need next tick of the allowedTimePeriod.
        DateTime nextInterval = new(nextIntervalLong, DateTimeKind.Utc);
        if (!IsSatisfiedBy(allowedPeriodCronExpression, timezoneId, nextInterval))
        {
            nextIntervalLong = GetFirstTickOfNextInterval(allowedPeriodCronExpression, nextInterval, timezoneId);
        }

        return CoerceWithExpiry(nextIntervalLong, expiryTime.Value);
    }

    private static long CoerceWithExpiry(long timeToCheckLong, DateTime expTime)
    {
        var timeToCheck = new DateTime(timeToCheckLong, DateTimeKind.Utc);

        return timeToCheck >= expTime ? DateTime.MinValue.Ticks : timeToCheck.Ticks;
    }

    public static long GetNextInterval(string cronExpression, DateTime? fromTime = null)
    {
        fromTime ??= DateTime.UtcNow;

        if (string.IsNullOrEmpty(cronExpression))
        {
            return GetFirstTickOfNextInterval(DomainConstants.InfiniteCronInterval, fromTime.Value);
        }

        return !IsValidCron(cronExpression)
            ? throw new RewardsValidationException($"Cron expression '{cronExpression}' is not valid")
            : GetFirstTickOfNextInterval(cronExpression, fromTime.Value);
    }

    public static bool IsSatisfiedBy(string cronExpression, string timezoneId, DateTime? fromTime = null)
    {
        if (string.IsNullOrWhiteSpace(cronExpression))
        {
            return true;
        }

        if (!IsValidCron(cronExpression))
        {
            throw new RewardsValidationException($"Cron expression '{cronExpression}' is not valid");
        }

        var timezone = GetTimeZone(timezoneId);

        fromTime ??= DateTime.UtcNow;
        var now = TimeZoneInfo.ConvertTimeFromUtc(fromTime.Value.ToUniversalTime(), timezone);
        var claimIntervalDateTime = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, timezone.GetUtcOffset(now));
        var allowedPeriod = new CronExpression(cronExpression) { TimeZone = timezone };

        return allowedPeriod.IsSatisfiedBy(claimIntervalDateTime);
    }

    private static long GetFirstTickOfNextInterval(string cronExpression, DateTime fromTime, string timezoneId = null)
    {
        var timezone = GetTimeZone(timezoneId);
        var expression = new CronExpression(cronExpression) { TimeZone = timezone };

        var nextInterval = expression
            .GetNextValidTimeAfter(fromTime)
            .GetValueOrDefault()
            .UtcTicks;

        return nextInterval;
    }

    private static TimeZoneInfo GetTimeZone(string timezoneId)
    {
        try
        {
            return string.IsNullOrWhiteSpace(timezoneId)
                       ? TimeZoneInfo.Utc
                       : TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        }
        catch (Exception)
        {
            return TimeZoneInfo.Utc;
        }
    }
}
