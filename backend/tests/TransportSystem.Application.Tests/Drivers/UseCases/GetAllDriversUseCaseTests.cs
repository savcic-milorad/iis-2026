using FluentAssertions;
using TransportSystem.Application.Drivers.UseCases;
using TransportSystem.Application.Tests.Helpers;
using TransportSystem.Domain.Entities;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Drivers.UseCases;

public class GetAllDriversUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GetAllDriversUseCase _useCase;

    public GetAllDriversUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new GetAllDriversUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_NoFilters_ReturnsAllActiveDrivers()
    {
        // Arrange
        var driver1 = Driver.Create("Alice Smith", "ABC123", "+381641111111",
            DateTime.UtcNow.AddYears(-5), DateTime.UtcNow.AddYears(5));
        var driver2 = Driver.Create("Bob Jones", "DEF456", "+381642222222",
            DateTime.UtcNow.AddYears(-3), DateTime.UtcNow.AddYears(7));

        _context.Drivers.AddRange(driver1, driver2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExecuteAsync_FilterByStatus_ReturnsMatchingDrivers()
    {
        // Arrange
        var activeDriver = Driver.Create("Active Driver", "ABC123", "+381641111111",
            DateTime.UtcNow.AddYears(-5), DateTime.UtcNow.AddYears(5), DriverStatus.Active);
        var onLeaveDriver = Driver.Create("OnLeave Driver", "DEF456", "+381642222222",
            DateTime.UtcNow.AddYears(-3), DateTime.UtcNow.AddYears(7), DriverStatus.OnLeave);

        _context.Drivers.AddRange(activeDriver, onLeaveDriver);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(status: DriverStatus.OnLeave);

        // Assert
        result.Should().HaveCount(1);
        result.First().FullName.Should().Be("OnLeave Driver");
    }

    [Fact]
    public async Task ExecuteAsync_SearchByName_ReturnsMatchingDrivers()
    {
        // Arrange
        var driver1 = Driver.Create("Alice Smith", "ABC123", "+381641111111",
            DateTime.UtcNow.AddYears(-5), DateTime.UtcNow.AddYears(5));
        var driver2 = Driver.Create("Bob Jones", "DEF456", "+381642222222",
            DateTime.UtcNow.AddYears(-3), DateTime.UtcNow.AddYears(7));

        _context.Drivers.AddRange(driver1, driver2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(searchTerm: "alice");

        // Assert
        result.Should().HaveCount(1);
        result.First().FullName.Should().Be("Alice Smith");
    }

    [Fact]
    public async Task ExecuteAsync_ExcludesSoftDeleted_ByDefault()
    {
        // Arrange
        var activeDriver = Driver.Create("Active Driver", "ABC123", "+381641111111",
            DateTime.UtcNow.AddYears(-5), DateTime.UtcNow.AddYears(5));
        var deletedDriver = Driver.Create("Deleted Driver", "DEF456", "+381642222222",
            DateTime.UtcNow.AddYears(-3), DateTime.UtcNow.AddYears(7));
        deletedDriver.Delete();

        _context.Drivers.AddRange(activeDriver, deletedDriver);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().NotContain(d => d.FullName == "Deleted Driver");
    }

    [Fact]
    public async Task ExecuteAsync_IncludeDeleted_ReturnsAllDrivers()
    {
        // Arrange
        var activeDriver = Driver.Create("Active Driver", "ABC123", "+381641111111",
            DateTime.UtcNow.AddYears(-5), DateTime.UtcNow.AddYears(5));
        var deletedDriver = Driver.Create("Deleted Driver", "DEF456", "+381642222222",
            DateTime.UtcNow.AddYears(-3), DateTime.UtcNow.AddYears(7));
        deletedDriver.Delete();

        _context.Drivers.AddRange(activeDriver, deletedDriver);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(includeDeleted: true);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(d => d.FullName == "Deleted Driver");
    }

    [Fact]
    public async Task ExecuteAsync_OrdersByFullName_ReturnsAlphabetically()
    {
        // Arrange
        var driver1 = Driver.Create("Zebra Driver", "ABC123", "+381641111111",
            DateTime.UtcNow.AddYears(-5), DateTime.UtcNow.AddYears(5));
        var driver2 = Driver.Create("Alpha Driver", "DEF456", "+381642222222",
            DateTime.UtcNow.AddYears(-3), DateTime.UtcNow.AddYears(7));

        _context.Drivers.AddRange(driver1, driver2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result[0].FullName.Should().Be("Alpha Driver");
        result[1].FullName.Should().Be("Zebra Driver");
    }
}
