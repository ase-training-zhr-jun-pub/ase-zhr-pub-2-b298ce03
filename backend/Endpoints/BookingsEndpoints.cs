using Calvin.BookingService.Data;
using Calvin.BookingService.Domain;
using Calvin.BookingService.Http;

namespace Calvin.BookingService.Endpoints;

public static class BookingsEndpoints
{
    public static IEndpointRouteBuilder MapBookings(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookings").WithTags("Bookings");

        group.MapGet("/", (InMemoryStore store, string? userId) =>
        {
            var bookings = store.Bookings.AsEnumerable();
            if (userId is not null)
                bookings = bookings.Where(b => b.UserId == userId);
            return Results.Ok(bookings.Select(b => ToResponse(b, store)).ToList());
        });

        group.MapGet("/{id}", (string id, InMemoryStore store) =>
            store.Bookings.Find(b => b.Id == id) is { } booking
                ? Results.Ok(ToResponse(booking, store))
                : Results.NotFound());

        group.MapPost("/", (BookingRequest request, InMemoryStore store) =>
        {
            if (!DateOnly.TryParse(request.Date, out var date))
                return Results.BadRequest(new { error = "Invalid date format. Use ISO 8601 (yyyy-MM-dd)." });

            TimeOnly? timeFrom = null;
            TimeOnly? timeTo = null;

            if (request.TimeFrom is not null)
            {
                if (!TimeOnly.TryParse(request.TimeFrom, out var tf))
                    return Results.BadRequest(new { error = "Invalid timeFrom format. Use HH:mm." });
                timeFrom = tf;
            }
            if (request.TimeTo is not null)
            {
                if (!TimeOnly.TryParse(request.TimeTo, out var tt))
                    return Results.BadRequest(new { error = "Invalid timeTo format. Use HH:mm." });
                timeTo = tt;
            }

            var booking = new Booking(
                store.NextBookingId(),
                request.Type,
                request.ResourceId,
                request.LocationId,
                request.UserId,
                date,
                timeFrom,
                timeTo);

            var result = store.TryAddBooking(booking);
            if (result is null)
                return Results.Conflict(new { error = "Resource already booked for this time slot." });

            var response = ToResponse(result, store);
            return Results.Created($"/api/bookings/{result.Id}", response);
        });

        group.MapDelete("/{id}", (string id, InMemoryStore store) =>
            store.RemoveBooking(id)
                ? Results.NoContent()
                : Results.NotFound());

        return app;
    }

    private static BookingResponse ToResponse(Booking b, InMemoryStore store)
    {
        var resource = ResolveResourceName(b, store);
        var location = store.Locations.Find(l => l.Id == b.LocationId)?.Name ?? b.LocationId;
        var date = b.Date.ToString("dd.MM.yyyy");
        var timeRange = b.TimeFrom is null ? "Ganztägig" : $"{b.TimeFrom:HH:mm} – {b.TimeTo:HH:mm}";
        return new BookingResponse(b.Id, b.Type, resource, location, date, timeRange);
    }

    private static string ResolveResourceName(Booking b, InMemoryStore store) =>
        b.Type == "Workplace"
            ? store.Workplaces.Find(w => w.Id == b.ResourceId) is { } wp
                ? $"{wp.Id} · {wp.Name}"
                : b.ResourceId
            : store.ConferenceRooms.Find(r => r.Id == b.ResourceId)?.Name ?? b.ResourceId;
}
