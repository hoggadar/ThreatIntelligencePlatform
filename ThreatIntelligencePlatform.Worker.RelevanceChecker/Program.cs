using Serilog;
using StackExchange.Redis;
using ThreatIntelligencePlatform.Configuration.RabbitMQSettings;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.MessageBroker.Services;
using ThreatIntelligencePlatform.Shared.Caching;

namespace ThreatIntelligencePlatform.Worker.RelevanceChecker;

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
                var redisConnectionString = configuration.GetConnectionString("Redis");
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                });
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                {
                    try
                    {
                        return ConnectionMultiplexer.Connect(redisConnectionString);
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex, "Failed to connect to Redis.");
                        throw;
                    }
                });
                services.Configure<RabbitMQOptions>(configuration.GetSection(RabbitMQOptions.SectionName));
                services.AddSingleton<IRabbitMQService, RabbitMQService>();
                services.AddSingleton<IRedisService, RedisService>();
                services.AddHostedService<IoCRelevanceCheckerWorker>();
            });
        return host;
    }
}