using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ThreatIntelligencePlatform.Business.Entities;
using ThreatIntelligencePlatform.Business.Interfaces;
using ThreatIntelligencePlatform.Business.Interfaces.Infrastructure;
using ThreatIntelligencePlatform.Business.Mappers;
using ThreatIntelligencePlatform.Business.Services;
using ThreatIntelligencePlatform.Configuration.AuthenticationSettings;
using ThreatIntelligencePlatform.Configuration.DataSeederSettings;
using ThreatIntelligencePlatform.DataAccess.Data;
using ThreatIntelligencePlatform.DataAccess.Data.DataSeeder.Implementations;
using ThreatIntelligencePlatform.DataAccess.Data.DataSeeder.Interfaces;
using ThreatIntelligencePlatform.DataAccess.Repositories.Implementations;
using ThreatIntelligencePlatform.Grpc.Clients;

namespace ThreatIntelligencePlatform.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
        
        builder.Services.Configure<UserDataSeederSettings>(
            builder.Configuration.GetSection(UserDataSeederSettings.SectionName));
        builder.Services.Configure<RoleDataSeederSettings>(
            builder.Configuration.GetSection(RoleDataSeederSettings.SectionName));
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

        builder.Services.AddControllers();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", corsPolicyBuilder =>
            {
                corsPolicyBuilder.AllowAnyOrigin()
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
            });
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Threat Intelligence Platform API",
                Version = "v1",
                Description = "API for Threat Intelligence Platform"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(defaultConnectionString);
        });
        
        builder.Services
            .AddIdentity<UserEntity, RoleEntity>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });

        builder.Services.AddSingleton<IIoCGrpcClient>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var grpcServiceUrl = configuration["GrpcService:Url"] ?? "http://ioc-db:8080";
            return new IoCGrpcClient(grpcServiceUrl);
        });
        
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();
        
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<IIoCService, IoCService>();
        
        builder.Services.AddScoped<IRoleDataSeeder, RoleDataSeeder>();
        builder.Services.AddScoped<IUserDataSeeder, UserDataSeeder>();
        
        builder.Services.AddAutoMapper(typeof(UserMapper));
        builder.Services.AddAutoMapper(typeof(RoleMapper));
        
        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        
        app.UseAuthorization();

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var logger = services.GetRequiredService<ILogger<Program>>();
                
                logger.LogInformation("Applying database migrations...");
                context.Database.Migrate();
                logger.LogInformation("Database migrations applied successfully.");
                
                var roleSeeder = services.GetRequiredService<IRoleDataSeeder>();
                await roleSeeder.SeedRolesAsync();
                logger.LogInformation("Completed seeding roles.");
                var userSeeder = services.GetRequiredService<IUserDataSeeder>();
                await userSeeder.SeedAdminAsync();
                logger.LogInformation("Completed seeding admin user.");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
        
        await app.RunAsync();
    }
}