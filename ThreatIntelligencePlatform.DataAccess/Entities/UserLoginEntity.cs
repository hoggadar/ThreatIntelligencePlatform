using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatform.DataAccess.Entities;

public class UserLoginEntity : IdentityUserLogin<Guid>
{
    public virtual UserEntity User { get; set; }
}