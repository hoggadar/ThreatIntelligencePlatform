using Serilog;
using Serilog.Events;
using ThreatIntelligencePlatform.Configuration.RabbitMQSettings;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.MessageBroker.Services;

namespace ThreatIntelligencePlatform.Worker.Normalizer;

public class Program
{
    public static async Task Main(string[] args)
    {
        ConfigureLogging();
        IHost host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                services.Configure<RabbitMQOptions>(configuration.GetSection(RabbitMQOptions.SectionName));
                services.AddSingleton<IRabbitMQService, RabbitMQService>();
                services.AddHostedService<IoCNormalizerWorker>();
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