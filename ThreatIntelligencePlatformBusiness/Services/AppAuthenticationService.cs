using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ThreatIntelligencePlatform.Business.DTOs.Authentication;
using ThreatIntelligencePlatform.Business.DTOs.User;
using ThreatIntelligencePlatform.Business.Interfaces;
using ThreatIntelligencePlatform.DataAccess.Entities;
using ThreatIntelligencePlatformDataAccess.Repositories.Interfaces;

namespace ThreatIntelligencePlatform.Business.Services
{
    public class AppAuthenticationService : IAppAuthenticationService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<AppAuthenticationService> _logger;
        
        public AppAuthenticationService(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager,
            IUserService userService, IJwtService jwtService, IMapper mapper, ILogger<AppAuthenticationService> logger)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<AuthenticationResponse> Signup(SignupDto dto)
        {
            var createUserDto = _mapper.Map<CreateUserDto>(dto);
            var createdUser = await _userService.CreateAsync(createUserDto);
            var userRoles = await _userService.GetUserRolesAsync(createdUser);
            var token = _jwtService.GenerateToken(createdUser, userRoles);
            return new AuthenticationResponse
            {
                Token = token,
                User = createdUser
            };
        }

        public async Task<AuthenticationResponse> Login(LoginDto dto)
        {
            var userEntity = await _userManager.FindByEmailAsync(dto.Email);
            if (userEntity == null)
            {
                _logger.LogWarning("Login attempt failed: User with email {Email} not found", dto.Email);
                throw new KeyNotFoundException($"User with Email '{dto.Email}' was not found.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(userEntity, dto.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login attempt failed: Invalid password for user {Email}", dto.Email);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var userDto = _mapper.Map<UserDto>(userEntity);
            var userRoles = await _userManager.GetRolesAsync(userEntity);
            var token = _jwtService.GenerateToken(userDto, userRoles);

            _logger.LogInformation("User {Email} successfully logged in", dto.Email);

            return new AuthenticationResponse
            {
                Token = token,
                User = userDto
            };
        }
    }
}
