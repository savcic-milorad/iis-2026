using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Persistence;
using TransportSystem.Application.Stations.DTOs;
using TransportSystem.Domain.Entities;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Application.Stations.UseCases;

/// <summary>
/// Use case for creating a new station
/// </summary>
public class CreateStationUseCase
{
    private readonly IApplicationDbContext _context;

    public CreateStationUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StationResponseDto> ExecuteAsync(CreateStationDto dto)
    {
        // Check for duplicate station name
        var existingStation = await _context.Stations
            .Where(s => s.Name == dto.Name)
            .FirstOrDefaultAsync();

        if (existingStation != null)
        {
            throw new DomainException($"A station with the name '{dto.Name}' already exists");
        }

        // Create station using domain entity factory method (validates internally)
        var station = Station.Create(
            dto.Name,
            dto.Latitude,
            dto.Longitude,
            dto.Address,
            dto.Description
        );

        // Add to database
        _context.Stations.Add(station);
        await _context.SaveChangesAsync();

        // Map to response DTO
        return MapToResponseDto(station);
    }

    private static StationResponseDto MapToResponseDto(Station station)
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
