using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ThreatIntelligencePlatform.Business.DTOs.Pagination;
using ThreatIntelligencePlatform.Business.Entities;
using ThreatIntelligencePlatform.Business.Interfaces.Infrastructure;
using ThreatIntelligencePlatform.DataAccess.Data;

namespace ThreatIntelligencePlatform.DataAccess.Repositories.Implementations;

public class UserRepository : Repository<UserEntity>, IUserRepository
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly RoleManager<RoleEntity> _roleManager;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(AppDbContext context, UserManager<UserEntity> userManager, RoleManager<RoleEntity> roleManager,
        ILogger<UserRepository> logger) 
        : base(context)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        try
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by email: {Email}", email);
            throw;
        }
    }

    public async Task<UserEntity?> GetWithRoleByIdAsync(Guid id)
    {
        try
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by ID: {Id}", id);
            throw;
        }
    }

    public async Task<PaginatedList<UserEntity>> GetAllUsersWithRolesAsync(int pageIndex, int pageSize)
    {
        if (pageIndex < 1)
            throw new ArgumentException("Page index must be greater than 0", nameof(pageIndex));
        
        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        try
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);

            return await PaginatedList<UserEntity>.CreateAsync(query, pageIndex, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users with pagination. PageIndex: {PageIndex}, PageSize: {PageSize}", 
                pageIndex, pageSize);
            throw;
        }
    }

    public async Task<IList<string>> GetUserRolesAsync(UserEntity user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");

        try
        {
            return await _userManager.GetRolesAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user with ID: {Id}", user.Id);
            throw;
        }
    }

    public async Task<IdentityResult> CreateUserAsync(UserEntity user, string password)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");

        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        try
        {
            return await _userManager.CreateAsync(user, password);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email: {Email}", user.Email);
            throw;
        }
    }

    public async Task<IdentityResult> UpdateUserAsync(UserEntity user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");

        try
        {
            return await _userManager.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {Id}", user.Id);
            throw;
        }
    }

    public async Task<IdentityResult> DeleteUserAsync(UserEntity user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");

        try
        {
            return await _userManager.DeleteAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {Id}", user.Id);
            throw;
        }
    }

    public async Task<IdentityResult> AddToRoleAsync(UserEntity user, string role)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");

        if (string.IsNullOrEmpty(role))
            throw new ArgumentException("Role cannot be empty", nameof(role));

        try
        {
            return await _userManager.AddToRoleAsync(user, role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding role {Role} to user with ID: {Id}", role, user.Id);
            throw;
        }
    }

    public async Task<IdentityResult> RemoveFromRoleAsync(UserEntity user, string role)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");

        if (string.IsNullOrEmpty(role))
            throw new ArgumentException("Role cannot be empty", nameof(role));

        try
        {
            return await _userManager.RemoveFromRoleAsync(user, role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {Role} from user with ID: {Id}", role, user.Id);
            throw;
        }
    }
}