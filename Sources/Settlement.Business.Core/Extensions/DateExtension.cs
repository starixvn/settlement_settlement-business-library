namespace Settlement.Business.Core.Extensions;

public static class DateExtension
{
    public static string ToDateTimeString(this DateTimeOffset value)
    {
        return value.ToString("yyyy-MM-dd hh:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
    }

    public static string ToDateTime2String(this DateTimeOffset value)
    {
        return value.ToString("yyyy-MM-dd hh:mm:ss.fffffff", System.Globalization.CultureInfo.InvariantCulture);
    }
}