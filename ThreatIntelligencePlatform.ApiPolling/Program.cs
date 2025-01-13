namespace ThreatIntelligencePlatform.ApiPolling;

public class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>();
                services.AddHttpClient();
            })
            .Build();
        await host.RunAsync();
    }
}