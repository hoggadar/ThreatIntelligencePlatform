using ThreatIntelligencePlatform.Business.DTOs.Authentication;
using ThreatIntelligencePlatform.Business.DTOs.User;

namespace ThreatIntelligencePlatform.Business.Interfaces
{
    public interface IAppAuthenticationService
    {
        Task<AuthenticationResponse> Signup(SignupDto dto);
        Task<AuthenticationResponse> Login(LoginDto dto);
    }
}
