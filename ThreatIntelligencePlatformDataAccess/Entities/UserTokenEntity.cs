using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatformDataAccess.Entities;

public class UserTokenEntity : IdentityUserToken<Guid>
{
    public virtual UserEntity User { get; set; }
}