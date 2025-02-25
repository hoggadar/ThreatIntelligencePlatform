namespace ThreatIntelligencePlatform.SharedData.DTOs.TweetFeed;

public class TweetFeedResponseDto
{
    public string Date { get; set; } = null!;
    public string User { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
    public List<string> Tags { get; set; } = null!;
    public string Tweet { get; set; } = null!;
}