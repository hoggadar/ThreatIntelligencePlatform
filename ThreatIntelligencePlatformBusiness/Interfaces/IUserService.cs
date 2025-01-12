using ThreatIntelligencePlatform.Business.DTOs.User;
using ThreatIntelligencePlatform.DataAccess.Pagination;

namespace ThreatIntelligencePlatform.Business.Interfaces
{
    public interface IUserService
    {
        Task<PaginatedList<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto> UpdateAsync(UpdateUserDto dto);
        Task<UserDto> DeleteAsync(string id);
        Task<IList<string>> GetUserRolesAsync(UserDto user);
    }
}
