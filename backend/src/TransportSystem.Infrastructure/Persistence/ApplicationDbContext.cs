using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TransportSystem.Domain.Entities;
using TransportSystem.Infrastructure.Identity;

namespace TransportSystem.Infrastructure.Persistence;

/// <summary>
/// Main database context for the Transport System
/// Extends IdentityDbContext to include ASP.NET Core Identity tables
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets for domain entities
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    // public DbSet<Route> Routes => Set<Route>(); // Phase 2
    // public DbSet<Line> Lines => Set<Line>(); // Phase 3
    // public DbSet<Schedule> Schedules => Set<Schedule>(); // Phase 3
    // public DbSet<Slot> Slots => Set<Slot>(); // Phase 3
    // public DbSet<Trip> Trips => Set<Trip>(); // Phase 3

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Identity tables with custom schema
        builder.HasDefaultSchema("dbo");

        // Customize Identity table names
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
        });

        // Apply all entity configurations from this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure global query filters for soft delete
        builder.Entity<Station>().HasQueryFilter(s => !s.IsDeleted);
        builder.Entity<Driver>().HasQueryFilter(d => !d.IsDeleted);
        builder.Entity<Vehicle>().HasQueryFilter(v => !v.IsDeleted);
    }
}
