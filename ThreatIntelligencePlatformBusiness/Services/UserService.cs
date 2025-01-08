using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ThreatIntelligencePlatform.Business.DTOs.User;
using ThreatIntelligencePlatform.Business.Interfaces;
using ThreatIntelligencePlatform.DataAccess.Entities;
using ThreatIntelligencePlatform.DataAccess.Pagination;
using ThreatIntelligencePlatformDataAccess.Repositories.Interfaces;

namespace ThreatIntelligencePlatform.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        
        public UserService(IUserRepository userRepository, UserManager<UserEntity> userManager, IMapper mapper,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedList<UserDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        
        public async Task<UserDto?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var foundUser = await _userManager.FindByEmailAsync(email);
            return _mapper.Map<UserDto>(foundUser);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var newUser = _mapper.Map<UserEntity>(dto);
            var result = await _userManager.CreateAsync(newUser, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                var message = $"User creation failed: {errors}";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
            await _userManager.AddToRoleAsync(newUser, "User");
            return _mapper.Map<UserDto>(newUser);
        }

        public async Task<UserDto> UpdateAsync(UpdateUserDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<string>> GetUserRolesAsync(UserDto dto)
        {
            var foundUser = await _userManager.FindByIdAsync(dto.Id);
            if (foundUser == null)
            {
                throw new KeyNotFoundException($"User with ID '{dto.Id}' was not found.");
            }
            var userRoles = await _userManager.GetRolesAsync(foundUser);
            return userRoles;
        }
    }
}
