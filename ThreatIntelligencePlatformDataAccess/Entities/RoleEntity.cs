using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatform.DataAccess.Entities;

public class RoleEntity : IdentityRole<Guid>
{
    [Column(TypeName = "varchar(256)")]
    public string? Description { get; set; }
    
    public virtual ICollection<UserRoleEntity> UserRoles { get; set; }
    public virtual ICollection<RoleClaimEntity> RoleClaims { get; set; }
}