using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Persistence;
using TransportSystem.Application.Stations.DTOs;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Application.Stations.UseCases;

/// <summary>
/// Use case for retrieving a single station by ID
/// </summary>
public class GetStationUseCase
{
    private readonly IApplicationDbContext _context;

    public GetStationUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StationResponseDto> ExecuteAsync(Guid id)
    {
        // Find station (respects soft delete filter by default)
        var station = await _context.Stations
            .FirstOrDefaultAsync(s => s.Id == id);

        if (station == null)
        {
            throw new EntityNotFoundException("Station", id);
        }

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
