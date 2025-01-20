using ThreatIntelligencePlatform.DataAccess.Entities;
using ThreatIntelligencePlatform.DataAccess.Repositories.Interfaces;

namespace ThreatIntelligencePlatformDataAccess.Repositories.Interfaces;

public interface IUserRepository : IRepository<UserEntity>
{
    Task<UserEntity?> GetByEmail(string email);
    Task<IEnumerable<UserEntity>> GetAllUsersWithRolesAsync();
}