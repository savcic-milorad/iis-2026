using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Persistence;
using TransportSystem.Application.Vehicles.DTOs;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Application.Vehicles.UseCases;

/// <summary>
/// Use case for retrieving a single vehicle by ID
/// </summary>
public class GetVehicleUseCase
{
    private readonly IApplicationDbContext _context;

    public GetVehicleUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VehicleResponseDto> ExecuteAsync(Guid id)
    {
        // Find vehicle (respects soft delete filter by default)
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            throw new EntityNotFoundException("Vehicle", id);
        }

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
