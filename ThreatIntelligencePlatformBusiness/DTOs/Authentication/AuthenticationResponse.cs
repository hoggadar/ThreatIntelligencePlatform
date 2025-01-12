using ThreatIntelligencePlatform.Business.DTOs.User;

namespace ThreatIntelligencePlatform.Business.DTOs.Authentication;

public class AuthenticationResponse
{
    public string Token { get; set; } = null!;
    public UserDto User { get; set; } = null!;
}