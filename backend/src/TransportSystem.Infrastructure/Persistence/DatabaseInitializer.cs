using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransportSystem.Infrastructure.Identity;
using TransportSystem.Infrastructure.Persistence.Seeds;

namespace TransportSystem.Infrastructure.Persistence;

/// <summary>
/// Handles database initialization, migration, and seeding
/// </summary>
public class DatabaseInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<DatabaseInitializer> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    /// <summary>
    /// Initializes the database by running migrations and seeding data
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            // Apply pending migrations
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                _logger.LogInformation("Applying pending migrations...");
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Migrations applied successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }

        await SeedAsync();
    }

    /// <summary>
    /// Seeds the database with initial data
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            // Seed users and roles first
            _logger.LogInformation("Seeding users and roles...");
            await UserSeeds.SeedUsersAsync(_userManager, _roleManager);
            _logger.LogInformation("Users and roles seeded successfully");

            // Seed stations
            if (!await _context.Stations.AnyAsync())
            {
                _logger.LogInformation("Seeding stations...");
                var stations = StationSeeds.GetStations();
                await _context.Stations.AddRangeAsync(stations);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {stations.Count} stations successfully");
            }
            else
            {
                _logger.LogInformation("Stations already exist, skipping seed");
            }

            // Seed drivers
            if (!await _context.Drivers.AnyAsync())
            {
                _logger.LogInformation("Seeding drivers...");
                var drivers = DriverSeeds.GetDrivers();
                await _context.Drivers.AddRangeAsync(drivers);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {drivers.Count} drivers successfully");
            }
            else
            {
                _logger.LogInformation("Drivers already exist, skipping seed");
            }

            // Seed vehicles
            if (!await _context.Vehicles.AnyAsync())
            {
                _logger.LogInformation("Seeding vehicles...");
                var vehicles = VehicleSeeds.GetVehicles();
                await _context.Vehicles.AddRangeAsync(vehicles);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Seeded {vehicles.Count} vehicles successfully");
            }
            else
            {
                _logger.LogInformation("Vehicles already exist, skipping seed");
            }

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}
