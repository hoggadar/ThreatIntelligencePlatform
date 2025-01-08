using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatform.DataAccess.Entities;

public class UserClaimEntity : IdentityUserClaim<Guid>
{
    public virtual UserEntity User { get; set; }
}