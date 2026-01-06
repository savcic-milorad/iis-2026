using Microsoft.AspNetCore.Identity;
using TransportSystem.Infrastructure.Identity;

namespace TransportSystem.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seed data for initial system users (Admin and Planner roles)
/// </summary>
public static class UserSeeds
{
    public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Create roles if they don't exist
        await CreateRoleIfNotExistsAsync(roleManager, "Admin");
        await CreateRoleIfNotExistsAsync(roleManager, "Planner");
        await CreateRoleIfNotExistsAsync(roleManager, "Driver");

        // Seed Admin user
        var adminEmail = "admin@transportsystem.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator"
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed Planner user
        var plannerEmail = "planner@transportsystem.com";
        var plannerUser = await userManager.FindByEmailAsync(plannerEmail);

        if (plannerUser == null)
        {
            plannerUser = new ApplicationUser
            {
                UserName = plannerEmail,
                Email = plannerEmail,
                EmailConfirmed = true,
                FullName = "Route Planner"
            };

            var result = await userManager.CreateAsync(plannerUser, "Planner123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(plannerUser, "Planner");
            }
        }

        // Seed additional test users
        var testUsers = new[]
        {
            new { Email = "driver1@transportsystem.com", FullName = "Test Driver 1", Role = "Driver", Password = "Driver123!" },
            new { Email = "driver2@transportsystem.com", FullName = "Test Driver 2", Role = "Driver", Password = "Driver123!" },
            new { Email = "planner2@transportsystem.com", FullName = "Assistant Planner", Role = "Planner", Password = "Planner123!" }
        };

        foreach (var testUserData in testUsers)
        {
            var existingUser = await userManager.FindByEmailAsync(testUserData.Email);

            if (existingUser == null)
            {
                var testUser = new ApplicationUser
                {
                    UserName = testUserData.Email,
                    Email = testUserData.Email,
                    EmailConfirmed = true,
                    FullName = testUserData.FullName
                };

                var result = await userManager.CreateAsync(testUser, testUserData.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(testUser, testUserData.Role);
                }
            }
        }
    }

    private static async Task CreateRoleIfNotExistsAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
