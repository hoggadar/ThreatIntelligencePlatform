using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ThreatIntelligencePlatform.DataAccess.Entities;

public class UserEntity : IdentityUser<Guid>
{
    [PersonalData]
    [Column(TypeName = "varchar(96)")]
    public string? FirstName { get; set; }
    
    [PersonalData]
    [Column(TypeName = "varchar(96)")]
    public string? LastName { get; set; }
    
    public virtual ICollection<UserClaimEntity> Claims { get; set; }
    public virtual ICollection<UserLoginEntity> Logins { get; set; }
    public virtual ICollection<UserTokenEntity> Tokens { get; set; }
    public virtual ICollection<UserRoleEntity> UserRoles { get; set; }
}