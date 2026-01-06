using FluentAssertions;
using TransportSystem.Application.Stations.UseCases;
using TransportSystem.Application.Tests.Helpers;
using TransportSystem.Domain.Entities;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Stations.UseCases;

public class GetAllStationsUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GetAllStationsUseCase _useCase;

    public GetAllStationsUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new GetAllStationsUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_NoFilters_ReturnsAllActiveStations()
    {
        // Arrange
        var station1 = Station.Create("Alpha Station", 45.2671, 19.8335, "Address 1");
        var station2 = Station.Create("Beta Station", 46.0, 20.0, "Address 2");
        var station3 = Station.Create("Gamma Station", 47.0, 21.0, "Address 3");

        _context.Stations.AddRange(station1, station2, station3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(s => s.Name == "Alpha Station");
        result.Should().Contain(s => s.Name == "Beta Station");
        result.Should().Contain(s => s.Name == "Gamma Station");
    }

    [Fact]
    public async Task ExecuteAsync_NoFilters_ExcludesSoftDeletedStations()
    {
        // Arrange
        var station1 = Station.Create("Active Station", 45.2671, 19.8335, "Address 1");
        var station2 = Station.Create("Deleted Station", 46.0, 20.0, "Address 2");
        station2.Delete(); // Soft delete

        _context.Stations.AddRange(station1, station2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(s => s.Name == "Active Station");
        result.Should().NotContain(s => s.Name == "Deleted Station");
    }

    [Fact]
    public async Task ExecuteAsync_IncludeDeleted_ReturnsAllStations()
    {
        // Arrange
        var station1 = Station.Create("Active Station", 45.2671, 19.8335, "Address 1");
        var station2 = Station.Create("Deleted Station", 46.0, 20.0, "Address 2");
        station2.Delete(); // Soft delete

        _context.Stations.AddRange(station1, station2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(includeDeleted: true);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(s => s.Name == "Active Station");
        result.Should().Contain(s => s.Name == "Deleted Station");
        result.First(s => s.Name == "Deleted Station").IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_SearchByName_ReturnsMatchingStations()
    {
        // Arrange
        var station1 = Station.Create("Central Station", 45.2671, 19.8335, "Address 1");
        var station2 = Station.Create("North Station", 46.0, 20.0, "Address 2");
        var station3 = Station.Create("South Station", 47.0, 21.0, "Address 3");

        _context.Stations.AddRange(station1, station2, station3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(searchTerm: "north");

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(s => s.Name == "North Station");
    }

    [Fact]
    public async Task ExecuteAsync_SearchByAddress_ReturnsMatchingStations()
    {
        // Arrange
        var station1 = Station.Create("Station 1", 45.2671, 19.8335, "Bulevar oslobođenja 46");
        var station2 = Station.Create("Station 2", 46.0, 20.0, "Trg slobode 5");
        var station3 = Station.Create("Station 3", 47.0, 21.0, "Bulevar Evrope 12");

        _context.Stations.AddRange(station1, station2, station3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(searchTerm: "bulevar");

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(s => s.Name == "Station 1");
        result.Should().Contain(s => s.Name == "Station 3");
    }

    [Fact]
    public async Task ExecuteAsync_SearchByDescription_ReturnsMatchingStations()
    {
        // Arrange
        var station1 = Station.Create("Station 1", 45.2671, 19.8335, "Address 1", "Main city center station");
        var station2 = Station.Create("Station 2", 46.0, 20.0, "Address 2", "Suburban station");
        var station3 = Station.Create("Station 3", 47.0, 21.0, "Address 3", "Airport station");

        _context.Stations.AddRange(station1, station2, station3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(searchTerm: "airport");

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(s => s.Name == "Station 3");
    }

    [Fact]
    public async Task ExecuteAsync_CaseInsensitiveSearch_ReturnsMatchingStations()
    {
        // Arrange
        var station1 = Station.Create("CENTRAL Station", 45.2671, 19.8335, "Address 1");
        var station2 = Station.Create("central PARK", 46.0, 20.0, "Address 2");

        _context.Stations.AddRange(station1, station2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(searchTerm: "CeNtRaL");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_OrdersByName_ReturnsAlphabeticallyOrdered()
    {
        // Arrange
        var station1 = Station.Create("Zebra Station", 45.2671, 19.8335, "Address 1");
        var station2 = Station.Create("Alpha Station", 46.0, 20.0, "Address 2");
        var station3 = Station.Create("Beta Station", 47.0, 21.0, "Address 3");

        _context.Stations.AddRange(station1, station2, station3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Alpha Station");
        result[1].Name.Should().Be("Beta Station");
        result[2].Name.Should().Be("Zebra Station");
    }

    [Fact]
    public async Task ExecuteAsync_SearchNoMatches_ReturnsEmptyList()
    {
        // Arrange
        var station1 = Station.Create("Alpha Station", 45.2671, 19.8335, "Address 1");
        var station2 = Station.Create("Beta Station", 46.0, 20.0, "Address 2");

        _context.Stations.AddRange(station1, station2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(searchTerm: "NonExistent");

        // Assert
        result.Should().BeEmpty();
    }
}
