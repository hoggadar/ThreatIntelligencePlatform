namespace ThreatIntelligencePlatform.Shared.Utils;

public static class GuidParser
{
    public static Guid Parse(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id), "ID cannot be null or empty");

        if (!Guid.TryParse(id, out var guid))
            throw new ArgumentException("Invalid GUID format", nameof(id));

        return guid;
    }
}