using ThreatIntelligencePlatform.RelevanceCheckerService.Service;

namespace ThreatIntelligencePlatform.RelevanceCheckerService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<IndicatorRelevanceCheckerService>();

        var host = builder.Build();
        host.Run();
    }
}