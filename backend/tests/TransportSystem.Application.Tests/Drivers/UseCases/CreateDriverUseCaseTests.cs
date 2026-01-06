using FluentAssertions;
using TransportSystem.Application.Drivers.DTOs;
using TransportSystem.Application.Drivers.UseCases;
using TransportSystem.Application.Tests.Helpers;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Drivers.UseCases;

public class CreateDriverUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CreateDriverUseCase _useCase;

    public CreateDriverUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new CreateDriverUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ValidDto_CreatesDriverSuccessfully()
    {
        // Arrange
        var dto = new CreateDriverDto
        {
            FullName = "John Doe",
            LicenseNumber = "ABC123",
            PhoneNumber = "+381641234567",
            LicenseIssuedDate = DateTime.UtcNow.AddYears(-5),
            LicenseExpiryDate = DateTime.UtcNow.AddYears(5),
            Status = DriverStatus.Active,
            Notes = "Experienced driver"
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.FullName.Should().Be("John Doe");
        result.LicenseNumber.Should().Be("ABC123");
        result.PhoneNumber.Should().Be("+381641234567");
        result.Status.Should().Be(DriverStatus.Active);
        result.HasValidLicense.Should().BeTrue();
        result.IsAvailable.Should().BeTrue();
        result.Notes.Should().Be("Experienced driver");
        result.IsDeleted.Should().BeFalse();

        // Verify in database
        var driverInDb = await _context.Drivers.FindAsync(result.Id);
        driverInDb.Should().NotBeNull();
        driverInDb!.FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task ExecuteAsync_DuplicateLicenseNumber_ThrowsDomainException()
    {
        // Arrange
        var existingDriver = Driver.Create(
            "Jane Smith",
            "XYZ789",
            "+381641111111",
            DateTime.UtcNow.AddYears(-3),
            DateTime.UtcNow.AddYears(7)
        );
        _context.Drivers.Add(existingDriver);
        await _context.SaveChangesAsync();

        var dto = new CreateDriverDto
        {
            FullName = "John Doe",
            LicenseNumber = "XYZ789", // Duplicate
            PhoneNumber = "+381642222222",
            LicenseIssuedDate = DateTime.UtcNow.AddYears(-5),
            LicenseExpiryDate = DateTime.UtcNow.AddYears(5)
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*XYZ789*already exists*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExecuteAsync_EmptyFullName_ThrowsDomainException(string fullName)
    {
        // Arrange
        var dto = new CreateDriverDto
        {
            FullName = fullName,
            LicenseNumber = "ABC123",
            PhoneNumber = "+381641234567",
            LicenseIssuedDate = DateTime.UtcNow.AddYears(-5),
            LicenseExpiryDate = DateTime.UtcNow.AddYears(5)
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*full name cannot be empty*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExecuteAsync_EmptyLicenseNumber_ThrowsDomainException(string licenseNumber)
    {
        // Arrange
        var dto = new CreateDriverDto
        {
            FullName = "John Doe",
            LicenseNumber = licenseNumber,
            PhoneNumber = "+381641234567",
            LicenseIssuedDate = DateTime.UtcNow.AddYears(-5),
            LicenseExpiryDate = DateTime.UtcNow.AddYears(5)
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*license number cannot be empty*");
    }

    [Fact]
    public async Task ExecuteAsync_FutureLicenseIssuedDate_ThrowsDomainException()
    {
        // Arrange
        var dto = new CreateDriverDto
        {
            FullName = "John Doe",
            LicenseNumber = "ABC123",
            PhoneNumber = "+381641234567",
            LicenseIssuedDate = DateTime.UtcNow.AddDays(1), // Future date
            LicenseExpiryDate = DateTime.UtcNow.AddYears(5)
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*issued date cannot be in the future*");
    }

    [Fact]
    public async Task ExecuteAsync_ExpiryBeforeIssuedDate_ThrowsDomainException()
    {
        // Arrange
        var dto = new CreateDriverDto
        {
            FullName = "John Doe",
            LicenseNumber = "ABC123",
            PhoneNumber = "+381641234567",
            LicenseIssuedDate = DateTime.UtcNow,
            LicenseExpiryDate = DateTime.UtcNow.AddDays(-1) // Before issued date
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*expiry date must be after the issued date*");
    }

    [Fact]
    public async Task ExecuteAsync_TooLongFullName_ThrowsDomainException()
    {
        // Arrange
        var dto = new CreateDriverDto
        {
            FullName = new string('A', 101), // 101 characters
            LicenseNumber = "ABC123",
            PhoneNumber = "+381641234567",
            LicenseIssuedDate = DateTime.UtcNow.AddYears(-5),
            LicenseExpiryDate = DateTime.UtcNow.AddYears(5)
        };

        // Act
        Func<Task> act = async () => await _useCase.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*full name cannot exceed 100 characters*");
    }

    [Fact]
    public async Task ExecuteAsync_ExpiredLicense_CreatesButNotAvailable()
    {
        // Arrange
        var dto = new CreateDriverDto
        {
            FullName = "John Doe",
            LicenseNumber = "ABC123",
            PhoneNumber = "+381641234567",
            LicenseIssuedDate = DateTime.UtcNow.AddYears(-10),
            LicenseExpiryDate = DateTime.UtcNow.AddDays(-1), // Expired
            Status = DriverStatus.Active
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.HasValidLicense.Should().BeFalse();
        result.IsAvailable.Should().BeFalse(); // Not available due to expired license
    }
}
