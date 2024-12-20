using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatformDataAccess.Entities;

public class UserClaimEntity : IdentityUserClaim<Guid>
{
    public virtual UserEntity User { get; set; }
}