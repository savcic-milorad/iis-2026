using FluentAssertions;
using TransportSystem.Application.Stations.UseCases;
using TransportSystem.Application.Tests.Helpers;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Stations.UseCases;

public class GetStationUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GetStationUseCase _useCase;

    public GetStationUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new GetStationUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ExistingId_ReturnsStation()
    {
        // Arrange
        var station = Station.Create("Central Station", 45.2671, 19.8335, "Test address", "Test description");
        _context.Stations.Add(station);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(station.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(station.Id);
        result.Name.Should().Be("Central Station");
        result.Latitude.Should().Be(45.2671);
        result.Longitude.Should().Be(19.8335);
        result.Address.Should().Be("Test address");
        result.Description.Should().Be("Test description");
    }

    [Fact]
    public async Task ExecuteAsync_NonExistingId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(nonExistingId);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Station with ID '{nonExistingId}' was not found");
    }

    [Fact]
    public async Task ExecuteAsync_SoftDeletedStation_ThrowsEntityNotFoundException()
    {
        // Arrange
        var station = Station.Create("Deleted Station", 45.2671, 19.8335, "Test address");
        station.Delete(); // Soft delete
        _context.Stations.Add(station);
        await _context.SaveChangesAsync();

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(station.Id);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Station with ID '{station.Id}' was not found");
    }
}
