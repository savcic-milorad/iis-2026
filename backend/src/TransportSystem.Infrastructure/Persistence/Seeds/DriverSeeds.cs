using TransportSystem.Domain.Entities;

namespace TransportSystem.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seed data for 30 drivers with realistic Serbian names and valid license information
/// </summary>
public static class DriverSeeds
{
    public static List<Driver> GetDrivers()
    {
        var drivers = new List<Driver>();
        var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var random = new Random(42); // Fixed seed for reproducibility

        var driverData = new[]
        {
            new { FullName = "Marko Petrović", Phone = "+381641234001" },
            new { FullName = "Nikola Jovanović", Phone = "+381641234002" },
            new { FullName = "Stefan Nikolić", Phone = "+381641234003" },
            new { FullName = "Aleksandar Đorđević", Phone = "+381641234004" },
            new { FullName = "Milan Ilić", Phone = "+381641234005" },
            new { FullName = "Đorđe Stanković", Phone = "+381641234006" },
            new { FullName = "Nemanja Pavlović", Phone = "+381234007" },
            new { FullName = "Luka Simić", Phone = "+381641234008" },
            new { FullName = "Dejan Živković", Phone = "+381641234009" },
            new { FullName = "Vladimir Marković", Phone = "+381641234010" },
            new { FullName = "Ivan Milošević", Phone = "+381641234011" },
            new { FullName = "Dušan Savić", Phone = "+381641234012" },
            new { FullName = "Petar Popović", Phone = "+381641234013" },
            new { FullName = "Miloš Kostić", Phone = "+381641234014" },
            new { FullName = "Jovana Đurić", Phone = "+381641234015" },
            new { FullName = "Ana Stojanović", Phone = "+381641234016" },
            new { FullName = "Milica Radovanović", Phone = "+381641234017" },
            new { FullName = "Jelena Tomić", Phone = "+381641234018" },
            new { FullName = "Katarina Kovačević", Phone = "+381641234019" },
            new { FullName = "Tijana Radić", Phone = "+381641234020" },
            new { FullName = "Marija Stefanović", Phone = "+381641234021" },
            new { FullName = "Ivana Lazarević", Phone = "+381641234022" },
            new { FullName = "Teodora Ristić", Phone = "+381641234023" },
            new { FullName = "Sara Milovanović", Phone = "+381641234024" },
            new { FullName = "Boris Stanišić", Phone = "+381641234025" },
            new { FullName = "Uroš Vasiljević", Phone = "+381641234026" },
            new { FullName = "Filip Todorović", Phone = "+381641234027" },
            new { FullName = "Nikola Antić", Phone = "+381641234028" },
            new { FullName = "Darko Jović", Phone = "+381641234029" },
            new { FullName = "Zoran Mitrović", Phone = "+381641234030" }
        };

        for (int i = 0; i < driverData.Length; i++)
        {
            var data = driverData[i];
            var licenseNumber = $"NS{(i + 1):D6}";

            // Generate realistic license dates
            var yearsWithLicense = random.Next(2, 20); // 2-20 years of driving experience
            var licenseIssuedDate = DateTime.UtcNow.AddYears(-yearsWithLicense).Date;
            var licenseExpiryDate = licenseIssuedDate.AddYears(10).Date; // Serbian licenses valid for 10 years

            // Determine status - most active, some on leave, few suspended
            DriverStatus status;
            var statusRoll = random.Next(100);
            if (statusRoll < 80) // 80% active
                status = DriverStatus.Active;
            else if (statusRoll < 95) // 15% on leave
                status = DriverStatus.OnLeave;
            else // 5% suspended
                status = DriverStatus.Suspended;

            var notes = GenerateNotes(i, status, random);

            var driver = Driver.Create(
                data.FullName,
                licenseNumber,
                data.Phone,
                licenseIssuedDate,
                licenseExpiryDate,
                status,
                userId: null,
                notes: notes
            );

            // Set consistent CreatedAt for seed data
            var createdAtProperty = typeof(Driver).BaseType!.BaseType!.GetProperty("CreatedAt");
            createdAtProperty!.SetValue(driver, baseDate.AddDays(i));

            drivers.Add(driver);
        }

        return drivers;
    }

    private static string? GenerateNotes(int index, DriverStatus status, Random random)
    {
        if (random.Next(100) < 60) // 60% have notes
        {
            var noteOptions = new[]
            {
                "Experienced with city routes",
                "Specializes in night shifts",
                "Excellent safety record",
                "Trained on new vehicle models",
                "Preferred for long routes",
                "Multilingual - speaks English and German",
                "Customer service excellence award 2023",
                "Recently completed advanced driving course",
                "Experienced with accessibility features",
                "Route optimization specialist"
            };

            if (status == DriverStatus.OnLeave)
                return "Currently on medical leave, expected return next month";

            if (status == DriverStatus.Suspended)
                return "Suspended pending investigation, review scheduled";

            return noteOptions[index % noteOptions.Length];
        }

        return null;
    }
}
