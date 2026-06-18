using Calvin.BookingService.Data;

namespace Calvin.BookingService.Endpoints;

public static class LocationsEndpoints
{
    public static IEndpointRouteBuilder MapLocations(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/locations").WithTags("Locations");

        // CLVN-002: all eight INNOQ locations for selection.
        group.MapGet("/", (CalvinDbContext db) =>
            Results.Ok(db.Locations.OrderBy(l => l.Name).ToList()));

        group.MapGet("/{id}", (string id, CalvinDbContext db) =>
            db.Locations.FirstOrDefault(l => l.Id == id) is { } location
                ? Results.Ok(location)
                : Problems.NotFound($"Location '{id}' not found."));

        return app;
    }
}
