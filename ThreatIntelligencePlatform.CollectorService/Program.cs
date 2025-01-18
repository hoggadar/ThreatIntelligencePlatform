using ThreatIntelligencePlatform.CollectorService.Services;
using ThreatIntelligencePlatform.Configuration.RabbitMQSettings;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.MessageBroker.Services;

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
                var configuration = hostContext.Configuration;
                services.Configure<RabbitMQOptions>(configuration.GetSection(RabbitMQOptions.SectionName));
                services.AddSingleton<IRabbitMQService, RabbitMQService>();
                services.AddHttpClient("TweetFeed", client =>
                {
                    client.BaseAddress = new Uri("https://api.tweetfeed.live/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
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