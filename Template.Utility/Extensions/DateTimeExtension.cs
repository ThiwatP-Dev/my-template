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
}