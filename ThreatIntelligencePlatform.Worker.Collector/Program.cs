using ThreatIntelligencePlatform.Configuration.RabbitMQSettings;
using ThreatIntelligencePlatform.MessageBroker.Initializers;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.MessageBroker.Services;
using ThreatIntelligencePlatform.Worker.Collector.Interfaces;
using ThreatIntelligencePlatform.Worker.Collector.Mappers;
using ThreatIntelligencePlatform.Worker.Collector.Services;

namespace ThreatIntelligencePlatform.Worker.Collector;

public class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = CreateHostBuilder(args).Build();
        using (var scope = host.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<RabbitMQInitializer>();
            initializer.Initialize();
        }
        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                services.Configure<RabbitMQOptions>(configuration.GetSection(RabbitMQOptions.SectionName));
                services.AddSingleton<RabbitMQInitializer>();
                services.AddSingleton<IRabbitMQService, RabbitMQService>();
                services.AddSingleton<IIoCProvider, TweetFeedService>();
                services.AddSingleton<IIoCProvider, ThreatFoxService>();
                services.AddSingleton<IIoCProvider, BlocklistService>();
                services.AddAutoMapper(typeof(TweetFeedMapper));
                services.AddAutoMapper(typeof(ThreatFoxMapper));
                services.AddAutoMapper(typeof(BlocklistMapper));
                services.AddAutoMapper(typeof(FeodoTrackerMapper));
                services.AddHttpClient("TweetFeed", client =>
                {
                    client.BaseAddress = new Uri("https://api.tweetfeed.live/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                });
                services.AddHttpClient("ThreatFox", client =>
                {
                    client.BaseAddress = new Uri("https://threatfox-api.abuse.ch/");
                });
                services.AddHttpClient("Blocklist", client =>
                {
                    client.BaseAddress = new Uri("https://lists.blocklist.de/");
                    client.DefaultRequestHeaders.Add("Accept", "text/plain; charset=UTF-8");
                });
                services.AddHttpClient("FeodoTracker", client =>
                {
                    client.BaseAddress = new Uri("https://feodotracker.abuse.ch/");
                });
                services.AddHostedService<IoCCollectorWorker>();
            });
        return host;
    }
}