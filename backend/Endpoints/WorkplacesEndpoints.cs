using Calvin.BookingService.Data;
using Calvin.BookingService.Domain;
using Calvin.BookingService.Http;

namespace Calvin.BookingService.Endpoints;

public static class WorkplacesEndpoints
{
    public static IEndpointRouteBuilder MapWorkplaces(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/workplaces").WithTags("Workplaces");

        // CLVN-014: available workplaces for a location/date. When `date` is given,
        // occupancy is computed from active bookings (workplaces are booked all-day).
        group.MapGet("/", (CalvinDbContext db,
            string? locationId, string? date, string? equipment) =>
        {
            var workplaces = db.Workplaces.AsEnumerable();
            if (locationId is not null)
                workplaces = workplaces.Where(w => w.LocationId == locationId);

            if (equipment is not null)
            {
                var wanted = SplitEquipment(equipment);
                workplaces = workplaces.Where(w => wanted.All(e =>
                    w.Equipment.Contains(e, StringComparer.OrdinalIgnoreCase)));
            }

            DateOnly? parsedDate = date is not null && DateOnly.TryParse(date, out var d) ? d : null;
            var dayBookings = LoadDayBookings(db, parsedDate);

            var result = workplaces
                .Select(w => ToResponse(w, parsedDate, dayBookings))
                .ToList();

            return Results.Ok(result);
        });

        group.MapGet("/{id}", (string id, CalvinDbContext db, string? date) =>
        {
            var wp = db.Workplaces.FirstOrDefault(w => w.Id == id);
            if (wp is null) return Problems.NotFound($"Workplace '{id}' not found.");
            DateOnly? parsedDate = date is not null && DateOnly.TryParse(date, out var d) ? d : null;
            var dayBookings = LoadDayBookings(db, parsedDate);
            return Results.Ok(ToResponse(wp, parsedDate, dayBookings));
        });

        return app;
    }

    private static List<Booking> LoadDayBookings(CalvinDbContext db, DateOnly? date) =>
        date is { } d
            ? db.Bookings.Where(b => b.Status == BookingStatus.Active && b.Date == d).ToList()
            : [];

    private static WorkplaceResponse ToResponse(Workplace w, DateOnly? date, List<Booking> dayBookings)
    {
        // Workplaces are booked all-day, so any active booking on the date occupies them.
        var occupied = date is not null
            ? dayBookings.Any(b => b.ResourceId == w.Id)
            : w.Occupied;

        return new WorkplaceResponse(w.Id, w.Name, w.Floor, w.Equipment.ToArray(), occupied, w.LocationId);
    }

    private static string[] SplitEquipment(string csv) =>
        csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
