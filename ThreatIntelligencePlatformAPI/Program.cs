using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThreatIntelligencePlatformDataAccess.Data;
using ThreatIntelligencePlatformDataAccess.Entities;

namespace ThreatIntelligencePlatformAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(defaultConnectionString);
        });
        builder.Services.AddIdentity<UserEntity, RoleEntity>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}