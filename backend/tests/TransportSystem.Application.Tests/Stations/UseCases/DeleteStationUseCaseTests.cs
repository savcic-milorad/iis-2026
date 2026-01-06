using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Stations.UseCases;
using TransportSystem.Application.Tests.Helpers;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Stations.UseCases;

public class DeleteStationUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly DeleteStationUseCase _useCase;

    public DeleteStationUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new DeleteStationUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ExistingStation_SoftDeletesSuccessfully()
    {
        // Arrange
        var station = Station.Create("Test Station", 45.2671, 19.8335, "Test address");
        _context.Stations.Add(station);
        await _context.SaveChangesAsync();

        // Act
        await _useCase.ExecuteAsync(station.Id);

        // Assert
        // Station should still exist in database but marked as deleted
        var stationInDb = await _context.Stations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == station.Id);

        stationInDb.Should().NotBeNull();
        stationInDb!.IsDeleted.Should().BeTrue();
        stationInDb.DeletedAt.Should().NotBeNull();
        stationInDb.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Station should not be found by normal queries (respects soft delete filter)
        var normalQuery = await _context.Stations
            .Where(s => s.Id == station.Id)
            .FirstOrDefaultAsync();
        normalQuery.Should().BeNull();
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
    public async Task ExecuteAsync_AlreadyDeleted_ThrowsDomainException()
    {
        // Arrange
        var station = Station.Create("Deleted Station", 45.2671, 19.8335, "Test address");
        station.Delete(); // Already soft deleted
        _context.Stations.Add(station);
        await _context.SaveChangesAsync();

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(station.Id);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*already deleted*");
    }

    [Fact]
    public async Task ExecuteAsync_MultipleStations_DeletesOnlyTarget()
    {
        // Arrange
        var station1 = Station.Create("Station 1", 45.2671, 19.8335, "Address 1");
        var station2 = Station.Create("Station 2", 46.0, 20.0, "Address 2");
        var station3 = Station.Create("Station 3", 47.0, 21.0, "Address 3");

        _context.Stations.AddRange(station1, station2, station3);
        await _context.SaveChangesAsync();

        // Act
        await _useCase.ExecuteAsync(station2.Id);

        // Assert
        var allStations = await _context.Stations.ToListAsync();
        allStations.Should().HaveCount(2);
        allStations.Should().Contain(s => s.Id == station1.Id);
        allStations.Should().Contain(s => s.Id == station3.Id);
        allStations.Should().NotContain(s => s.Id == station2.Id);

        // Verify station2 is soft deleted, not physically removed
        var deletedStation = await _context.Stations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == station2.Id);
        deletedStation.Should().NotBeNull();
        deletedStation!.IsDeleted.Should().BeTrue();
    }
}
