using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Persistence;
using TransportSystem.Application.Vehicles.DTOs;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Application.Vehicles.UseCases;

/// <summary>
/// Use case for changing vehicle status (Active, Maintenance, OutOfService)
/// </summary>
public class ChangeVehicleStatusUseCase
{
    private readonly IApplicationDbContext _context;

    public ChangeVehicleStatusUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VehicleResponseDto> ExecuteAsync(Guid id, VehicleStatus newStatus)
    {
        // Find vehicle
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            throw new EntityNotFoundException("Vehicle", id);
        }

        // Change status using domain method
        vehicle.ChangeStatus(newStatus);

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
