using AutoMapper;
using Microsoft.Extensions.Logging;
using ThreatIntelligencePlatform.Business.DTOs.Pagination;
using ThreatIntelligencePlatform.Business.DTOs.Role;
using ThreatIntelligencePlatform.Business.Interfaces;
using ThreatIntelligencePlatform.Business.Interfaces.Infrastructure;
using ThreatIntelligencePlatform.Shared.Utils;

namespace ThreatIntelligencePlatform.Business.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleService> _logger;

    public RoleService(IRoleRepository roleRepository, IUserRepository userRepository, IMapper mapper, ILogger<RoleService> logger)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PaginatedList<RoleDto>> GetAllAsync(int pageIndex = 1, int pageSize = 10)
    {
        if (pageIndex < 1)
            throw new ArgumentException("Page index must be greater than 0", nameof(pageIndex));
        
        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

        try
        {
            var roles = await _roleRepository.GetAllRolesAsync(pageIndex, pageSize);
            var roleDtos = roles.Items.Select(role => _mapper.Map<RoleDto>(role)).ToList();
            return new PaginatedList<RoleDto>(roleDtos, roles.TotalCount, roles.PageIndex, roles.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles with pagination. PageIndex: {PageIndex}, PageSize: {PageSize}", 
                pageIndex, pageSize);
            throw;
        }
    }

    public async Task<RoleDto?> GetByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Role name cannot be empty", nameof(name));

        try
        {
            var role = await _roleRepository.GetRoleByNameAsync(name);
            if (role == null)
                return null;

            return _mapper.Map<RoleDto>(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role by name: {Name}", name);
            throw;
        }
    }

    public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        try
        {
            var guid = GuidParser.Parse(userId);
            var user = await _userRepository.GetByIdAsync(guid);
            if (user == null)
                throw new KeyNotFoundException($"User with ID '{userId}' was not found.");
            
            var userRoles = await _roleRepository.GetUserRolesAsync(user.Id);
            return _mapper.Map<IEnumerable<RoleDto>>(userRoles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user with ID: {UserId}", userId);
            throw;
        }
    }
}