using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ThreatIntelligencePlatform.Business.DTOs.Pagination;
using ThreatIntelligencePlatform.Business.Entities;
using ThreatIntelligencePlatform.Business.Interfaces.Infrastructure;
using ThreatIntelligencePlatform.DataAccess.Data;

namespace ThreatIntelligencePlatform.DataAccess.Repositories.Implementations;

public class RoleRepository : Repository<RoleEntity>, IRoleRepository
{
    private readonly RoleManager<RoleEntity> _roleManager;
    private readonly ILogger<RoleRepository> _logger;

    public RoleRepository(AppDbContext context, RoleManager<RoleEntity> roleManager, ILogger<RoleRepository> logger) 
        : base(context)
    {
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PaginatedList<RoleEntity>> GetAllRolesAsync(int pageIndex, int pageSize)
    {
        if (pageIndex < 1)
            throw new ArgumentException("Page index must be greater than 0", nameof(pageIndex));
        
        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        try
        {
            var query = _context.Roles;
            return await PaginatedList<RoleEntity>.CreateAsync(query, pageIndex, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles with pagination. PageIndex: {PageIndex}, PageSize: {PageSize}", 
                pageIndex, pageSize);
            throw;
        }
    }

    public async Task<RoleEntity?> GetRoleByNameAsync(string roleName)
    {
        if (string.IsNullOrEmpty(roleName))
            throw new ArgumentException("Role name cannot be empty", nameof(roleName));

        try
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role by name: {RoleName}", roleName);
            throw;
        }
    }

    public async Task<IEnumerable<RoleEntity>> GetUserRolesAsync(Guid userId)
    {
        try
        {
            return await _context.Roles
                .Include(r => r.UserRoles)
                .Where(r => r.UserRoles.Any(ur => ur.UserId == userId))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user with ID: {Id}", userId);
            throw;
        }
    }
}