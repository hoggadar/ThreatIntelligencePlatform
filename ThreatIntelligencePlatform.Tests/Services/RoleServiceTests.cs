using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using ThreatIntelligencePlatform.Business.DTOs.Pagination;
using ThreatIntelligencePlatform.Business.Entities;
using ThreatIntelligencePlatform.Business.Interfaces.Infrastructure;
using ThreatIntelligencePlatform.Business.Mappers;
using ThreatIntelligencePlatform.Business.Services;

namespace ThreatIntelligencePlatform.Tests.Services;

public class RoleServiceTests
{
    private readonly Mock<IRoleRepository> _mockRoleRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly IMapper _mapper;
    private readonly RoleService _service;
    private readonly List<RoleEntity> _roles;

    public RoleServiceTests()
    {
        _mockRoleRepo = new Mock<IRoleRepository>();
        _mockUserRepo = new Mock<IUserRepository>();

        var mapperConfig = new MapperConfiguration(config =>
        {
            config.AddProfile(new RoleMapper());
        });
        _mapper = mapperConfig.CreateMapper();

        _service = new RoleService(_mockRoleRepo.Object, _mockUserRepo.Object, _mapper, new Mock<ILogger<RoleService>>().Object);
        
        _roles = new List<RoleEntity>
        {
            new() { Id = Guid.NewGuid(), Name = "Admin", Description = "Administrator role" },
            new() { Id = Guid.NewGuid(), Name = "User", Description = "Regular user role" },
            new() { Id = Guid.NewGuid(), Name = "Manager", Description = "Manager role" }
        };
    }

    [Fact]
    public async Task GetAllAsync_WithValidPagination_ShouldReturnPaginatedRoles()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var paginatedRoles = new PaginatedList<RoleEntity>(_roles, _roles.Count, pageIndex, pageSize);
        
        _mockRoleRepo.Setup(repo => repo.GetAllRolesAsync(pageIndex, pageSize))
            .ReturnsAsync(paginatedRoles);

        // Act
        var result = await _service.GetAllAsync(pageIndex, pageSize);

        // Assert
        Assert.Equal(_roles.Count, result.Items.Count());
        Assert.Equal(pageIndex, result.PageIndex);
        Assert.Equal(pageSize, result.PageSize);
        _mockRoleRepo.Verify(repo => repo.GetAllRolesAsync(pageIndex, pageSize), Times.Once);
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
            
        _mockRoleRepo.Verify(repo => repo.GetAllRolesAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
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
            
        _mockRoleRepo.Verify(repo => repo.GetAllRolesAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetByNameAsync_WithExistingRole_ShouldReturnRole()
    {
        // Arrange
        var roleName = "Admin";
        var role = _roles.First(r => r.Name == roleName);
        
        _mockRoleRepo.Setup(repo => repo.GetRoleByNameAsync(roleName))
            .ReturnsAsync(role);

        // Act
        var result = await _service.GetByNameAsync(roleName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(roleName, result.Name);
        _mockRoleRepo.Verify(repo => repo.GetRoleByNameAsync(roleName), Times.Once);
    }

    [Fact]
    public async Task GetByNameAsync_WithNonExistingRole_ShouldReturnNull()
    {
        // Arrange
        var nonExistingRoleName = "NonExistingRole";
        
        _mockRoleRepo.Setup(repo => repo.GetRoleByNameAsync(nonExistingRoleName))
            .ReturnsAsync((RoleEntity)null);

        // Act
        var result = await _service.GetByNameAsync(nonExistingRoleName);

        // Assert
        Assert.Null(result);
        _mockRoleRepo.Verify(repo => repo.GetRoleByNameAsync(nonExistingRoleName), Times.Once);
    }

    [Fact]
    public async Task GetByNameAsync_WithEmptyRoleName_ShouldThrowArgumentException()
    {
        // Arrange
        var emptyRoleName = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetByNameAsync(emptyRoleName));
            
        _mockRoleRepo.Verify(repo => repo.GetRoleByNameAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetUserRolesAsync_WithExistingUser_ShouldReturnUserRoles()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var user = new UserEntity { Id = Guid.Parse(userId) };
        var userRoles = _roles.Take(2).ToList();
        
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);
            
        _mockRoleRepo.Setup(repo => repo.GetUserRolesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(userRoles);

        // Act
        var result = await _service.GetUserRolesAsync(userId);

        // Assert
        Assert.Equal(userRoles.Count, result.Count());
        _mockUserRepo.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockRoleRepo.Verify(repo => repo.GetUserRolesAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetUserRolesAsync_WithNonExistingUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((UserEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.GetUserRolesAsync(userId));
            
        _mockUserRepo.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        _mockRoleRepo.Verify(repo => repo.GetUserRolesAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetUserRolesAsync_WithInvalidUserId_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidUserId = "not-a-guid";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUserRolesAsync(invalidUserId));
            
        _mockUserRepo.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _mockRoleRepo.Verify(repo => repo.GetUserRolesAsync(It.IsAny<Guid>()), Times.Never);
    }
}