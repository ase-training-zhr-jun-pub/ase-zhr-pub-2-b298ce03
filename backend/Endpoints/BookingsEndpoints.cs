using System.Text;
using Calvin.BookingService.Data;
using Calvin.BookingService.Domain;
using Calvin.BookingService.Http;

namespace Calvin.BookingService.Endpoints;

public static class BookingsEndpoints
{
    public static IEndpointRouteBuilder MapBookings(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookings").WithTags("Bookings");

        // CLVN-022/023: list bookings, optionally scoped to a user and filtered by
        // location/type/date/range/status, sorted chronologically (next first).
        // Cancelled bookings are hidden unless explicitly requested.
        group.MapGet("/", (CalvinDbContext db,
            string? userId, string? locationId, string? type,
            string? date, string? from, string? to,
            string? status, bool? includeCancelled, string? sort) =>
        {
            var bookings = db.Bookings.AsEnumerable();

            if (userId is not null) bookings = bookings.Where(b => b.UserId == userId);
            if (locationId is not null) bookings = bookings.Where(b => b.LocationId == locationId);
            if (type is not null) bookings = bookings.Where(b => b.Type == type);

            if (date is not null && DateOnly.TryParse(date, out var d))
                bookings = bookings.Where(b => b.Date == d);
            if (from is not null && DateOnly.TryParse(from, out var f))
                bookings = bookings.Where(b => b.Date >= f);
            if (to is not null && DateOnly.TryParse(to, out var t))
                bookings = bookings.Where(b => b.Date <= t);

            if (status is not null)
                bookings = bookings.Where(b => string.Equals(b.Status, status, StringComparison.OrdinalIgnoreCase));
            else if (includeCancelled != true)
                bookings = bookings.Where(b => b.Status == BookingStatus.Active);

            var ordered = sort == "-date"
                ? bookings.OrderByDescending(b => b.Date).ThenByDescending(b => b.TimeFrom ?? TimeOnly.MinValue)
                : bookings.OrderBy(b => b.Date).ThenBy(b => b.TimeFrom ?? TimeOnly.MinValue);

            return Results.Ok(ordered.Select(b => BookingMapper.ToResponse(b, db)).ToList());
        });

        // CLVN-024: full detail of a single booking.
        group.MapGet("/{id}", (string id, CalvinDbContext db) =>
            db.Bookings.FirstOrDefault(b => b.Id == id) is { } booking
                ? Results.Ok(BookingMapper.ToResponse(booking, db))
                : Problems.NotFound($"Booking '{id}' not found."));

        // CLVN-025: export a booking as an iCalendar (.ics) file.
        group.MapGet("/{id}/export", (string id, CalvinDbContext db) =>
        {
            var booking = db.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking is null) return Problems.NotFound($"Booking '{id}' not found.");

            var ics = BuildIcs(booking, db);
            return Results.File(Encoding.UTF8.GetBytes(ics), "text/calendar", $"{booking.Id}.ics");
        });

        // CLVN-016..020: create a booking. Validates input, prevents double bookings.
        group.MapPost("/", (BookingRequest request, CalvinDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Type))
                return Problems.Validation("Field 'type' is required.");
            if (string.IsNullOrWhiteSpace(request.ResourceId))
                return Problems.Validation("Field 'resourceId' is required.");
            if (string.IsNullOrWhiteSpace(request.UserId))
                return Problems.Validation("Field 'userId' is required.");

            var parseError = ParseBooking(
                request.Date, request.TimeFrom, request.TimeTo, request.Title, request.Notes,
                out var date, out var timeFrom, out var timeTo);
            if (parseError is not null) return parseError;

            if (BookingRules.FindConflict(db.Bookings, request.ResourceId, date, timeFrom, timeTo) is not null)
                return Problems.Conflict("Resource already booked for this time slot.");

            var booking = new Booking
            {
                Id = BookingRules.NextBookingId(db.Bookings),
                Type = request.Type,
                ResourceId = request.ResourceId,
                LocationId = request.LocationId,
                UserId = request.UserId,
                Date = date,
                TimeFrom = timeFrom,
                TimeTo = timeTo,
                Title = request.Title?.Trim() ?? "",
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes,
                Status = BookingStatus.Active,
            };

            db.Bookings.Add(booking);
            db.SaveChanges();

