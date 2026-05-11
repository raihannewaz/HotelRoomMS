
namespace Common.Core.DateTimeConversions;

public static class DateTimeConversion
{
    public static DateTime UTCToBST()
    {
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("BST");

        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
    }


    public static DateTime UTCToBST(DateTime date)
    {
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("BST");

        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
    }
}
