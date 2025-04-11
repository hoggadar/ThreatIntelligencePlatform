using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ThreatIntelligencePlatform.Business.DTOs.Auth;
using ThreatIntelligencePlatform.Business.DTOs.User;
using ThreatIntelligencePlatform.Business.Entities;
using ThreatIntelligencePlatform.Business.Interfaces;

namespace ThreatIntelligencePlatform.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<UserEntity> userManager, IUserService userService, IJwtService jwtService,
            IMapper mapper, ILogger<AuthService> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<AuthResponse> SignupAsync(SignupDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Signup data cannot be null");

            if (string.IsNullOrEmpty(dto.Email))
                throw new ArgumentException("Email cannot be empty", nameof(dto.Email));

            if (string.IsNullOrEmpty(dto.Password))
                throw new ArgumentException("Password cannot be empty", nameof(dto.Password));

            try
            {
                var userEntity = await _userService.GetByEmailAsync(dto.Email);
                if (userEntity != null)
                {
                    _logger.LogWarning("Signup attempt failed: User with email {Email} already exists", dto.Email);
                    throw new InvalidOperationException($"User with email '{dto.Email}' already exists");
                }

                var createUserDto = _mapper.Map<CreateUserDto>(dto);
                var createdUserDto = await _userService.CreateAsync(createUserDto);
                var userRoles = await _userService.GetUserRolesAsync(createdUserDto.Id);
                var token = _jwtService.GenerateToken(createdUserDto, userRoles);
                
                _logger.LogInformation("User {Email} successfully signed up", dto.Email);
                
                return new AuthResponse
                {
                    Token = token,
                    User = createdUserDto
                };
            }
            catch (Exception ex) when (ex is not InvalidOperationException and not ArgumentException and not ArgumentNullException)
            {
                _logger.LogError(ex, "Error during signup for user with email: {Email}", dto.Email);
                throw new InvalidOperationException("An error occurred while signing up");
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Login data cannot be null");

            if (string.IsNullOrEmpty(dto.Email))
                throw new ArgumentException("Email cannot be empty", nameof(dto.Email));

            if (string.IsNullOrEmpty(dto.Password))
                throw new ArgumentException("Password cannot be empty", nameof(dto.Password));

            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login attempt failed: User with email {Email} not found", dto.Email);
                    throw new KeyNotFoundException($"User with email '{dto.Email}' was not found");
                }

                var result = await _userManager.CheckPasswordAsync(user, dto.Password);
                if (!result)
                {
                    _logger.LogWarning("Login attempt failed: Invalid password for user {Email}", dto.Email);
                    throw new UnauthorizedAccessException("Invalid password");
                }

                var userDto = _mapper.Map<UserDto>(user);
                var userRoles = await _userService.GetUserRolesAsync(userDto.Id);
                userDto.Roles = userRoles.ToArray();
                var token = _jwtService.GenerateToken(userDto, userRoles);
                
                _logger.LogInformation("User {Email} successfully logged in", dto.Email);
                
                return new AuthResponse
                {
                    Token = token,
                    User = userDto
                };
            }
            catch (Exception ex) when (ex is not KeyNotFoundException and not UnauthorizedAccessException and not ArgumentException and not ArgumentNullException)
            {
                _logger.LogError(ex, "Error during login for user with email: {Email}", dto.Email);
                throw new InvalidOperationException("An error occurred while logging in");
            }
        }
    }
}
