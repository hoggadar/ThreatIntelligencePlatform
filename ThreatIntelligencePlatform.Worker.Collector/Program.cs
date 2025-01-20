using ThreatIntelligencePlatform.Configuration.RabbitMQSettings;
using ThreatIntelligencePlatform.MessageBroker.Initializers;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.MessageBroker.Services;
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