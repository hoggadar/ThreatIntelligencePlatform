using Serilog;
using Serilog.Events;
using ThreatIntelligencePlatform.Worker.WhitelistCollector.Interfaces;
using ThreatIntelligencePlatform.Worker.WhitelistCollector.Services;

namespace ThreatIntelligencePlatform.Worker.WhitelistCollector;

public class Program
{
    public static async Task Main(string[] args)
    {
        ConfigureLogging();
        
        try
        {
            Log.Information("Starting WhitelistCollectorWorker...");

            IHost host = CreateHostBuilder(args).Build();
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
                services.AddSingleton<IWhitelistProvider, MajesticService>();
                services.AddHttpClient("MajesticMillion", client =>
                {
                    client.BaseAddress = new Uri("https://downloads.majestic.com/");
                });
                services.AddHostedService<WhitelistCollectorWorker>();
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