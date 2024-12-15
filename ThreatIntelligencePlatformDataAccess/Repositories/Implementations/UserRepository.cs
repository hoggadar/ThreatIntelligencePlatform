using ThreatIntelligencePlatformDataAccess.Data;
using ThreatIntelligencePlatformDataAccess.Entities;
using ThreatIntelligencePlatformDataAccess.Repositories.Interfaces;

namespace ThreatIntelligencePlatformDataAccess.Repositories.Implementations;

public class UserRepository : Repository<UserEntity>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
}