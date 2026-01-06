using FluentAssertions;
using TransportSystem.Application.Stations.DTOs;
using TransportSystem.Application.Stations.UseCases;
using TransportSystem.Application.Tests.Helpers;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Stations.UseCases;

public class UpdateStationUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UpdateStationUseCase _useCase;

    public UpdateStationUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new UpdateStationUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ValidDto_UpdatesStationSuccessfully()
    {
        // Arrange
        var station = Station.Create("Old Name", 45.2671, 19.8335, "Old Address", "Old Description");
        _context.Stations.Add(station);
        await _context.SaveChangesAsync();

        var dto = new UpdateStationDto
        {
            Name = "New Name",
            Latitude = 46.0,
            Longitude = 20.0,
            Address = "New Address",
            Description = "New Description"
        };

        // Act
        var result = await _useCase.ExecuteAsync(station.Id, dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(station.Id);
        result.Name.Should().Be("New Name");
        result.Latitude.Should().Be(46.0);
        result.Longitude.Should().Be(20.0);
        result.Address.Should().Be("New Address");
        result.Description.Should().Be("New Description");
        result.UpdatedAt.Should().NotBeNull();
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verify in database
        var stationInDb = await _context.Stations.FindAsync(station.Id);
        stationInDb.Should().NotBeNull();
        stationInDb!.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task ExecuteAsync_NonExistingId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        var dto = new UpdateStationDto
        {
            Name = "Test Name",
            Latitude = 45.2671,
            Longitude = 19.8335,
            Address = "Test Address"
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(nonExistingId, dto);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Station with ID '{nonExistingId}' was not found");
    }

    [Fact]
    public async Task ExecuteAsync_SoftDeletedStation_ThrowsDomainException()
    {
        // Arrange
        var station = Station.Create("Deleted Station", 45.2671, 19.8335, "Test address");
        station.Delete(); // Soft delete
        _context.Stations.Add(station);
        await _context.SaveChangesAsync();

        var dto = new UpdateStationDto
        {
            Name = "Updated Name",
            Latitude = 45.2671,
            Longitude = 19.8335,
            Address = "Test Address"
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(station.Id, dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*deleted station*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExecuteAsync_EmptyName_ThrowsDomainException(string name)
    {
        // Arrange
        var station = Station.Create("Original Name", 45.2671, 19.8335, "Test address");
        _context.Stations.Add(station);
        await _context.SaveChangesAsync();

        var dto = new UpdateStationDto
        {
            Name = name,
            Latitude = 45.2671,
            Longitude = 19.8335,
            Address = "Test Address"
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(station.Id, dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*name cannot be empty*");
    }

    [Theory]
    [InlineData(-91, 19.8335)]
    [InlineData(91, 19.8335)]
    [InlineData(45.2671, -181)]
    [InlineData(45.2671, 181)]
    public async Task ExecuteAsync_InvalidCoordinates_ThrowsDomainException(double latitude, double longitude)
    {
        // Arrange
        var station = Station.Create("Test Station", 45.2671, 19.8335, "Test address");
        _context.Stations.Add(station);
        await _context.SaveChangesAsync();

        var dto = new UpdateStationDto
        {
            Name = "Test Station",
            Latitude = latitude,
            Longitude = longitude,
            Address = "Test Address"
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(station.Id, dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task ExecuteAsync_TooLongName_ThrowsDomainException()
    {
        // Arrange
        var station = Station.Create("Original Name", 45.2671, 19.8335, "Test address");
        _context.Stations.Add(station);
        await _context.SaveChangesAsync();

        var dto = new UpdateStationDto
        {
            Name = new string('A', 101), // 101 characters
            Latitude = 45.2671,
            Longitude = 19.8335,
            Address = "Test Address"
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(station.Id, dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*name cannot exceed 100 characters*");
    }
}
