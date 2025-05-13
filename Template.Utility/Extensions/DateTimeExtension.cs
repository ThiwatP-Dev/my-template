namespace Template.Utility.Extensions;

public static class DateTimeExtension
{
    public static DateTime ToUtcStartOfDay(this DateTime datetime, TimeZoneInfo timeZone)
    {
        // Ensure input is treated as UTC
        var utcDateTime = DateTime.SpecifyKind(datetime, DateTimeKind.Utc);

        // Convert to target time zone
        var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);

        // Get the date (start of day) in that time zone
        var localStartOfDay = localDateTime.Date;

        // Convert that local start-of-day back to UTC
        var utcStartOfDay = TimeZoneInfo.ConvertTimeToUtc(localStartOfDay, timeZone);

        return utcStartOfDay;
    }

    /// <summary>
    /// Converts a UTC DateTime to the specified time zone.
    /// </summary>
    /// <param name="utcDateTime">The UTC DateTime.</param>
    /// <param name="timeZone">The destination TimeZoneInfo.</param>
    /// <returns>DateTime in the specified time zone.</returns>
    public static DateTime ToTimeZone(this DateTime utcDateTime, TimeZoneInfo timeZone)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc), timeZone);
    }

    /// <summary>
    /// Converts a time-zone-local DateTime to UTC.
    /// </summary>
    /// <param name="dateTime">The local DateTime in a specific time zone.</param>
    /// <param name="timeZone">The source TimeZoneInfo.</param>
    /// <returns>UTC DateTime.</returns>
    public static DateTime FromTimeZoneToUtc(this DateTime dateTime, TimeZoneInfo timeZone)
    {
        return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
    }
}