using Serilog;
using Serilog.Events;
using ThreatIntelligencePlatform.Configuration.RabbitMQSettings;
using ThreatIntelligencePlatform.MessageBroker.Initializers;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.MessageBroker.Services;
using ThreatIntelligencePlatform.Worker.IoCCollector.Interfaces;
using ThreatIntelligencePlatform.Worker.IoCCollector.Mappers;
using ThreatIntelligencePlatform.Worker.IoCCollector.Services;

namespace ThreatIntelligencePlatform.Worker.IoCCollector;

public class Program
{
    public static async Task Main(string[] args)
    {
        ConfigureLogging();

        try
        {
            Log.Information("Starting IoCCollectorWorker...");

            IHost host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<RabbitMQInitializer>();
                initializer.Initialize();
            }
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                services.Configure<RabbitMQOptions>(configuration.GetSection(RabbitMQOptions.SectionName));
                services.AddSingleton<RabbitMQInitializer>();
                services.AddSingleton<IRabbitMQService, RabbitMQService>();
                services.AddSingleton<IIoCProvider, TweetFeedService>();
                services.AddSingleton<IIoCProvider, ThreatFoxService>();
                services.AddSingleton<IIoCProvider, BlocklistService>();
                services.AddSingleton<IIoCProvider, FeodoTrackerService>();
                services.AddSingleton<IIoCProvider, EmergingThreatsService>();
                services.AddSingleton<IIoCProvider, FireHolLevelService>();
                services.AddAutoMapper(typeof(TweetFeedMapper));
                services.AddAutoMapper(typeof(ThreatFoxMapper));
                services.AddAutoMapper(typeof(BlocklistMapper));
                services.AddAutoMapper(typeof(FeodoTrackerMapper));
                services.AddAutoMapper(typeof(EmergingThreatsMapper));
                services.AddAutoMapper(typeof(FireHolLevelMapper));
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
                services.AddHttpClient("EmergingThreats", client =>
                {
                    client.BaseAddress = new Uri("https://rules.emergingthreats.net/");
                });
                services.AddHttpClient("FireHolLevel", client =>
                {
                    client.BaseAddress = new Uri("https://raw.githubusercontent.com/");
                });
                services.AddHostedService<IoCCollectorWorker>();
            });
        return host;
    }
    
    private static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/worker.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}