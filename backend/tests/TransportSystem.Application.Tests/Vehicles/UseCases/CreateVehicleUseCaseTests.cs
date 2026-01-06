using FluentAssertions;
using TransportSystem.Application.Tests.Helpers;
using TransportSystem.Application.Vehicles.DTOs;
using TransportSystem.Application.Vehicles.UseCases;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Vehicles.UseCases;

public class CreateVehicleUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CreateVehicleUseCase _useCase;

    public CreateVehicleUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new CreateVehicleUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ValidDto_CreatesVehicleSuccessfully()
    {
        // Arrange
        var dto = new CreateVehicleDto
        {
            RegistrationNumber = "NS-123-AB",
            Model = "Mercedes-Benz Citaro",
            Capacity = 80,
            ManufactureYear = 2020,
            Status = VehicleStatus.Active,
            Notes = "New bus"
        };

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.RegistrationNumber.Should().Be("NS-123-AB");
        result.Model.Should().Be("Mercedes-Benz Citaro");
        result.Capacity.Should().Be(80);
        result.ManufactureYear.Should().Be(2020);
        result.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_DuplicateRegistrationNumber_ThrowsDomainException()
    {
        // Arrange
        var existing = Vehicle.Create("NS-111-AA", "Model 1", 50, 2019);
        _context.Vehicles.Add(existing);
        await _context.SaveChangesAsync();

        var dto = new CreateVehicleDto
        {
            RegistrationNumber = "NS-111-AA",
            Model = "Model 2",
            Capacity = 60,
            ManufactureYear = 2020
        };

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(dto));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(201)]
    public async Task ExecuteAsync_InvalidCapacity_ThrowsDomainException(int capacity)
    {
        // Arrange
        var dto = new CreateVehicleDto
        {
            RegistrationNumber = "NS-123-AB",
            Model = "Test Model",
            Capacity = capacity,
            ManufactureYear = 2020
        };

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(dto));
    }
}
