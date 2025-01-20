namespace ThreatIntelligencePlatform.SharedData.Utils;

public class DateTimeParser
{
    public static DateTime? Parse(string? dateTimeString)
    {
        return DateTime.TryParse(dateTimeString, out var dateTime) ? dateTime : null;
    }
}