namespace ThreatIntelligencePlatform.Configuration.RabbitMQSettings;

public class RabbitMQSettings
{
    public const string SectionName = "RabbitMQ";
    public string HostName { get; set; } = null!;
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}