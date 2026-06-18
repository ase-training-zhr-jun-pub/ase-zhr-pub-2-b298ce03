using Calvin.BookingService.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Calvin.BookingService.IntegrationTests;

// Replaces the SQLite DB with an isolated in-memory DB for each test run.
// A unique DB name per factory instance prevents cross-test contamination.
public class CustomWebAppFactory : WebApplicationFactory<Program>
{
    // Captured once per factory instance so all requests within one test class
    // share the same in-memory DB (required for create-then-fetch tests).
    private readonly string _dbName = "testdb-" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var dbName = _dbName; // capture for lambda

        builder.ConfigureServices(services =>
        {
            // Remove ALL descriptors that belong to EF Core / the DbContext so
            // neither the Sqlite provider nor its internal services remain.
            var toRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<CalvinDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(CalvinDbContext) ||
                    d.ServiceType.Namespace?.StartsWith("Microsoft.EntityFrameworkCore") == true)
                .ToList();

            foreach (var d in toRemove) services.Remove(d);

            // Re-add CalvinDbContext backed by an isolated in-memory database.
            // dbName is fixed per factory so all HTTP requests in a test class
            // share the same database and can observe each other's changes.
            services.AddDbContext<CalvinDbContext>(o =>
                o.UseInMemoryDatabase(dbName));
        });
    }
}
