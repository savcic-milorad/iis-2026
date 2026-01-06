using FluentAssertions;
using TransportSystem.Application.Drivers.UseCases;
using TransportSystem.Application.Tests.Helpers;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Drivers.UseCases;

public class GetDriverUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GetDriverUseCase _useCase;

    public GetDriverUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new GetDriverUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ExistingId_ReturnsDriver()
    {
        // Arrange
        var driver = Driver.Create(
            "John Doe",
            "ABC123",
            "+381641234567",
            DateTime.UtcNow.AddYears(-5),
            DateTime.UtcNow.AddYears(5),
            DriverStatus.Active,
            null,
            "Test notes"
        );
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        // Act
        var result = await _useCase.ExecuteAsync(driver.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(driver.Id);
        result.FullName.Should().Be("John Doe");
        result.LicenseNumber.Should().Be("ABC123");
        result.PhoneNumber.Should().Be("+381641234567");
        result.Status.Should().Be(DriverStatus.Active);
        result.Notes.Should().Be("Test notes");
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
            .WithMessage($"Driver with ID '{nonExistingId}' was not found");
    }

    [Fact]
    public async Task ExecuteAsync_SoftDeletedDriver_ThrowsEntityNotFoundException()
    {
        // Arrange
        var driver = Driver.Create(
            "Deleted Driver",
            "DEL123",
            "+381641234567",
            DateTime.UtcNow.AddYears(-5),
            DateTime.UtcNow.AddYears(5)
        );
        driver.Delete(); // Soft delete
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(driver.Id);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage($"Driver with ID '{driver.Id}' was not found");
    }
}
