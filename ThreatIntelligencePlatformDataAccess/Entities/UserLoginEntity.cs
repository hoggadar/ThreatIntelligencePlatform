using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatformDataAccess.Entities;

public class UserLoginEntity : IdentityUserLogin<Guid>
{
    public virtual UserEntity User { get; set; }
}