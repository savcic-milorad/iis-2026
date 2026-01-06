using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Tests.Helpers;
using TransportSystem.Application.Vehicles.DTOs;
using TransportSystem.Application.Vehicles.UseCases;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Vehicles.UseCases;

// GetVehicleUseCase Tests
public class GetVehicleUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GetVehicleUseCase _useCase;

    public GetVehicleUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new GetVehicleUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ExistingId_ReturnsVehicle()
    {
        var vehicle = Vehicle.Create("NS-123-AB", "Mercedes Citaro", 80, 2020);
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        var result = await _useCase.ExecuteAsync(vehicle.Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(vehicle.Id);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistingId_ThrowsEntityNotFoundException()
    {
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _useCase.ExecuteAsync(Guid.NewGuid()));
    }
}

// UpdateVehicleUseCase Tests
public class UpdateVehicleUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UpdateVehicleUseCase _useCase;

    public UpdateVehicleUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new UpdateVehicleUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ValidDto_UpdatesSuccessfully()
    {
        var vehicle = Vehicle.Create("NS-123-AB", "Old Model", 50, 2015);
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        var dto = new UpdateVehicleDto
        {
            Model = "New Model",
            Capacity = 80,
            ManufactureYear = 2020
        };

        var result = await _useCase.ExecuteAsync(vehicle.Id, dto);

        result.Model.Should().Be("New Model");
        result.Capacity.Should().Be(80);
    }
}

// DeleteVehicleUseCase Tests
public class DeleteVehicleUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly DeleteVehicleUseCase _useCase;

    public DeleteVehicleUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new DeleteVehicleUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ExistingVehicle_SoftDeletesSuccessfully()
    {
        var vehicle = Vehicle.Create("NS-123-AB", "Model", 80, 2020);
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        await _useCase.ExecuteAsync(vehicle.Id);

        var deleted = await _context.Vehicles
            .IgnoreQueryFilters()
            .FirstAsync(v => v.Id == vehicle.Id);
        deleted.IsDeleted.Should().BeTrue();
    }
}

// GetAllVehiclesUseCase Tests
public class GetAllVehiclesUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GetAllVehiclesUseCase _useCase;

    public GetAllVehiclesUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new GetAllVehiclesUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_NoFilters_ReturnsAllVehicles()
    {
        var v1 = Vehicle.Create("NS-111-AA", "Model 1", 50, 2019);
        var v2 = Vehicle.Create("NS-222-BB", "Model 2", 60, 2020);
        _context.Vehicles.AddRange(v1, v2);
        await _context.SaveChangesAsync();

        var result = await _useCase.ExecuteAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExecuteAsync_SearchByModel_ReturnsMatching()
    {
        var v1 = Vehicle.Create("NS-111-AA", "Mercedes Citaro", 50, 2019);
        var v2 = Vehicle.Create("NS-222-BB", "MAN Lion", 60, 2020);
        _context.Vehicles.AddRange(v1, v2);
        await _context.SaveChangesAsync();

        var result = await _useCase.ExecuteAsync(searchTerm: "mercedes");

        result.Should().HaveCount(1);
        result[0].Model.Should().Be("Mercedes Citaro");
    }
}

// ChangeVehicleStatusUseCase Tests
public class ChangeVehicleStatusUseCaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ChangeVehicleStatusUseCase _useCase;

    public ChangeVehicleStatusUseCaseTests()
    {
        _context = TestDbContextFactory.CreateInMemory(Guid.NewGuid().ToString());
        _useCase = new ChangeVehicleStatusUseCase(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ChangeToMaintenance_UpdatesSuccessfully()
    {
        var vehicle = Vehicle.Create("NS-123-AB", "Model", 80, 2020, VehicleStatus.Active);
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        var result = await _useCase.ExecuteAsync(vehicle.Id, VehicleStatus.Maintenance);

        result.Status.Should().Be(VehicleStatus.Maintenance);
        result.IsAvailable.Should().BeFalse();
    }
}
