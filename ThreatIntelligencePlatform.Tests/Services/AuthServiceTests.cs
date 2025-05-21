using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using ThreatIntelligencePlatform.Business.DTOs.Auth;
using ThreatIntelligencePlatform.Business.DTOs.User;
using ThreatIntelligencePlatform.Business.Entities;
using ThreatIntelligencePlatform.Business.Interfaces;
using ThreatIntelligencePlatform.Business.Mappers;
using ThreatIntelligencePlatform.Business.Services;

namespace ThreatIntelligencePlatform.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<UserEntity>> _mockUserManager;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly IMapper _mapper;
    private readonly AuthService _service;
    private readonly UserEntity _testUser;

    public AuthServiceTests()
    {
        var userStore = new Mock<IUserStore<UserEntity>>();
        _mockUserManager = new Mock<UserManager<UserEntity>>(
            userStore.Object, null, null, null, null, null, null, null, null);
            
        _mockUserService = new Mock<IUserService>();
        _mockJwtService = new Mock<IJwtService>();

        var mapperConfig = new MapperConfiguration(config =>
        {
            config.AddProfile(new UserMapper());
        });
        _mapper = mapperConfig.CreateMapper();

        _service = new AuthService(
            _mockUserManager.Object,
            _mockUserService.Object,
            _mockJwtService.Object,
            _mapper,
            new Mock<ILogger<AuthService>>().Object);

        _testUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User"
        };
    }

    [Fact]
    public async Task SignupAsync_WithValidData_ShouldCreateUserAndReturnToken()
    {
        // Arrange
        var signupDto = new SignupDto
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            FirstName = "New",
            LastName = "User"
        };

        var createdUserDto = new UserDto
        {
            Id = Guid.NewGuid().ToString(),
            Email = signupDto.Email,
            FirstName = signupDto.FirstName,
            LastName = signupDto.LastName
        };

        var userRoles = new List<string> { "User" };
        var token = "test-token";

        _mockUserService.Setup(s => s.GetByEmailAsync(signupDto.Email))
            .ReturnsAsync((UserDto)null);

        _mockUserService.Setup(s => s.CreateAsync(It.IsAny<CreateUserDto>()))
            .ReturnsAsync(createdUserDto);

        _mockUserService.Setup(s => s.GetUserRolesAsync(createdUserDto.Id))
            .ReturnsAsync(userRoles);

        _mockJwtService.Setup(s => s.GenerateToken(It.IsAny<UserDto>(), It.IsAny<IList<string>>()))
            .Returns(token);

        // Act
        var result = await _service.SignupAsync(signupDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(token, result.Token);
        Assert.Equal(createdUserDto.Email, result.User.Email);
        _mockUserService.Verify(s => s.GetByEmailAsync(signupDto.Email), Times.Once);
        _mockUserService.Verify(s => s.CreateAsync(It.IsAny<CreateUserDto>()), Times.Once);
        _mockUserService.Verify(s => s.GetUserRolesAsync(createdUserDto.Id), Times.Once);
        _mockJwtService.Verify(s => s.GenerateToken(It.IsAny<UserDto>(), It.IsAny<IList<string>>()), Times.Once);
    }

    [Fact]
    public async Task SignupAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var signupDto = new SignupDto
        {
            Email = "existing@example.com",
            Password = "Password123!",
            FirstName = "New",
            LastName = "User"
        };

        var existingUser = new UserDto
        {
            Id = Guid.NewGuid().ToString(),
            Email = signupDto.Email
        };

        _mockUserService.Setup(s => s.GetByEmailAsync(signupDto.Email))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.SignupAsync(signupDto));
            
        _mockUserService.Verify(s => s.GetByEmailAsync(signupDto.Email), Times.Once);
        _mockUserService.Verify(s => s.CreateAsync(It.IsAny<CreateUserDto>()), Times.Never);
        _mockUserService.Verify(s => s.GetUserRolesAsync(It.IsAny<string>()), Times.Never);
        _mockJwtService.Verify(s => s.GenerateToken(It.IsAny<UserDto>(), It.IsAny<IList<string>>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = _testUser.Email,
            Password = "Password123!"
        };

        var userDto = new UserDto
        {
            Id = _testUser.Id.ToString(),
            Email = _testUser.Email,
            FirstName = _testUser.FirstName,
            LastName = _testUser.LastName
        };

        var userRoles = new List<string> { "User" };
        var token = "test-token";

        _mockUserManager.Setup(m => m.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(_testUser);

        _mockUserManager.Setup(m => m.CheckPasswordAsync(_testUser, loginDto.Password))
            .ReturnsAsync(true);

        _mockUserService.Setup(s => s.GetUserRolesAsync(userDto.Id))
            .ReturnsAsync(userRoles);

        _mockJwtService.Setup(s => s.GenerateToken(It.IsAny<UserDto>(), It.IsAny<IList<string>>()))
            .Returns(token);

        // Act
        var result = await _service.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(token, result.Token);
        Assert.Equal(userDto.Email, result.User.Email);
        _mockUserManager.Verify(m => m.FindByEmailAsync(loginDto.Email), Times.Once);
        _mockUserManager.Verify(m => m.CheckPasswordAsync(_testUser, loginDto.Password), Times.Once);
        _mockUserService.Verify(s => s.GetUserRolesAsync(userDto.Id), Times.Once);
        _mockJwtService.Verify(s => s.GenerateToken(It.IsAny<UserDto>(), It.IsAny<IList<string>>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithNonExistingUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexisting@example.com",
            Password = "Password123!"
        };

        _mockUserManager.Setup(m => m.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync((UserEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.LoginAsync(loginDto));
            
        _mockUserManager.Verify(m => m.FindByEmailAsync(loginDto.Email), Times.Once);
        _mockUserManager.Verify(m => m.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
        _mockUserService.Verify(s => s.GetUserRolesAsync(It.IsAny<string>()), Times.Never);
        _mockJwtService.Verify(s => s.GenerateToken(It.IsAny<UserDto>(), It.IsAny<IList<string>>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = _testUser.Email,
            Password = "WrongPassword123!"
        };

        _mockUserManager.Setup(m => m.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(_testUser);

        _mockUserManager.Setup(m => m.CheckPasswordAsync(_testUser, loginDto.Password))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _service.LoginAsync(loginDto));
            
        _mockUserManager.Verify(m => m.FindByEmailAsync(loginDto.Email), Times.Once);
        _mockUserManager.Verify(m => m.CheckPasswordAsync(_testUser, loginDto.Password), Times.Once);
        _mockUserService.Verify(s => s.GetUserRolesAsync(It.IsAny<string>()), Times.Never);
        _mockJwtService.Verify(s => s.GenerateToken(It.IsAny<UserDto>(), It.IsAny<IList<string>>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithEmptyEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "",
            Password = "Password123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LoginAsync(loginDto));
            
        _mockUserManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Never);
        _mockUserManager.Verify(m => m.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
        _mockUserService.Verify(s => s.GetUserRolesAsync(It.IsAny<string>()), Times.Never);
        _mockJwtService.Verify(s => s.GenerateToken(It.IsAny<UserDto>(), It.IsAny<IList<string>>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithEmptyPassword_ShouldThrowArgumentException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = _testUser.Email,
            Password = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LoginAsync(loginDto));
            
        _mockUserManager.Verify(m => m.FindByEmailAsync(It.IsAny<string>()), Times.Never);
        _mockUserManager.Verify(m => m.CheckPasswordAsync(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
        _mockUserService.Verify(s => s.GetUserRolesAsync(It.IsAny<string>()), Times.Never);
        _mockJwtService.Verify(s => s.GenerateToken(It.IsAny<UserDto>(), It.IsAny<IList<string>>()), Times.Never);
    }
}