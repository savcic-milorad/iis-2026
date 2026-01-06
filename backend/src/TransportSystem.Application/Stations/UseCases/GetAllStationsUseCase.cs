using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Persistence;
using TransportSystem.Application.Stations.DTOs;

namespace TransportSystem.Application.Stations.UseCases;

/// <summary>
/// Use case for retrieving all stations with optional filtering and search
/// </summary>
public class GetAllStationsUseCase
{
    private readonly IApplicationDbContext _context;

    public GetAllStationsUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all stations with optional search term
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter by name or address</param>
    /// <param name="includeDeleted">Whether to include soft-deleted stations (default: false)</param>
    public async Task<List<StationResponseDto>> ExecuteAsync(string? searchTerm = null, bool includeDeleted = false)
    {
        // Start with base query
        IQueryable<Domain.Entities.Station> query = _context.Stations;

        // Include deleted stations if requested
        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchTermLower = searchTerm.ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(searchTermLower) ||
                s.Address.ToLower().Contains(searchTermLower) ||
                (s.Description != null && s.Description.ToLower().Contains(searchTermLower))
            );
        }

        // Order by name
        query = query.OrderBy(s => s.Name);

        // Execute query and map to DTOs
        var stations = await query.ToListAsync();

        return stations.Select(MapToResponseDto).ToList();
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
