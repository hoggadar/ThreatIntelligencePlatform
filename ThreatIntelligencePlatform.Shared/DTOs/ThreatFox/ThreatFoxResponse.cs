namespace ThreatIntelligencePlatform.Shared.DTOs.ThreatFox;

public class ThreatFoxResponse
{
    public string QueryStatus { get; set; } = null!;
    public IEnumerable<ThreatFoxData> Data { get; set; } = null!;
}