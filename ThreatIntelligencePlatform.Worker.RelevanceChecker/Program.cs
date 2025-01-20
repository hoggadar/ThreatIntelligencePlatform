using ThreatIntelligencePlatform.Worker.RelevanceChecker.Service;

namespace ThreatIntelligencePlatform.Worker.RelevanceChecker;

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