using ThreatIntelligencePlatform.Configuration.RabbitMQSettings;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.MessageBroker.Services;
using ThreatIntelligencePlatform.NormalizerService.Services;

namespace ThreatIntelligencePlatform.NormalizerService;

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
                services.Configure<RabbitMQSettings>(configuration.GetSection(RabbitMQSettings.SectionName));
                services.AddScoped<IRabbitMQService, RabbitMQService>();
                services.AddHostedService<IoCNormalizerService>();
            });
        return host;
    }
}