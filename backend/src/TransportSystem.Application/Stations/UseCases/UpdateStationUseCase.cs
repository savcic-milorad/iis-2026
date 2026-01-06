using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Persistence;
using TransportSystem.Application.Stations.DTOs;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Application.Stations.UseCases;

/// <summary>
/// Use case for updating an existing station
/// </summary>
public class UpdateStationUseCase
{
    private readonly IApplicationDbContext _context;

    public UpdateStationUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StationResponseDto> ExecuteAsync(Guid id, UpdateStationDto dto)
    {
        // Find station (ignoring soft delete filter to allow updating soft-deleted stations if needed)
        var station = await _context.Stations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (station == null)
        {
            throw new EntityNotFoundException("Station", id);
        }

        // Check if station is soft-deleted
        if (station.IsDeleted)
        {
            throw new DomainException($"Cannot update a deleted station. Please restore it first.");
        }

        // Check for duplicate name (excluding current station)
        var duplicateStation = await _context.Stations
            .Where(s => s.Name == dto.Name && s.Id != id)
            .FirstOrDefaultAsync();

        if (duplicateStation != null)
        {
            throw new DomainException($"A station with the name '{dto.Name}' already exists");
        }

        // Update station using domain entity method (validates internally)
        station.Update(
            dto.Name,
            dto.Latitude,
            dto.Longitude,
            dto.Address,
            dto.Description
        );

        // Save changes
        await _context.SaveChangesAsync();

        // Map to response DTO
        return MapToResponseDto(station);
    }

    private static StationResponseDto MapToResponseDto(Domain.Entities.Station station)
    {
        return new StationResponseDto
        {
            Id = station.Id,
            Name = station.Name,
            Latitude = station.Coordinates.Latitude,
            Longitude = station.Coordinates.Longitude,
            Address = station.Address,
            Description = station.Description,
            CreatedAt = station.CreatedAt,
            UpdatedAt = station.UpdatedAt,
            IsDeleted = station.IsDeleted
        };
    }
}
