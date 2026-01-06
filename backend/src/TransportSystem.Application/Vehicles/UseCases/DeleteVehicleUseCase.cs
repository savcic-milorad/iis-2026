using Microsoft.EntityFrameworkCore;
using TransportSystem.Application.Persistence;
using TransportSystem.Domain.Exceptions;

namespace TransportSystem.Application.Vehicles.UseCases;

/// <summary>
/// Use case for soft-deleting a vehicle
/// </summary>
public class DeleteVehicleUseCase
{
    private readonly IApplicationDbContext _context;

    public DeleteVehicleUseCase(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ExecuteAsync(Guid id)
    {
        // Find vehicle (ignoring soft delete filter)
        var vehicle = await _context.Vehicles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            throw new EntityNotFoundException("Vehicle", id);
        }

        // Check if already deleted
        if (vehicle.IsDeleted)
        {
            throw new DomainException($"Vehicle '{vehicle.RegistrationNumber}' is already deleted");
        }

        // TODO: Check if vehicle is assigned to any active trips/slots (Phase 4)
        // For now, we'll just soft delete

        // Soft delete the vehicle using domain method
        vehicle.Delete();

        // Save changes
        await _context.SaveChangesAsync();
    }
}
