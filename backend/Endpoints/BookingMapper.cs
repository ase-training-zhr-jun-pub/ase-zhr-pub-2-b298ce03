using Calvin.BookingService.Data;
using Calvin.BookingService.Domain;
using Calvin.BookingService.Http;

namespace Calvin.BookingService.Endpoints;

public static class BookingMapper
{
    public static BookingResponse ToResponse(Booking b, CalvinDbContext db)
    {
        var (resourceName, equipment) = ResolveResource(b, db);
        var location = db.Locations.FirstOrDefault(l => l.Id == b.LocationId)?.Name ?? b.LocationId;

        var dateDisplay = b.Date.ToString("dd.MM.yyyy");
        var dateIso = b.Date.ToString("yyyy-MM-dd");
        var timeFrom = b.TimeFrom?.ToString("HH:mm");
        var timeTo = b.TimeTo?.ToString("HH:mm");
        var timeRange = b.TimeFrom is null ? "Ganztägig" : $"{timeFrom} – {timeTo}";
        var isPast = b.Date < DateOnly.FromDateTime(DateTime.Now);

        return new BookingResponse(
            b.Id, b.Type, resourceName, location, dateDisplay, timeRange,
            b.ResourceId, b.LocationId, b.UserId, dateIso, timeFrom, timeTo,
            b.Title, b.Notes, b.Status, isPast, equipment);
    }

    private static (string Name, string[] Equipment) ResolveResource(Booking b, CalvinDbContext db)
    {
        if (b.Type == BookingType.Workplace)
        {
            var wp = db.Workplaces.FirstOrDefault(w => w.Id == b.ResourceId);
            return wp is null
                ? (b.ResourceId, [])
                : ($"{wp.Id} · {wp.Name}", wp.Equipment.ToArray());
        }

        var room = db.ConferenceRooms.FirstOrDefault(r => r.Id == b.ResourceId);
        return room is null
            ? (b.ResourceId, [])
            : (room.Name, room.Equipment.ToArray());
    }
}
