using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Persistence;
using TransportSystem.Application.Vehicles.DTOs;
using TransportSystem.Domain.Entities;

namespace TransportSystem.Application.Vehicles.UseCases;

/// <summary>
/// Use case for retrieving all vehicles with optional filtering and search
/// </summary>
public class GetAllVehiclesUseCase
{
    private readonly IApplicationDbContext _context;

    public GetAllVehiclesUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all vehicles with optional search term and status filter
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter by registration number, model, or notes</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="includeDeleted">Whether to include soft-deleted vehicles (default: false)</param>
    public async Task<List<VehicleResponseDto>> ExecuteAsync(
        string? searchTerm = null,
        VehicleStatus? status = null,
        bool includeDeleted = false)
    {
        // Start with base query
        IQueryable<Vehicle> query = _context.Vehicles;

        // Include deleted vehicles if requested
        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchTermLower = searchTerm.ToLower();
            query = query.Where(v =>
                v.RegistrationNumber.ToLower().Contains(searchTermLower) ||
                v.Model.ToLower().Contains(searchTermLower) ||
                (v.Notes != null && v.Notes.ToLower().Contains(searchTermLower))
            );
        }

        // Apply status filter if provided
        if (status.HasValue)
        {
            query = query.Where(v => v.Status == status.Value);
        }

        // Order by registration number
        query = query.OrderBy(v => v.RegistrationNumber);

        // Execute query and map to DTOs
        var vehicles = await query.ToListAsync();

        return vehicles.Select(MapToResponseDto).ToList();
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
