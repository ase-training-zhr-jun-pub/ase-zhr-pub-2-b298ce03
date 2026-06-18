using Calvin.BookingService.Domain;

namespace Calvin.BookingService.Data;

public static class BookingRules
{
    // Two time ranges overlap on the same day. A null TimeFrom means all-day
    // ("Ganztägig"), which conflicts with any other booking that day.
    public static bool Overlaps(TimeOnly? aFrom, TimeOnly? aTo, TimeOnly? bFrom, TimeOnly? bTo)
    {
        if (aFrom is null || aTo is null || bFrom is null || bTo is null) return true;
        return aFrom < bTo && bFrom < aTo;
    }

    // Returns the active booking conflicting with the candidate on the same
    // resource/date, ignoring the booking with id == excludeId (used by PUT).
    public static Booking? FindConflict(
        IEnumerable<Booking> bookings,
        string resourceId,
        DateOnly date,
        TimeOnly? timeFrom,
        TimeOnly? timeTo,
        string? excludeId = null)
    {
        return bookings.FirstOrDefault(b =>
            b.Status == BookingStatus.Active &&
            b.Id != excludeId &&
            b.ResourceId == resourceId &&
            b.Date == date &&
            Overlaps(b.TimeFrom, b.TimeTo, timeFrom, timeTo));
    }

    // Generates the next sequential booking id (CLVN-B-####).
    public static string NextBookingId(IEnumerable<Booking> bookings)
    {
        var max = bookings
            .Select(b => int.TryParse(b.Id.Split('-').LastOrDefault(), out var n) ? n : 0)
            .DefaultIfEmpty(1002)
            .Max();
        return $"CLVN-B-{max + 1}";
    }
}
