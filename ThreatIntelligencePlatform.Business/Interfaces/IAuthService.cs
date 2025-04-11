using ThreatIntelligencePlatform.Business.DTOs.Auth;
using ThreatIntelligencePlatform.Business.DTOs.User;

namespace ThreatIntelligencePlatform.Business.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> SignupAsync(SignupDto dto);
        Task<AuthResponse> LoginAsync(LoginDto dto);
    }
}
