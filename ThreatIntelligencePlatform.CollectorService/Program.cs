using ThreatIntelligencePlatform.CollectorService.Services;

namespace ThreatIntelligencePlatform.CollectorService;

public class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<IndicatorCollectorService>();
                services.AddHttpClient();
            })
            .Build();
        await host.RunAsync();
    }
}