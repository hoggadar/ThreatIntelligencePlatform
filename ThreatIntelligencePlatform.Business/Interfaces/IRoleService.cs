using ThreatIntelligencePlatform.Business.DTOs.Pagination;
using ThreatIntelligencePlatform.Business.DTOs.Role;

namespace ThreatIntelligencePlatform.Business.Interfaces;

public interface IRoleService
{
    Task<PaginatedList<RoleDto>> GetAllAsync(int pageIndex = 1, int pageSize = 10);
    Task<RoleDto?> GetByNameAsync(string name);
    Task<IEnumerable<RoleDto>> GetUserRolesAsync(string userId);
}