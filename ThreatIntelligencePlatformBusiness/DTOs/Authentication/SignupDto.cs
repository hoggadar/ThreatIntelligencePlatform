namespace ThreatIntelligencePlatform.Business.DTOs.Authentication
{
    public class SignupDto : AuthenticationDtoBase
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
}
