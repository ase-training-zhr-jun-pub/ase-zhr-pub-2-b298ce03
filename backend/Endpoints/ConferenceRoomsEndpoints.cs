using Calvin.BookingService.Data;
using Calvin.BookingService.Domain;
using Calvin.BookingService.Http;

namespace Calvin.BookingService.Endpoints;

public static class ConferenceRoomsEndpoints
{
    // Office hours used for the alternative-slot search (CLVN-012).
    private static readonly TimeOnly OfficeOpen = new(8, 0);
    private static readonly TimeOnly OfficeClose = new(18, 0);
    private static readonly TimeSpan SlotStep = TimeSpan.FromMinutes(15);

    public static IEndpointRouteBuilder MapConferenceRooms(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/conference-rooms").WithTags("Conference Rooms");

        // CLVN-003/004/005/010/013: list rooms with optional location, capacity,
        // equipment and date/time-based availability filtering + sorting.
        group.MapGet("/", (CalvinDbContext db,
            string? locationId, string? date, string? timeFrom, string? timeTo,
            int? minCapacity, string? equipment, string? sort) =>
        {
            var rooms = db.ConferenceRooms.AsEnumerable();

            if (locationId is not null)
                rooms = rooms.Where(r => r.LocationId == locationId);

            if (minCapacity is { } min)
                rooms = rooms.Where(r => r.Capacity >= min);

            if (equipment is not null)
            {
                var wanted = SplitEquipment(equipment);
                rooms = rooms.Where(r => wanted.All(e =>
                    r.Equipment.Contains(e, StringComparer.OrdinalIgnoreCase)));
            }

            DateOnly? parsedDate = date is not null && DateOnly.TryParse(date, out var d) ? d : null;
            TimeOnly? from = timeFrom is not null && TimeOnly.TryParse(timeFrom, out var f) ? f : null;
            TimeOnly? to = timeTo is not null && TimeOnly.TryParse(timeTo, out var t) ? t : null;

            var dayBookings = LoadDayBookings(db, parsedDate);

            var result = rooms
                .Select(r => ToResponse(r, parsedDate, from, to, dayBookings))
                .ToList();

            result = ApplySort(result, sort);
            return Results.Ok(result);
        });

        group.MapGet("/{id}", (string id, CalvinDbContext db,
            string? date, string? timeFrom, string? timeTo) =>
        {
            var room = db.ConferenceRooms.FirstOrDefault(r => r.Id == id);
            if (room is null) return Problems.NotFound($"Conference room '{id}' not found.");

            DateOnly? parsedDate = date is not null && DateOnly.TryParse(date, out var d) ? d : null;
            TimeOnly? from = timeFrom is not null && TimeOnly.TryParse(timeFrom, out var f) ? f : null;
            TimeOnly? to = timeTo is not null && TimeOnly.TryParse(timeTo, out var t) ? t : null;

            var dayBookings = LoadDayBookings(db, parsedDate);
            return Results.Ok(ToResponse(room, parsedDate, from, to, dayBookings));
        });

        // CLVN-010/012: check availability for a date/time range and, on conflict,
        // suggest up to three alternative slots of the same duration.
        group.MapGet("/{id}/availability", (string id, CalvinDbContext db,
            string? date, string? timeFrom, string? timeTo) =>
        {
            var room = db.ConferenceRooms.FirstOrDefault(r => r.Id == id);
            if (room is null) return Problems.NotFound($"Conference room '{id}' not found.");

            if (date is null || !DateOnly.TryParse(date, out var parsedDate))
                return Problems.Validation("Query parameter 'date' (yyyy-MM-dd) is required.");

            TimeOnly? from = timeFrom is not null && TimeOnly.TryParse(timeFrom, out var f) ? f : null;
            TimeOnly? to = timeTo is not null && TimeOnly.TryParse(timeTo, out var t) ? t : null;

            var dayBookings = db.Bookings
                .Where(b => b.Status == BookingStatus.Active &&
                            b.ResourceId == id && b.Date == parsedDate)
                .ToList();

            var conflicts = dayBookings
                .Where(b => BookingRules.Overlaps(b.TimeFrom, b.TimeTo, from, to))
                .Select(b => BookingMapper.ToResponse(b, db))
                .ToArray();

            var available = conflicts.Length == 0;
            TimeSlotResponse[] alternatives = available || from is null || to is null
                ? []
                : FindAlternatives(dayBookings, from.Value, to.Value);

            return Results.Ok(new AvailabilityResponse(available, conflicts, alternatives));
        });

        // CLVN-011: occupied time windows of a room on a given day.
        group.MapGet("/{id}/bookings", (string id, CalvinDbContext db, string? date) =>
        {
            if (db.ConferenceRooms.All(r => r.Id != id))
                return Problems.NotFound($"Conference room '{id}' not found.");

            var bookings = db.Bookings
                .Where(b => b.Status == BookingStatus.Active && b.ResourceId == id);

            if (date is not null && DateOnly.TryParse(date, out var d))
                bookings = bookings.Where(b => b.Date == d);

            var result = bookings
                .AsEnumerable()
                .OrderBy(b => b.Date)
                .ThenBy(b => b.TimeFrom ?? TimeOnly.MinValue)
                .Select(b => BookingMapper.ToResponse(b, db))
                .ToList();

            return Results.Ok(result);
        });

        return app;
    }

