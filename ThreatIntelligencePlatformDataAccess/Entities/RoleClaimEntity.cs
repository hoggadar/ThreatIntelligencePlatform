using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatformDataAccess.Entities;

public class RoleClaimEntity : IdentityRoleClaim<Guid>
{
    public virtual RoleEntity Role { get; set; }
}