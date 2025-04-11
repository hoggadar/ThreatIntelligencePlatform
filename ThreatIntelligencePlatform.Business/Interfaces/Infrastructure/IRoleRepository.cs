using Microsoft.AspNetCore.Identity;
using ThreatIntelligencePlatform.Business.DTOs.Pagination;
using ThreatIntelligencePlatform.Business.Entities;

namespace ThreatIntelligencePlatform.Business.Interfaces.Infrastructure;

public interface IRoleRepository : IRepository<RoleEntity>
{
    Task<RoleEntity?> GetRoleByNameAsync(string roleName);
    Task<PaginatedList<RoleEntity>> GetAllRolesAsync(int pageIndex, int pageSize);
    Task<IEnumerable<RoleEntity>> GetUserRolesAsync(Guid userId);
}