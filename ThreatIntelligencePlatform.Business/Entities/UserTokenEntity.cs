using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatform.Business.Entities;

public class UserTokenEntity : IdentityUserToken<Guid>
{
    public virtual UserEntity User { get; set; }
}