using ThreatIntelligencePlatform.Business.DTOs.User;

namespace ThreatIntelligencePlatform.Business.Interfaces;

public interface IJwtService
{
    string GenerateToken(UserDto user, IList<string> roles);
}