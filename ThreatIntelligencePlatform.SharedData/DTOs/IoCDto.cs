namespace ThreatIntelligencePlatform.SharedData.DTOs;

public class IoCDto
{
    public string Id { get; set; } = null!;
    public string Source { get; set; } = null!;
    public DateTime? FirstSeen { get; set; }
    public DateTime? LastSeen { get; set; }
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
    public List<string> Tags { get; set; } = null!;
    public Dictionary<string, string> AdditionalData { get; set; } = null!;
}