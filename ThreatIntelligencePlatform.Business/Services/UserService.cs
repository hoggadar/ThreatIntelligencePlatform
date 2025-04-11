using AutoMapper;
using Microsoft.Extensions.Logging;
using ThreatIntelligencePlatform.Business.DTOs.Pagination;
using ThreatIntelligencePlatform.Business.DTOs.User;
using ThreatIntelligencePlatform.Business.Entities;
using ThreatIntelligencePlatform.Business.Interfaces;
using ThreatIntelligencePlatform.Business.Interfaces.Infrastructure;
using ThreatIntelligencePlatform.Shared.Utils;

namespace ThreatIntelligencePlatform.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        
        public UserService(IUserRepository userRepository, IMapper mapper,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PaginatedList<UserDto>> GetAllAsync(int pageIndex = 1, int pageSize = 10)
        {
            if (pageIndex < 1)
                throw new ArgumentException("Page index must be greater than 0", nameof(pageIndex));
            
            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

            try
            {
                var users = await _userRepository.GetAllUsersWithRolesAsync(pageIndex, pageSize);
                var userDtos = users.Items.Select(user => _mapper.Map<UserDto>(user)).ToList();
                return new PaginatedList<UserDto>(userDtos, users.TotalCount, users.PageIndex, users.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users with pagination. PageIndex: {PageIndex}, PageSize: {PageSize}", 
                    pageIndex, pageSize);
                throw;
            }
        }
        
        public async Task<UserDto?> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("User ID cannot be empty", nameof(id));

            try
            {
                var guid = GuidParser.Parse(id);
                var user = await _userRepository.GetWithRoleByIdAsync(guid);
                if (user == null)
                    return null;

                return _mapper.Map<UserDto>(user);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error parsing user ID: {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by ID: {Id}", id);
                throw;
            }
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                    return null;

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by email: {Email}", email);
                throw;
            }
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "User data cannot be null");

            if (string.IsNullOrEmpty(dto.Email))
                throw new ArgumentException("Email cannot be empty", nameof(dto.Email));

            if (string.IsNullOrEmpty(dto.Password))
                throw new ArgumentException("Password cannot be empty", nameof(dto.Password));

            try
            {
                var newUser = _mapper.Map<UserEntity>(dto);
                var result = await _userRepository.CreateUserAsync(newUser, dto.Password);
                await _userRepository.AddToRoleAsync(newUser, "User");
                
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    var message = $"User creation failed: {errors}";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }
                
                var userDto = _mapper.Map<UserDto>(newUser);
                userDto.Roles = (await _userRepository.GetUserRolesAsync(newUser)).ToArray();

                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email: {Email}", dto.Email);
                throw;
            }
        }

        public async Task<UserDto> UpdateAsync(string id, UpdateUserDto dto)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("User ID cannot be empty", nameof(id));

            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "User data cannot be null");

            if (string.IsNullOrEmpty(dto.Email))
                throw new ArgumentException("Email cannot be empty", nameof(dto.Email));

            try
            {
                var guid = GuidParser.Parse(id);
                var user = await _userRepository.GetWithRoleByIdAsync(guid);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID '{id}' was not found.");

                _mapper.Map(dto, user);
                var result = await _userRepository.UpdateUserAsync(user);
                
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    var message = $"User update failed: {errors}";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {Id}", id);
                throw;
            }
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("User ID cannot be empty", nameof(id));

            try
            {
                var guid = GuidParser.Parse(id);
                var user = await _userRepository.GetWithRoleByIdAsync(guid);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID '{id}' was not found.");

                var result = await _userRepository.DeleteUserAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    var message = $"User deletion failed: {errors}";
                    _logger.LogError(message);
                    throw new InvalidOperationException(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {Id}", id);
                throw;
            }
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            try
            {
                var guid = GuidParser.Parse(userId);
                var user = await _userRepository.GetByIdAsync(guid);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID '{userId}' was not found.");

                return await _userRepository.GetUserRolesAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles for user with ID: {Id}", userId);
                throw;
            }
        }

        public async Task AddToRoleAsync(string userId, string role)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (string.IsNullOrEmpty(role))
                throw new ArgumentException("Role cannot be empty", nameof(role));

            try
            {
                var guid = GuidParser.Parse(userId);
                var user = await _userRepository.GetByIdAsync(guid);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID '{userId}' was not found.");

                var result = await _userRepository.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to add role: {errors}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding role {Role} to user with ID: {Id}", role, userId);
                throw;
            }
        }

        public async Task RemoveFromRoleAsync(string userId, string role)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (string.IsNullOrEmpty(role))
                throw new ArgumentException("Role cannot be empty", nameof(role));

            try
            {
                var guid = GuidParser.Parse(userId);
                var user = await _userRepository.GetByIdAsync(guid);
                if (user == null)
                    throw new KeyNotFoundException($"User with ID '{userId}' was not found.");

                var result = await _userRepository.RemoveFromRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to remove role: {errors}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {Role} from user with ID: {Id}", role, userId);
                throw;
            }
        }
    }
}
