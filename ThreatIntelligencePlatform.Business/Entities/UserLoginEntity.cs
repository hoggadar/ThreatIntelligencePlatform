using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatform.Business.Entities;

public class UserLoginEntity : IdentityUserLogin<Guid>
{
    public virtual UserEntity User { get; set; }
}