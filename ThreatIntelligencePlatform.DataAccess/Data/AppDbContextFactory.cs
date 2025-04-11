using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ThreatIntelligencePlatform.DataAccess.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        string projectDirectory = Directory.GetCurrentDirectory();
        string solutionDirectory = Directory.GetParent(projectDirectory)?.FullName;
        string apiProjectDirectory = Path.Combine(solutionDirectory, "ThreatIntelligencePlatform.API");
        
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");

        builder.UseNpgsql(connectionString);

        return new AppDbContext(builder.Options);
    }
}