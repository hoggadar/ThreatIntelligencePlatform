using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using ThreatIntelligencePlatform.Business.DTOs.Pagination;
using ThreatIntelligencePlatform.Business.DTOs.User;
using ThreatIntelligencePlatform.Business.Entities;
using ThreatIntelligencePlatform.Business.Interfaces.Infrastructure;
using ThreatIntelligencePlatform.Business.Mappers;
using ThreatIntelligencePlatform.Business.Services;
using ThreatIntelligencePlatform.Shared.Utils;

namespace ThreatIntelligencePlatform.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly IMapper _mapper;
    private readonly UserService _service;
    private readonly List<UserEntity> _users;

    public UserServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();

        var mapperConfig = new MapperConfiguration(config =>
        {
            config.AddProfile(new UserMapper());
        });
        _mapper = mapperConfig.CreateMapper();

        _service = new UserService(_mockUserRepo.Object, _mapper, new Mock<ILogger<UserService>>().Object);
        
        _users = new List<UserEntity>
        {
            new() 
            { 
                Id = Guid.NewGuid(), 
                Email = "user1@example.com", 
                UserName = "user1",
                FirstName = "John",
                LastName = "Doe"
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                Email = "user2@example.com", 
                UserName = "user2",
                FirstName = "Jane",
                LastName = "Smith"
            }
        };
    }

    [Fact]
    public async Task GetAllAsync_WithValidPagination_ShouldReturnPaginatedUsers()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var paginatedUsers = new PaginatedList<UserEntity>(_users, _users.Count, pageIndex, pageSize);
        
        _mockUserRepo.Setup(repo => repo.GetAllUsersWithRolesAsync(pageIndex, pageSize))
            .ReturnsAsync(paginatedUsers);

        // Act
        var result = await _service.GetAllAsync(pageIndex, pageSize);

        // Assert
        Assert.Equal(_users.Count, result.Items.Count());
        Assert.Equal(pageIndex, result.PageIndex);
        Assert.Equal(pageSize, result.PageSize);
        _mockUserRepo.Verify(repo => repo.GetAllUsersWithRolesAsync(pageIndex, pageSize), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithInvalidPageIndex_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidPageIndex = 0;
        var pageSize = 10;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetAllAsync(invalidPageIndex, pageSize));
            
        _mockUserRepo.Verify(repo => repo.GetAllUsersWithRolesAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_WithInvalidPageSize_ShouldThrowArgumentException()
    {
        // Arrange
        var pageIndex = 1;
        var invalidPageSize = 0;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetAllAsync(pageIndex, invalidPageSize));
            
        _mockUserRepo.Verify(repo => repo.GetAllUsersWithRolesAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingUser_ShouldReturnUser()
    {
        // Arrange
        var userId = _users[0].Id.ToString();
        var user = _users[0];
        
        _mockUserRepo.Setup(repo => repo.GetWithRoleByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
        _mockUserRepo.Verify(repo => repo.GetWithRoleByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingUser_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        
        _mockUserRepo.Setup(repo => repo.GetWithRoleByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserEntity)null);

        // Act
        var result = await _service.GetByIdAsync(userId);

        // Assert
        Assert.Null(result);
        _mockUserRepo.Verify(repo => repo.GetWithRoleByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidUserId_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidUserId = "not-a-guid";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetByIdAsync(invalidUserId));
            
        _mockUserRepo.Verify(repo => repo.GetWithRoleByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetByEmailAsync_WithExistingUser_ShouldReturnUser()
    {
        // Arrange
        var email = _users[0].Email;
        var user = _users[0];
        
        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        _mockUserRepo.Verify(repo => repo.GetByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_WithNonExistingUser_ShouldReturnNull()
    {
        // Arrange
        var email = "nonexisting@example.com";
        
        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(email))
            .ReturnsAsync((UserEntity)null);

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        Assert.Null(result);
        _mockUserRepo.Verify(repo => repo.GetByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_WithEmptyEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var emptyEmail = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetByEmailAsync(emptyEmail));
            
        _mockUserRepo.Verify(repo => repo.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            FirstName = "New",
            LastName = "User"
        };
        
        var newUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = createUserDto.Email,
            UserName = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName
        };
        
        var identityResult = IdentityResult.Success;
        var userRoles = new List<string> { "User" };
        
        _mockUserRepo.Setup(repo => repo.CreateUserAsync(It.IsAny<UserEntity>(), createUserDto.Password))
            .ReturnsAsync(identityResult)
            .Callback<UserEntity, string>((u, p) => 
            {
                newUser.Id = Guid.NewGuid();
                u.Id = newUser.Id;
            });
            
        _mockUserRepo.Setup(repo => repo.AddToRoleAsync(It.IsAny<UserEntity>(), "User"))
            .ReturnsAsync(identityResult);
            
        _mockUserRepo.Setup(repo => repo.GetUserRolesAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync(userRoles);

        // Act
        var result = await _service.CreateAsync(createUserDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createUserDto.Email, result.Email);
        Assert.Equal(userRoles, result.Roles);
        _mockUserRepo.Verify(repo => repo.CreateUserAsync(It.IsAny<UserEntity>(), createUserDto.Password), Times.Once);
        _mockUserRepo.Verify(repo => repo.AddToRoleAsync(It.IsAny<UserEntity>(), "User"), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            FirstName = "New",
            LastName = "User"
        };
        
        var identityErrors = new IdentityError[] { new IdentityError { Description = "Error" } };
        var identityResult = IdentityResult.Failed(identityErrors);
        
        _mockUserRepo.Setup(repo => repo.CreateUserAsync(It.IsAny<UserEntity>(), createUserDto.Password))
            .ReturnsAsync(identityResult);
            
        _mockUserRepo.Setup(repo => repo.AddToRoleAsync(It.IsAny<UserEntity>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.CreateAsync(createUserDto));
            
        Assert.Contains("User creation failed", exception.Message);
        _mockUserRepo.Verify(repo => repo.CreateUserAsync(It.IsAny<UserEntity>(), createUserDto.Password), Times.Once);
        _mockUserRepo.Verify(repo => repo.AddToRoleAsync(It.IsAny<UserEntity>(), "User"), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingUser_ShouldUpdateUser()
    {
        // Arrange
        var userId = _users[0].Id.ToString();
        var updateUserDto = new UpdateUserDto
        {
            Email = "updated@example.com",
            FirstName = "Updated",
            LastName = "User"
        };
        
        var existingUser = _users[0];
        var identityResult = IdentityResult.Success;
        
        _mockUserRepo.Setup(repo => repo.GetWithRoleByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingUser);
            
        _mockUserRepo.Setup(repo => repo.UpdateUserAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync(identityResult);

        // Act
        var result = await _service.UpdateAsync(userId, updateUserDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateUserDto.Email, result.Email);
        _mockUserRepo.Verify(repo => repo.GetWithRoleByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockUserRepo.Verify(repo => repo.UpdateUserAsync(It.IsAny<UserEntity>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var updateUserDto = new UpdateUserDto
        {
            Email = "updated@example.com",
            FirstName = "Updated",
            LastName = "User"
        };
        
        _mockUserRepo.Setup(repo => repo.GetWithRoleByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.UpdateAsync(userId, updateUserDto));
            
        _mockUserRepo.Verify(repo => repo.GetWithRoleByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockUserRepo.Verify(repo => repo.UpdateUserAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingUser_ShouldDeleteUser()
    {
        // Arrange
        var userId = _users[0].Id.ToString();
        var existingUser = _users[0];
        var identityResult = IdentityResult.Success;
        
        _mockUserRepo.Setup(repo => repo.GetWithRoleByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingUser);
            
        _mockUserRepo.Setup(repo => repo.DeleteUserAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync(identityResult);

        // Act
        await _service.DeleteAsync(userId);

        // Assert
        _mockUserRepo.Verify(repo => repo.GetWithRoleByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockUserRepo.Verify(repo => repo.DeleteUserAsync(It.IsAny<UserEntity>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        
        _mockUserRepo.Setup(repo => repo.GetWithRoleByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.DeleteAsync(userId));
            
        _mockUserRepo.Verify(repo => repo.GetWithRoleByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockUserRepo.Verify(repo => repo.DeleteUserAsync(It.IsAny<UserEntity>()), Times.Never);
    }

    [Fact]
    public async Task GetUserRolesAsync_WithExistingUser_ShouldReturnUserRoles()
    {
        // Arrange
        var userId = _users[0].Id.ToString();
        var user = _users[0];
        var roles = new List<string> { "User", "Admin" };
        
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);
            
        _mockUserRepo.Setup(repo => repo.GetUserRolesAsync(It.IsAny<UserEntity>()))
            .ReturnsAsync(roles);

        // Act
        var result = await _service.GetUserRolesAsync(userId);

        // Assert
        Assert.Equal(roles.Count, result.Count);
        _mockUserRepo.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockUserRepo.Verify(repo => repo.GetUserRolesAsync(It.IsAny<UserEntity>()), Times.Once);
    }

    [Fact]
    public async Task AddToRoleAsync_WithExistingUser_ShouldAddRole()
    {
        // Arrange
        var userId = _users[0].Id.ToString();
        var user = _users[0];
        var role = "Admin";
        var identityResult = IdentityResult.Success;
        
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);
            
        _mockUserRepo.Setup(repo => repo.AddToRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(identityResult);

        // Act
        await _service.AddToRoleAsync(userId, role);

        // Assert
        _mockUserRepo.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockUserRepo.Verify(repo => repo.AddToRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task RemoveFromRoleAsync_WithExistingUser_ShouldRemoveRole()
    {
        // Arrange
        var userId = _users[0].Id.ToString();
        var user = _users[0];
        var role = "Admin";
        var identityResult = IdentityResult.Success;
        
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);
            
        _mockUserRepo.Setup(repo => repo.RemoveFromRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(identityResult);

        // Act
        await _service.RemoveFromRoleAsync(userId, role);

        // Assert
        _mockUserRepo.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockUserRepo.Verify(repo => repo.RemoveFromRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Once);
    }
}