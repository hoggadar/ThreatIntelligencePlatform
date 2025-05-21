using Moq;
using ThreatIntelligencePlatform.Business.Interfaces;
using ThreatIntelligencePlatform.Business.Services;
using ThreatIntelligencePlatform.Grpc.Clients;
using ThreatIntelligencePlatform.Shared.DTOs;

namespace ThreatIntelligencePlatform.Tests.Services;

public class IoCServiceTests
{
    private readonly Mock<IIoCGrpcClient> _mockGrpcClient;
    private readonly IoCService _service;

    public IoCServiceTests()
    {
        _mockGrpcClient = new Mock<IIoCGrpcClient>();
        _service = new IoCService(_mockGrpcClient.Object);
    }

    [Fact]
    public async Task LoadAsync_ShouldCallGrpcClient()
    {
        // Arrange
        long limit = 10;
        long offset = 0;
        string search = "test";
        var cancellationToken = CancellationToken.None;
        
        var expectedResult = new List<IoCDto>
        {
            new() { Value = "test1", Type = "ip", Source = "source1" },
            new() { Value = "test2", Type = "domain", Source = "source2" }
        };
        
        _mockGrpcClient.Setup(client => client.LoadAsync(limit, offset, search, cancellationToken))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.LoadAsync(limit, offset, search, cancellationToken);

        // Assert
        Assert.Equal(expectedResult, result);
        _mockGrpcClient.Verify(client => client.LoadAsync(limit, offset, search, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task StoreAsync_ShouldCallGrpcClient()
    {
        // Arrange
        var iocs = new List<IoCDto>
        {
            new() { Value = "test1", Type = "ip", Source = "source1" },
            new() { Value = "test2", Type = "domain", Source = "source2" }
        };
        var cancellationToken = CancellationToken.None;
        
        _mockGrpcClient.Setup(client => client.StoreAsync(iocs, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _service.StoreAsync(iocs, cancellationToken);

        // Assert
        _mockGrpcClient.Verify(client => client.StoreAsync(iocs, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CountAsync_ShouldCallGrpcClient()
    {
        // Arrange
        long expectedCount = 42;
        var cancellationToken = CancellationToken.None;
        
        _mockGrpcClient.Setup(client => client.CountAsync(cancellationToken))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _service.CountAsync(cancellationToken);

        // Assert
        Assert.Equal(expectedCount, result);
        _mockGrpcClient.Verify(client => client.CountAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CountByTypeAsync_ShouldCallGrpcClient()
    {
        // Arrange
        var expectedCounts = new Dictionary<string, long>
        {
            { "ip", 10 },
            { "domain", 20 },
            { "url", 30 }
        };
        var cancellationToken = CancellationToken.None;
        
        _mockGrpcClient.Setup(client => client.CountByTypeAsync(cancellationToken))
            .ReturnsAsync(expectedCounts);

        // Act
        var result = await _service.CountByTypeAsync(cancellationToken);

        // Assert
        Assert.Equal(expectedCounts, result);
        _mockGrpcClient.Verify(client => client.CountByTypeAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CountSpecificTypeAsync_ShouldCallGrpcClient()
    {
        // Arrange
        string type = "domain";
        long expectedCount = 42;
        var cancellationToken = CancellationToken.None;
        
        _mockGrpcClient.Setup(client => client.CountSpecificTypeAsync(type, cancellationToken))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _service.CountSpecificTypeAsync(type, cancellationToken);

        // Assert
        Assert.Equal(expectedCount, result);
        _mockGrpcClient.Verify(client => client.CountSpecificTypeAsync(type, cancellationToken), Times.Once);
    }
}