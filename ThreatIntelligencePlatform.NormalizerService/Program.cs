using ThreatIntelligencePlatform.NormalizerService.Services;

namespace ThreatIntelligencePlatform.NormalizerService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<IndicatorNormalizerService>();

        var host = builder.Build();
        host.Run();
    }
}