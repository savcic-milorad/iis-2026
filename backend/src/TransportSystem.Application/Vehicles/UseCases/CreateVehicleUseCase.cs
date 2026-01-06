using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Persistence;
using TransportSystem.Application.Vehicles.DTOs;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Application.Vehicles.UseCases;

/// <summary>
/// Use case for creating a new vehicle
/// </summary>
public class CreateVehicleUseCase
{
    private readonly IApplicationDbContext _context;

    public CreateVehicleUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VehicleResponseDto> ExecuteAsync(CreateVehicleDto dto)
    {
        // Check for duplicate registration number
        var existingVehicle = await _context.Vehicles
            .Where(v => v.RegistrationNumber == dto.RegistrationNumber.ToUpperInvariant())
            .FirstOrDefaultAsync();

        if (existingVehicle != null)
        {
            throw new DomainException($"A vehicle with registration number '{dto.RegistrationNumber}' already exists");
        }

        // Create vehicle using domain entity factory method (validates internally)
        var vehicle = Vehicle.Create(
            dto.RegistrationNumber,
            dto.Model,
            dto.Capacity,
            dto.ManufactureYear,
            dto.Status,
            dto.Notes
        );

        // Add to database
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        // Map to response DTO
        return MapToResponseDto(vehicle);
    }

    private static VehicleResponseDto MapToResponseDto(Vehicle vehicle)
    {
        return new VehicleResponseDto
        {
            Id = vehicle.Id,
            RegistrationNumber = vehicle.RegistrationNumber,
            Model = vehicle.Model,
            Capacity = vehicle.Capacity,
            ManufactureYear = vehicle.ManufactureYear,
            Status = vehicle.Status,
            IsAvailable = vehicle.IsAvailable(),
            Notes = vehicle.Notes,
            CreatedAt = vehicle.CreatedAt,
            UpdatedAt = vehicle.UpdatedAt,
            IsDeleted = vehicle.IsDeleted
        };
    }
}
