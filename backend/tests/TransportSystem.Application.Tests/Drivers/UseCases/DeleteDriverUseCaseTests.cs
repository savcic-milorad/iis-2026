using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Drivers.UseCases;
using TransportSystem.Application.Tests.Helpers;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Drivers.UseCases;

public class DeleteDriverUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly DeleteDriverUseCase _useCase;

    public DeleteDriverUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new DeleteDriverUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ExistingDriver_SoftDeletesSuccessfully()
    {
        // Arrange
        var driver = Driver.Create(
            "John Doe",
            "ABC123",
            "+381641234567",
            DateTime.UtcNow.AddYears(-5),
            DateTime.UtcNow.AddYears(5)
        );
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        // Act
        await _useCase.ExecuteAsync(driver.Id);

        // Assert
        var driverInDb = await _context.Drivers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == driver.Id);

        driverInDb.Should().NotBeNull();
        driverInDb!.IsDeleted.Should().BeTrue();
        driverInDb.DeletedAt.Should().NotBeNull();

        // Should not be found by normal queries
        var normalQuery = await _context.Drivers
            .Where(d => d.Id == driver.Id)
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
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task ExecuteAsync_AlreadyDeleted_ThrowsDomainException()
    {
        // Arrange
        var driver = Driver.Create(
            "Deleted Driver",
            "DEL123",
            "+381641234567",
            DateTime.UtcNow.AddYears(-5),
            DateTime.UtcNow.AddYears(5)
        );
        driver.Delete();
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(driver.Id);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*already deleted*");
    }
}
