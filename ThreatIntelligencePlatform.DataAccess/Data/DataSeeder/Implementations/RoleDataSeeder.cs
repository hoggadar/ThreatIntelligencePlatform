using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ThreatIntelligencePlatform.Configuration.DataSeederSettings;
using ThreatIntelligencePlatform.DataAccess.Data.DataSeeder.Interfaces;
using ThreatIntelligencePlatform.DataAccess.Entities;

namespace ThreatIntelligencePlatform.DataAccess.Data.DataSeeder.Implementations;

public class RoleDataSeeder : IRoleDataSeeder
{
    private readonly RoleManager<RoleEntity> _roleManager;
    private readonly ILogger<RoleDataSeeder> _logger;
    private readonly RoleDataSeederSettings _settings;

    public RoleDataSeeder(RoleManager<RoleEntity> roleManager, ILogger<RoleDataSeeder> logger,
        IOptions<RoleDataSeederSettings> options)
    {
        _roleManager = roleManager;
        _logger = logger;
        _settings = options.Value;
    }
    
    public async Task SeedRolesAsync()
    {
        foreach (var roleName in _settings.RolesToSeed)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new RoleEntity
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    Description = $"Default description for {roleName} role"
                };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Created new role: {RoleName}", roleName);
                }
                else
                {
                    _logger.LogError("Failed to create role: {RoleName}. Errors: {Errors}", roleName,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogInformation("Role already exists: {RoleName}", roleName);
            }
        }
    }
}