            var response = BookingMapper.ToResponse(booking, db);
            return Results.Created($"/api/bookings/{booking.Id}", response);
        });

        // CLVN-027: modify / reschedule a booking. Re-checks availability for the
        // new slot (excluding the booking itself). Only future bookings.
        group.MapPut("/{id}", (string id, BookingUpdateRequest request, CalvinDbContext db) =>
        {
            var booking = db.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking is null) return Problems.NotFound($"Booking '{id}' not found.");
            if (booking.Status == BookingStatus.Cancelled)
                return Problems.Conflict("Cancelled bookings cannot be modified.");
            if (IsPast(booking.Date))
                return Problems.Conflict("Past bookings cannot be modified.");

            if (string.IsNullOrWhiteSpace(request.Type))
                return Problems.Validation("Field 'type' is required.");
            if (string.IsNullOrWhiteSpace(request.ResourceId))
                return Problems.Validation("Field 'resourceId' is required.");

            var parseError = ParseBooking(
                request.Date, request.TimeFrom, request.TimeTo, request.Title, request.Notes,
                out var date, out var timeFrom, out var timeTo);
            if (parseError is not null) return parseError;

            if (BookingRules.FindConflict(db.Bookings, request.ResourceId, date, timeFrom, timeTo, excludeId: id) is not null)
                return Problems.Conflict("Resource already booked for this time slot.");

            booking.Type = request.Type;
            booking.ResourceId = request.ResourceId;
            booking.LocationId = request.LocationId;
            booking.Date = date;
            booking.TimeFrom = timeFrom;
            booking.TimeTo = timeTo;
            if (request.Title is not null) booking.Title = request.Title.Trim();
            booking.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes;

            db.SaveChanges();
            return Results.Ok(BookingMapper.ToResponse(booking, db));
        });

        // CLVN-026: cancel a booking (soft cancellation, keeps history). The slot
        // becomes free again because conflict checks ignore cancelled bookings.
        group.MapPost("/{id}/cancellation", (string id, CalvinDbContext db) =>
        {
            var booking = db.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking is null) return Problems.NotFound($"Booking '{id}' not found.");
            if (IsPast(booking.Date))
                return Problems.Conflict("Past bookings cannot be cancelled.");

            booking.Status = BookingStatus.Cancelled;
            db.SaveChanges();
            return Results.Ok(BookingMapper.ToResponse(booking, db));
        });

        // Hard delete (kept for the existing frontend deleteBooking call).
        group.MapDelete("/{id}", (string id, CalvinDbContext db) =>
        {
            var booking = db.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking is null) return Problems.NotFound($"Booking '{id}' not found.");
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return Results.NoContent();
        });

        return app;
    }

    private static bool IsPast(DateOnly date) => date < DateOnly.FromDateTime(DateTime.Now);

    // Parses/validates the date, time range, title and notes of a booking request.
    // Returns a problem result on failure, otherwise null with the out values set.
    private static IResult? ParseBooking(
        string dateStr, string? fromStr, string? toStr, string? title, string? notes,
        out DateOnly date, out TimeOnly? timeFrom, out TimeOnly? timeTo)
    {
        date = default;
        timeFrom = null;
        timeTo = null;

        if (!DateOnly.TryParse(dateStr, out date))
            return Problems.Validation("Invalid date format. Use ISO 8601 (yyyy-MM-dd).");
        if (IsPast(date))
            return Problems.Validation("Date must not be in the past.");

        if (title is { Length: > 100 })
            return Problems.Validation("Title must not exceed 100 characters.");
        if (notes is { Length: > 500 })
            return Problems.Validation("Note must not exceed 500 characters.");

        var hasFrom = fromStr is not null;
        var hasTo = toStr is not null;
        if (hasFrom != hasTo)
            return Problems.Validation("timeFrom and timeTo must be provided together (or both omitted for an all-day booking).");

        if (hasFrom)
        {
            if (!TimeOnly.TryParse(fromStr, out var tf))
                return Problems.Validation("Invalid timeFrom format. Use HH:mm.");
            if (!TimeOnly.TryParse(toStr, out var tt))
                return Problems.Validation("Invalid timeTo format. Use HH:mm.");
            if (tt <= tf)
                return Problems.Validation("timeTo must be after timeFrom.");
            timeFrom = tf;
            timeTo = tt;
        }

        return null;
    }

    private static string BuildIcs(Booking b, CalvinDbContext db)
    {
        var response = BookingMapper.ToResponse(b, db);
        var stamp = DateTime.Now.ToString("yyyyMMddTHHmmss");

        string dtStart, dtEnd;
        if (b.TimeFrom is { } tf && b.TimeTo is { } tt)
        {
            dtStart = $"DTSTART:{b.Date:yyyyMMdd}T{tf:HHmmss}";
            dtEnd = $"DTEND:{b.Date:yyyyMMdd}T{tt:HHmmss}";
        }
        else
        {
            // All-day event: DTEND is the day after (exclusive end per RFC 5545).
            dtStart = $"DTSTART;VALUE=DATE:{b.Date:yyyyMMdd}";
            dtEnd = $"DTEND;VALUE=DATE:{b.Date.AddDays(1):yyyyMMdd}";
        }

        var summary = string.IsNullOrWhiteSpace(b.Title) ? response.Resource : b.Title;
        var lines = new[]
        {
            "BEGIN:VCALENDAR",
            "VERSION:2.0",
            "PRODID:-//INNOQ//Calvin BookingService//DE",
            "BEGIN:VEVENT",
            $"UID:{b.Id}@calvin.innoq",
            $"DTSTAMP:{stamp}",
            dtStart,
            dtEnd,
            $"SUMMARY:{Escape(summary)}",
            $"LOCATION:{Escape($"{response.Resource}, {response.Location}")}",
            $"DESCRIPTION:{Escape(b.Notes ?? "")}",
            "END:VEVENT",
            "END:VCALENDAR",
        };
        return string.Join("\r\n", lines) + "\r\n";
    }

    private static string Escape(string value) =>
        value.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,")
             .Replace("\r\n", "\\n").Replace("\n", "\\n");
}
