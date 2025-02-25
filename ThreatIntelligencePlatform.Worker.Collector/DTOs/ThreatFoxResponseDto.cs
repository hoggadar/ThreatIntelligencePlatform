namespace ThreatIntelligencePlatform.Worker.Collector.DTOs;

public class ThreatFoxResponseDto
{
    public string QueryStatus { get; set; } = null!;
    public IEnumerable<ThreatFoxData> Data { get; set; } = null!;
}

public class ThreatFoxData : BaseIoCResponse
{
    public string Id { get; set; } = null!;
    public string ThreatType { get; set; } = null!;
    public string ThreatTypeDesc { get; set; } = null!;
    public string IoCType { get; set; } = null!;
    public int ConfidenceLevel { get; set; }
    public string? FirstSeen { get; set; }
    public string? LastSeen { get; set; }
    public string Reference { get; set; } = null!;
    public string Reporter { get; set; } = null!;
    public List<string> Tags { get; set; } = null!;
}