namespace Settlement.Business.Core.Extensions;

public static class DateExtension
{
    public static string ToStandardString(this DateTime value)
    {
        return value.ToString("yyyy-MM-dd hh:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
    }
}