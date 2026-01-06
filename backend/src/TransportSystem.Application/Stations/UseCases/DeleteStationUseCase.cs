using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Persistence;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Application.Stations.UseCases;

/// <summary>
/// Use case for soft-deleting a station
/// </summary>
public class DeleteStationUseCase
{
    private readonly IApplicationDbContext _context;

    public DeleteStationUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ExecuteAsync(Guid id)
    {
        // Find station (ignoring soft delete filter)
        var station = await _context.Stations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (station == null)
        {
            throw new EntityNotFoundException("Station", id);
        }

        // Check if already deleted
        if (station.IsDeleted)
        {
            throw new DomainException($"Station '{station.Name}' is already deleted");
        }

        // TODO: Check if station is used in any routes (Phase 2.2)
        // For now, we'll just soft delete it

        // Soft delete the station using domain method
        station.Delete();

        // Save changes
        await _context.SaveChangesAsync();
    }
}
