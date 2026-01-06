using FluentAssertions;
using TransportSystem.Application.Stations.DTOs;
using TransportSystem.Application.Stations.UseCases;
using TransportSystem.Application.Tests.Helpers;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Stations.UseCases;

public class CreateStationUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CreateStationUseCase _useCase;

    public CreateStationUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new CreateStationUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ValidDto_CreatesStationSuccessfully()
    {
        // Arrange
        var dto = new CreateStationDto
        {
            Name = "Central Station",
            Latitude = 45.2671,
            Longitude = 19.8335,
            Address = "Bulevar oslobođenja 46, Novi Sad",
            Description = "Main station in city center"
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Central Station");
        result.Latitude.Should().Be(45.2671);
        result.Longitude.Should().Be(19.8335);
        result.Address.Should().Be("Bulevar oslobođenja 46, Novi Sad");
        result.Description.Should().Be("Main station in city center");
        result.IsDeleted.Should().BeFalse();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verify in database
        var stationInDb = await _context.Stations.FindAsync(result.Id);
        stationInDb.Should().NotBeNull();
        stationInDb!.Name.Should().Be("Central Station");
    }

    [Fact]
    public async Task ExecuteAsync_DuplicateName_ThrowsDomainException()
    {
        // Arrange
        var existingStation = Station.Create("Central Station", 45.2671, 19.8335, "Existing address");
        _context.Stations.Add(existingStation);
        await _context.SaveChangesAsync();

        var dto = new CreateStationDto
        {
            Name = "Central Station",
            Latitude = 45.2671,
            Longitude = 19.8335,
            Address = "New address"
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*Central Station*already exists*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExecuteAsync_EmptyName_ThrowsDomainException(string name)
    {
        // Arrange
        var dto = new CreateStationDto
        {
            Name = name,
            Latitude = 45.2671,
            Longitude = 19.8335,
            Address = "Test address"
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*name cannot be empty*");
    }

    [Theory]
    [InlineData(-91, 19.8335)] // Latitude too low
    [InlineData(91, 19.8335)]  // Latitude too high
    [InlineData(45.2671, -181)] // Longitude too low
    [InlineData(45.2671, 181)]  // Longitude too high
    public async Task ExecuteAsync_InvalidCoordinates_ThrowsDomainException(double latitude, double longitude)
    {
        // Arrange
        var dto = new CreateStationDto
        {
            Name = "Test Station",
            Latitude = latitude,
            Longitude = longitude,
            Address = "Test address"
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task ExecuteAsync_TooLongName_ThrowsDomainException()
    {
        // Arrange
        var dto = new CreateStationDto
        {
            Name = new string('A', 101), // 101 characters
            Latitude = 45.2671,
            Longitude = 19.8335,
            Address = "Test address"
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*name cannot exceed 100 characters*");
    }
}
