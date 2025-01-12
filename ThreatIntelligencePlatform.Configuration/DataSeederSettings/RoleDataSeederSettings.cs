namespace ThreatIntelligencePlatform.Configuration.DataSeederSettings;

public class RoleDataSeederSettings
{
    public const string SectionName = "RoleDataSeeder";
    public string[] RolesToSeed { get; set; } = ["Admin", "User"];
}