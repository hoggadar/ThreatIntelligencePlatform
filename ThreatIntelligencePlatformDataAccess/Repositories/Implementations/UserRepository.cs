using Microsoft.EntityFrameworkCore;
using ThreatIntelligencePlatform.DataAccess.Data;
using ThreatIntelligencePlatform.DataAccess.Entities;
using ThreatIntelligencePlatformDataAccess.Repositories.Interfaces;

namespace ThreatIntelligencePlatform.DataAccess.Repositories.Implementations;

public class UserRepository : Repository<UserEntity>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<UserEntity?> GetByEmail(string email)
    {
        var foundUser = await _context.Users.FirstOrDefaultAsync(p => p.Email == email);
        return foundUser;
    }

    public async Task<IEnumerable<UserEntity>> GetAllUsersWithRolesAsync()
    {
        throw new NotImplementedException();
    }
}