    // Active bookings for the given day, materialized so occupancy/overlap checks
    // run in memory (BookingRules.Overlaps cannot be translated to SQL).
    private static List<Booking> LoadDayBookings(CalvinDbContext db, DateOnly? date) =>
        date is { } d
            ? db.Bookings.Where(b => b.Status == BookingStatus.Active && b.Date == d).ToList()
            : [];

    private static ConferenceRoomResponse ToResponse(
        ConferenceRoom r, DateOnly? date, TimeOnly? from, TimeOnly? to, List<Booking> dayBookings)
    {
        var occupied = date is not null
            ? dayBookings.Any(b =>
                b.ResourceId == r.Id &&
                BookingRules.Overlaps(b.TimeFrom, b.TimeTo, from, to))
            : r.Occupied;

        return new ConferenceRoomResponse(
            r.Id, r.Name, r.Capacity, r.Equipment.ToArray(), occupied, r.LocationId);
    }

    // Returns up to three free slots of the requested duration, nearest first.
    private static TimeSlotResponse[] FindAlternatives(
        List<Booking> dayBookings, TimeOnly from, TimeOnly to)
    {
        var duration = to - from;
        if (duration <= TimeSpan.Zero) return [];

        var candidates = new List<TimeOnly>();
        for (var start = OfficeOpen; start.Add(duration) <= OfficeClose; start = start.Add(SlotStep))
        {
            if (start == from) continue; // skip the originally requested slot
            var end = start.Add(duration);
            var free = !dayBookings.Any(b => BookingRules.Overlaps(b.TimeFrom, b.TimeTo, start, end));
            if (free) candidates.Add(start);
        }

        return candidates
            .OrderBy(s => Math.Abs((s - from).Ticks))
            .Take(3)
            .Select(s => new TimeSlotResponse(s.ToString("HH:mm"), s.Add(duration).ToString("HH:mm")))
            .ToArray();
    }

    private static List<ConferenceRoomResponse> ApplySort(List<ConferenceRoomResponse> rooms, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort)) return rooms;
        var desc = sort.StartsWith('-');
        var key = sort.TrimStart('-');

        IEnumerable<ConferenceRoomResponse> sorted = key switch
        {
            "name" => desc ? rooms.OrderByDescending(r => r.Name) : rooms.OrderBy(r => r.Name),
            "capacity" => desc ? rooms.OrderByDescending(r => r.Capacity) : rooms.OrderBy(r => r.Capacity),
            _ => rooms,
        };
        return sorted.ToList();
    }

    private static string[] SplitEquipment(string csv) =>
        csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
