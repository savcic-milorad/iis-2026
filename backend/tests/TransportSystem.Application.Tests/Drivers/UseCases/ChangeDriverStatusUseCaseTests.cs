using FluentAssertions;
using TransportSystem.Application.Drivers.UseCases;
using TransportSystem.Application.Tests.Helpers;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Drivers.UseCases;

public class ChangeDriverStatusUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ChangeDriverStatusUseCase _useCase;

    public ChangeDriverStatusUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new ChangeDriverStatusUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ChangeToOnLeave_UpdatesStatusSuccessfully()
    {
        // Arrange
        var driver = Driver.Create(
            "John Doe",
            "ABC123",
            "+381641234567",
            DateTime.UtcNow.AddYears(-5),
            DateTime.UtcNow.AddYears(5),
            DriverStatus.Active
        );
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(driver.Id, DriverStatus.OnLeave);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(driver.Id);
        result.Status.Should().Be(DriverStatus.OnLeave);
        result.UpdatedAt.Should().NotBeNull();
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verify in database
        var driverInDb = await _context.Drivers.FindAsync(driver.Id);
        driverInDb!.Status.Should().Be(DriverStatus.OnLeave);
    }

    [Fact]
    public async Task ExecuteAsync_ChangeToSuspended_UpdatesStatusSuccessfully()
    {
        // Arrange
        var driver = Driver.Create(
            "John Doe",
            "ABC123",
            "+381641234567",
            DateTime.UtcNow.AddYears(-5),
            DateTime.UtcNow.AddYears(5),
            DriverStatus.Active
        );
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(driver.Id, DriverStatus.Suspended);

        // Assert
        result.Status.Should().Be(DriverStatus.Suspended);
        result.IsAvailable.Should().BeFalse(); // Suspended drivers are not available
    }

    [Fact]
    public async Task ExecuteAsync_SameStatus_DoesNotUpdateTimestamp()
    {
        // Arrange
        var driver = Driver.Create(
            "John Doe",
            "ABC123",
            "+381641234567",
            DateTime.UtcNow.AddYears(-5),
            DateTime.UtcNow.AddYears(5),
            DriverStatus.Active
        );
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        var originalUpdatedAt = driver.UpdatedAt;

        // Act
        var result = await _useCase.ExecuteAsync(driver.Id, DriverStatus.Active);

        // Assert
        result.Status.Should().Be(DriverStatus.Active);
        result.UpdatedAt.Should().Be(originalUpdatedAt); // Should not have changed
    }

    [Fact]
    public async Task ExecuteAsync_NonExistingId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(nonExistingId, DriverStatus.OnLeave);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Driver with ID '{nonExistingId}' was not found");
    }
}
