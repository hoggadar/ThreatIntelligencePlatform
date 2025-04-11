using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatform.Business.Entities;

public class RoleClaimEntity : IdentityRoleClaim<Guid>
{
    public virtual RoleEntity Role { get; set; }
}