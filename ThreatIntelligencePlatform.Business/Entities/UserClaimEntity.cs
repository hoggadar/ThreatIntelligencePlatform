using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatform.Business.Entities;

public class UserClaimEntity : IdentityUserClaim<Guid>
{
    public virtual UserEntity User { get; set; }
}