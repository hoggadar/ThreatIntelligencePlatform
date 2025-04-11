using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatform.Business.Entities;

public class UserRoleEntity : IdentityUserRole<Guid>
{
    public virtual UserEntity User { get; set; }
    public virtual RoleEntity Role { get; set; }
}