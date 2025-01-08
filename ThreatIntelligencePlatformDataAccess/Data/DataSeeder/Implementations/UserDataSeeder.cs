using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ThreatIntelligencePlatform.Configuration.DataSeederSettings;
using ThreatIntelligencePlatform.DataAccess.Data.DataSeeder.Interfaces;
using ThreatIntelligencePlatform.DataAccess.Entities;

namespace ThreatIntelligencePlatform.DataAccess.Data.DataSeeder.Implementations;

public class UserDataSeeder : IUserDataSeeder
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly ILogger<UserDataSeeder> _logger;
    private readonly UserDataSeederSettings _settings;

    public UserDataSeeder(UserManager<UserEntity> userManager, ILogger<UserDataSeeder> logger,
        IOptions<UserDataSeederSettings> options)
    {
        _userManager = userManager;
        _logger = logger;
        _settings = options.Value;
    }
    
    public async Task SeedAdminAsync()
    {
        var adminUser = await _userManager.FindByEmailAsync(_settings.AdminEmail);
        if (adminUser == null)
        {
            adminUser = new UserEntity
            {
                UserName = _settings.AdminEmail,
                Email = _settings.AdminEmail,
                EmailConfirmed = true,
                FirstName = "Andrew",
                LastName = "Ermolenko"
            };
            var result = await _userManager.CreateAsync(adminUser, _settings.AdminPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("Admin user created successfully");
                var roleResult = await _userManager.AddToRoleAsync(adminUser, "Admin");
                if (roleResult.Succeeded)
                {
                    _logger.LogInformation("Admin role assigned to admin user");
                }
                else
                {
                    _logger.LogError("Failed to assign Admin role. Errors: {Errors}", 
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogError("Failed to create admin user. Errors: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            _logger.LogInformation("Admin user already exists");
        }
    }
}