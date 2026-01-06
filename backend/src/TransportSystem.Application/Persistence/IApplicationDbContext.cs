using Microsoft.EntityFrameworkCore;
using TransportSystem.Domain.Entities;

namespace TransportSystem.Application.Persistence;

/// <summary>
/// Interface for the application database context
/// Allows the application layer to interact with the database without depending on Infrastructure
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Station> Stations { get; }
    DbSet<Driver> Drivers { get; }
    DbSet<Vehicle> Vehicles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
