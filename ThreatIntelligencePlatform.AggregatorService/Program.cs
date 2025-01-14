using ThreatIntelligencePlatform.AggregatorService.Services;

namespace ThreatIntelligencePlatform.AggregatorService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<IndicatorAggregatorService>();

        var host = builder.Build();
        host.Run();
    }
}