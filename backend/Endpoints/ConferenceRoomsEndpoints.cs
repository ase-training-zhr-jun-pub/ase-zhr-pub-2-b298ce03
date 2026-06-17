using Calvin.BookingService.Data;
using Calvin.BookingService.Domain;

namespace Calvin.BookingService.Endpoints;

public static class ConferenceRoomsEndpoints
{
    public static IEndpointRouteBuilder MapConferenceRooms(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/conference-rooms").WithTags("Conference Rooms");

        group.MapGet("/", (InMemoryStore store,
            string? locationId, string? date, string? timeFrom, string? timeTo) =>
        {
            var rooms = store.ConferenceRooms.AsEnumerable();
            if (locationId is not null)
                rooms = rooms.Where(r => r.LocationId == locationId);

            if (date is not null && DateOnly.TryParse(date, out var parsedDate))
            {
                TimeOnly? from = timeFrom is not null && TimeOnly.TryParse(timeFrom, out var f) ? f : null;
                TimeOnly? to = timeTo is not null && TimeOnly.TryParse(timeTo, out var t) ? t : null;

                rooms = rooms.Select(r => r with
                {
                    Occupied = store.Bookings.Any(b =>
                        b.ResourceId == r.Id &&
                        b.Date == parsedDate &&
                        TimeRangesOverlap(b.TimeFrom, b.TimeTo, from, to))
                });
            }

            return Results.Ok(rooms.ToList());
        });

        group.MapGet("/{id}", (string id, InMemoryStore store) =>
            store.ConferenceRooms.Find(r => r.Id == id) is { } room
                ? Results.Ok(room)
                : Results.NotFound());

        return app;
    }

    private static bool TimeRangesOverlap(TimeOnly? aFrom, TimeOnly? aTo, TimeOnly? bFrom, TimeOnly? bTo)
    {
        if (aFrom is null || bFrom is null) return true;
        return aFrom < bTo && bFrom < aTo;
    }
}
