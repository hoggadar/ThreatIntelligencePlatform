using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatform.DataAccess.Entities;

public class UserTokenEntity : IdentityUserToken<Guid>
{
    public virtual UserEntity User { get; set; }
}