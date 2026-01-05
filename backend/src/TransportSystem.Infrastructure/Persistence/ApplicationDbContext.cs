using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

    // DbSets for domain entities will be added in Phase 1.3
    // public DbSet<Station> Stations => Set<Station>();
    // public DbSet<Driver> Drivers => Set<Driver>();
    // public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    // public DbSet<Route> Routes => Set<Route>();
    // public DbSet<Line> Lines => Set<Line>();
    // public DbSet<Schedule> Schedules => Set<Schedule>();
    // public DbSet<Slot> Slots => Set<Slot>();
    // public DbSet<Trip> Trips => Set<Trip>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Identity tables with custom schema (optional)
        builder.HasDefaultSchema("dbo");

        // Customize Identity table names (optional)
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
        });

        // Entity configurations will be added in Phase 1.3
        // builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
