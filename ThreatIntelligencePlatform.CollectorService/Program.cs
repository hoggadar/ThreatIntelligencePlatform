using ThreatIntelligencePlatform.CollectorService.Services;

namespace ThreatIntelligencePlatform.CollectorService;

public class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHttpClient("TweetFeed", client =>
                {
                    client.BaseAddress = new Uri("https://api.tweetfeed.live/");
                });
                services.AddHttpClient("ThreatFox", client =>
                {
                    client.BaseAddress = new Uri("https://threatfox-api.abuse.ch/");
                });
                services.AddSingleton<TweetFeedService>();
                services.AddSingleton<ThreatFoxService>();
                services.AddHostedService<IoCCollectorWorker>();
            });
        return host;
    }
}