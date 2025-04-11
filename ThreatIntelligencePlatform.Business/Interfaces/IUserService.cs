using ThreatIntelligencePlatform.Business.DTOs.Pagination;
using ThreatIntelligencePlatform.Business.DTOs.User;

namespace ThreatIntelligencePlatform.Business.Interfaces
{
    public interface IUserService
    {
        Task<PaginatedList<UserDto>> GetAllAsync(int pageIndex = 1, int pageSize = 10);
        Task<UserDto?> GetByIdAsync(string id);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto> UpdateAsync(string id, UpdateUserDto dto);
        Task DeleteAsync(string id);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task AddToRoleAsync(string userId, string role);
        Task RemoveFromRoleAsync(string userId, string role);
    }
}
