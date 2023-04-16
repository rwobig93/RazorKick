namespace Application.Helpers.Runtime;

public static class DateTimeHelpers
{
    public static DateTime ConvertToLocal(this DateTime originalDateTime, TimeZoneInfo timeZone)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(originalDateTime, timeZone);
    }
}