using Calvin.BookingService.Data;

namespace Calvin.BookingService.Endpoints;

public static class LocationsEndpoints
{
    public static IEndpointRouteBuilder MapLocations(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/locations").WithTags("Locations");

        group.MapGet("/", (InMemoryStore store) =>
            Results.Ok(store.Locations));

        group.MapGet("/{id}", (string id, InMemoryStore store) =>
            store.Locations.Find(l => l.Id == id) is { } location
                ? Results.Ok(location)
                : Results.NotFound());

        return app;
    }
}
