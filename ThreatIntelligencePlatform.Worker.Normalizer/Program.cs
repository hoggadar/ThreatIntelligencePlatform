using ThreatIntelligencePlatform.Configuration.RabbitMQSettings;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.MessageBroker.Services;
using ThreatIntelligencePlatform.Worker.Normalizer.Services;

namespace ThreatIntelligencePlatform.Worker.Normalizer;

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
                services.AddHostedService<IoCNormalizerWorker>();
            });
        return host;
    }
}