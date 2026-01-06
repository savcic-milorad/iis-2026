using Microsoft.EntityFrameworkCore;
using TransportSystem.Infrastructure.Persistence;

namespace TransportSystem.Application.Tests.Helpers;

public static class TestDbContextFactory
{
    public static ApplicationDbContext CreateInMemory(string databaseName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new ApplicationDbContext(options);
    }
}
