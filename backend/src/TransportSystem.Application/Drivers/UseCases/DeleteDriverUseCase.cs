using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Persistence;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Application.Drivers.UseCases;

/// <summary>
/// Use case for soft-deleting a driver
/// </summary>
public class DeleteDriverUseCase
{
    private readonly IApplicationDbContext _context;

    public DeleteDriverUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ExecuteAsync(Guid id)
    {
        // Find driver (ignoring soft delete filter)
        var driver = await _context.Drivers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (driver == null)
        {
            throw new EntityNotFoundException("Driver", id);
        }

        // Check if already deleted
        if (driver.IsDeleted)
        {
            throw new DomainException($"Driver '{driver.FullName}' is already deleted");
        }

        // TODO: Check if driver is assigned to any active trips/workshifts (Phase 4)
        // For now, we'll just soft delete

        // Soft delete the driver using domain method
        driver.Delete();

        // Save changes
        await _context.SaveChangesAsync();
    }
}
