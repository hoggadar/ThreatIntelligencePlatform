using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatform.DataAccess.Entities;

public class RoleClaimEntity : IdentityRoleClaim<Guid>
{
    public virtual RoleEntity Role { get; set; }
}