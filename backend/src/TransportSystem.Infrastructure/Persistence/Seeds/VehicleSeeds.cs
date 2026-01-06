using TransportSystem.Domain.Entities;

namespace TransportSystem.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seed data for 20 vehicles with realistic specifications for city public transport
/// </summary>
public static class VehicleSeeds
{
    public static List<Vehicle> GetVehicles()
    {
        var vehicles = new List<Vehicle>();
        var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var random = new Random(42); // Fixed seed for reproducibility

        // Bus models commonly used in Serbian public transport
        var busModels = new[]
        {
            new { Model = "Ikarbus IK-206", Capacity = 90, ManufactureYearRange = (2015, 2020) },
            new { Model = "Ikarbus IK-218", Capacity = 105, ManufactureYearRange = (2018, 2023) },
            new { Model = "MAN Lion's City", Capacity = 110, ManufactureYearRange = (2016, 2022) },
            new { Model = "Mercedes-Benz Citaro", Capacity = 100, ManufactureYearRange = (2017, 2023) },
            new { Model = "Solaris Urbino 12", Capacity = 95, ManufactureYearRange = (2019, 2024) },
            new { Model = "Solaris Urbino 18", Capacity = 140, ManufactureYearRange = (2020, 2024) },
            new { Model = "Iveco Crossway", Capacity = 85, ManufactureYearRange = (2015, 2021) },
            new { Model = "Ikarbus IK-201", Capacity = 80, ManufactureYearRange = (2010, 2015) }
        };

        for (int i = 0; i < 20; i++)
        {
            var busModel = busModels[i % busModels.Length];
            var registrationNumber = $"NS-{(100 + i):D3}-AB";

            // Generate manufacture year within the model's range
            var (minYear, maxYear) = busModel.ManufactureYearRange;
            var manufactureYear = random.Next(minYear, maxYear + 1);

            // Determine status based on age and random factors
            VehicleStatus status;
            var age = DateTime.UtcNow.Year - manufactureYear;
            var statusRoll = random.Next(100);

            if (age > 8 && statusRoll < 30) // Older vehicles more likely to be in maintenance
                status = VehicleStatus.Maintenance;
            else if (age > 12 && statusRoll < 15) // Very old vehicles might be out of service
                status = VehicleStatus.OutOfService;
            else if (statusRoll < 10) // Random maintenance
                status = VehicleStatus.Maintenance;
            else
                status = VehicleStatus.Active;

            var notes = GenerateNotes(i, status, age, random);

            var vehicle = Vehicle.Create(
                registrationNumber,
                busModel.Model,
                busModel.Capacity,
                manufactureYear,
                status,
                notes
            );

            // Set consistent CreatedAt for seed data
            var createdAtProperty = typeof(Vehicle).BaseType!.BaseType!.GetProperty("CreatedAt");
            createdAtProperty!.SetValue(vehicle, baseDate.AddDays(i * 2));

            vehicles.Add(vehicle);
        }

        return vehicles;
    }

    private static string? GenerateNotes(int index, VehicleStatus status, int age, Random random)
    {
        if (random.Next(100) < 70) // 70% have notes
        {
            if (status == VehicleStatus.Maintenance)
            {
                var maintenanceReasons = new[]
                {
                    "Scheduled maintenance - engine service",
                    "Brake system inspection and replacement",
                    "Transmission repair in progress",
                    "Air conditioning system maintenance",
                    "Routine 50,000 km service",
                    "Suspension system upgrade"
                };
                return maintenanceReasons[index % maintenanceReasons.Length];
            }

            if (status == VehicleStatus.OutOfService)
            {
                return "Scheduled for decommissioning - end of service life";
            }

            // Active vehicles
            var activeNotes = new[]
            {
                "Recently serviced - excellent condition",
                "Equipped with wheelchair accessibility",
                "Low floor model for easy access",
                "Climate control system fully operational",
                "GPS tracking system installed",
                "Recently upgraded interior",
                "Energy-efficient model",
                "Air-conditioned - suitable for summer routes",
                "Equipped with USB charging ports",
                "Video surveillance system active"
            };

            if (age <= 3)
                return "New vehicle - optimal performance";

            return activeNotes[index % activeNotes.Length];
        }

        return null;
    }
}
