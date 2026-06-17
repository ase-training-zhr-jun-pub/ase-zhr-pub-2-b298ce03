using Calvin.BookingService.Data;

namespace Calvin.BookingService.Endpoints;

public static class WorkplacesEndpoints
{
    public static IEndpointRouteBuilder MapWorkplaces(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/workplaces").WithTags("Workplaces");

        group.MapGet("/", (InMemoryStore store, string? locationId, string? date) =>
        {
            var workplaces = store.Workplaces.AsEnumerable();
            if (locationId is not null)
                workplaces = workplaces.Where(w => w.LocationId == locationId);

            if (date is not null && DateOnly.TryParse(date, out var parsedDate))
            {
                workplaces = workplaces.Select(w => w with
                {
                    Occupied = store.Bookings.Any(b =>
                        b.ResourceId == w.Id && b.Date == parsedDate)
                });
            }

            return Results.Ok(workplaces.ToList());
        });

        group.MapGet("/{id}", (string id, InMemoryStore store) =>
            store.Workplaces.Find(w => w.Id == id) is { } workplace
                ? Results.Ok(workplace)
                : Results.NotFound());

        return app;
    }
}
