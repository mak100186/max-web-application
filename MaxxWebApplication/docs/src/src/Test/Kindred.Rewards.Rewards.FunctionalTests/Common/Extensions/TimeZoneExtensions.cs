namespace Kindred.Rewards.Rewards.FunctionalTests.Common.Extensions;

public static class TimeZoneExtensions
{
    public const string AusEasternStandardTime = "AUS Eastern Standard Time";

    //NOTE: this is defaulted for AEST, use TimeZone.Local.Id for system's local timezone
    public static DateTime GetLocalDaylightTransitionStartDate(int year, string timezoneId = AusEasternStandardTime)
    {
        var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        var adjustmentRules = timezoneInfo.GetAdjustmentRules();

        var startDate = GetTransitionDates(adjustmentRules, year)["start"];

        return startDate;
    }

    private static Dictionary<string, DateTime> GetTransitionDates(TimeZoneInfo.AdjustmentRule[] adjustmentRules, int year)
    {
        var transitionDates = new Dictionary<string, DateTime>();
        var firstOfYear = new DateTime(year, 1, 1);

        //there are two adjustment rules: 1987-2006 and 2007 onward
        foreach (var adjustmentRule in adjustmentRules)
        {
            if (adjustmentRule.DateStart <= firstOfYear && firstOfYear <= adjustmentRule.DateEnd)
            {
                var start = GetTransitionDate(adjustmentRule.DaylightTransitionStart, year);
                var end = GetTransitionDate(adjustmentRule.DaylightTransitionEnd, year);

                transitionDates.Add("start", start);
                transitionDates.Add("end", end);
            }
        }

        return transitionDates;
    }

    private static DateTime GetTransitionDate(TimeZoneInfo.TransitionTime transitionTime, int year)
    {
        if (transitionTime.IsFixedDateRule)
        {
            return new(year, transitionTime.Month, transitionTime.Day, transitionTime.TimeOfDay.Hour, transitionTime.TimeOfDay.Minute, transitionTime.TimeOfDay.Second);
        }

        var transitionDate = new DateTime(year, transitionTime.Month, 1, transitionTime.TimeOfDay.Hour, transitionTime.TimeOfDay.Minute, transitionTime.TimeOfDay.Second);

        // Special value meaning the last DayOfWeek (e.g., Sunday) in the month.
        if (transitionTime.Week == 5)
        {
            transitionDate = transitionDate.AddMonths(1);

            transitionDate = transitionDate.AddDays(-1);
            while (transitionDate.DayOfWeek != transitionTime.DayOfWeek)
            {
                transitionDate = transitionDate.AddDays(-1);
            }

            return transitionDate;
        }

        transitionDate = transitionDate.AddDays(-1);

        for (var howManyWeeks = 0; howManyWeeks < transitionTime.Week; howManyWeeks++)
        {
            transitionDate = transitionDate.AddDays(1);
            while (transitionDate.DayOfWeek != transitionTime.DayOfWeek)
            {
                transitionDate = transitionDate.AddDays(1);
            }
        }

        return transitionDate;
    }
}
