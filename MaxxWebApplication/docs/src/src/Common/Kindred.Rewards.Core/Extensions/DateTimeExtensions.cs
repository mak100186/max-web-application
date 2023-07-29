namespace Kindred.Rewards.Core.Extensions;

public static class DateTimeExtensions
{
    public static bool NotInTheFuture(this DateTime source)
    {
        return source <= DateTime.UtcNow;
    }

    public static DateTime? ToNullableDateTime(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DateTime.Parse(value);
    }

    public static DateTime ApplyUtc(this DateTime dateTime)
    {
        return new(dateTime.Ticks, DateTimeKind.Utc);
    }
}
