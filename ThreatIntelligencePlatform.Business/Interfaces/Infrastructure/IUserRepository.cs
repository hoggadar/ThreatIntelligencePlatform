using Microsoft.AspNetCore.Identity;
using ThreatIntelligencePlatform.Business.DTOs.Pagination;
using ThreatIntelligencePlatform.Business.Entities;

namespace ThreatIntelligencePlatform.Business.Interfaces.Infrastructure;

public interface IUserRepository : IRepository<UserEntity>
{
    Task<UserEntity?> GetByEmailAsync(string email);
    Task<UserEntity?> GetWithRoleByIdAsync(Guid id);
    Task<PaginatedList<UserEntity>> GetAllUsersWithRolesAsync(int pageIndex, int pageSize);
    Task<IList<string>> GetUserRolesAsync(UserEntity user);
    Task<IdentityResult> CreateUserAsync(UserEntity user, string password);
    Task<IdentityResult> UpdateUserAsync(UserEntity user);
    Task<IdentityResult> DeleteUserAsync(UserEntity user);
    Task<IdentityResult> AddToRoleAsync(UserEntity user, string role);
    Task<IdentityResult> RemoveFromRoleAsync(UserEntity user, string role);
}