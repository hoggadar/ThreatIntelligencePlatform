using Microsoft.Extensions.Logging;
using ThreatIntelligencePlatform.Configuration.RabbitMQSettings;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.MessageBroker.Services;

namespace ThreatIntelligencePlatform.ManualTester;

class Program
{
    static async Task Main(string[] args)
    {
        var rabbitOptions = new RabbitMQOptions
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Information);
        });

        var rabbitMQService = new RabbitMQService(
            Microsoft.Extensions.Options.Options.Create(rabbitOptions),
            loggerFactory.CreateLogger<RabbitMQService>());

        var tester = new ManualTester(
            rabbitMQService,
            loggerFactory.CreateLogger<ManualTester>());

        try
        {
            await tester.PublishTestDataAsync();
            Console.WriteLine("Test data published successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing test data: {ex.Message}");
        }
    }
}