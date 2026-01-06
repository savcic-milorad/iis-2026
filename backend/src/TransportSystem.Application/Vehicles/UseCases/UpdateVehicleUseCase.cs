using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Persistence;
using TransportSystem.Application.Vehicles.DTOs;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Application.Vehicles.UseCases;

/// <summary>
/// Use case for updating an existing vehicle
/// </summary>
public class UpdateVehicleUseCase
{
    private readonly IApplicationDbContext _context;

    public UpdateVehicleUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VehicleResponseDto> ExecuteAsync(Guid id, UpdateVehicleDto dto)
    {
        // Find vehicle (ignoring soft delete filter)
        var vehicle = await _context.Vehicles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            throw new EntityNotFoundException("Vehicle", id);
        }

        // Check if vehicle is soft-deleted
        if (vehicle.IsDeleted)
        {
            throw new DomainException($"Cannot update a deleted vehicle. Please restore it first.");
        }

        // Update vehicle using domain entity method (validates internally)
        vehicle.Update(
            dto.Model,
            dto.Capacity,
            dto.ManufactureYear,
            dto.Notes
        );

        // Save changes
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
