namespace ThreatIntelligencePlatform.Business.DTOs.User
{
    public class UserDtoBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
    }
}
