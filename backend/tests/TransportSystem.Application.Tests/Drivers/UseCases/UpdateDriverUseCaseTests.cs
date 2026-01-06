using FluentAssertions;
using TransportSystem.Application.Drivers.DTOs;
using TransportSystem.Application.Drivers.UseCases;
using TransportSystem.Application.Tests.Helpers;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Drivers.UseCases;

public class UpdateDriverUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UpdateDriverUseCase _useCase;

    public UpdateDriverUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new UpdateDriverUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ValidDto_UpdatesDriverSuccessfully()
    {
        // Arrange
        var driver = Driver.Create(
            "Old Name",
            "ABC123",
            "+381641111111",
            DateTime.UtcNow.AddYears(-5),
            DateTime.UtcNow.AddYears(5)
        );
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        var dto = new UpdateDriverDto
        {
            FullName = "New Name",
            PhoneNumber = "+381642222222",
            LicenseIssuedDate = DateTime.UtcNow.AddYears(-3),
            LicenseExpiryDate = DateTime.UtcNow.AddYears(7),
            Notes = "Updated notes"
        };

        // Act
        var result = await _useCase.ExecuteAsync(driver.Id, dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(driver.Id);
        result.FullName.Should().Be("New Name");
        result.PhoneNumber.Should().Be("+381642222222");
        result.Notes.Should().Be("Updated notes");
        result.LicenseNumber.Should().Be("ABC123"); // Should not change
        result.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ExecuteAsync_NonExistingId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        var dto = new UpdateDriverDto
        {
            FullName = "Test Name",
            PhoneNumber = "+381641234567",
            LicenseIssuedDate = DateTime.UtcNow.AddYears(-5),
            LicenseExpiryDate = DateTime.UtcNow.AddYears(5)
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(nonExistingId, dto);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task ExecuteAsync_SoftDeletedDriver_ThrowsDomainException()
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

        var dto = new UpdateDriverDto
        {
            FullName = "Updated Name",
            PhoneNumber = "+381641234567",
            LicenseIssuedDate = DateTime.UtcNow.AddYears(-5),
            LicenseExpiryDate = DateTime.UtcNow.AddYears(5)
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(driver.Id, dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*deleted driver*");
    }
}
