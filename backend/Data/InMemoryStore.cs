using Calvin.BookingService.Domain;

namespace Calvin.BookingService.Data;

public class InMemoryStore
{
    public List<Location> Locations { get; } = SeedData.Locations();
    public List<ConferenceRoom> ConferenceRooms { get; } = SeedData.ConferenceRooms();
    public List<Workplace> Workplaces { get; } = SeedData.Workplaces();
    public List<Booking> Bookings { get; } = SeedData.Bookings();
    public List<User> Users { get; } = SeedData.Users();

    private readonly object _lock = new();
    private int _bookingCounter = 1003;

    public string NextBookingId()
    {
        lock (_lock)
        {
            return $"CLVN-B-{_bookingCounter++}";
        }
    }

    // Returns the new booking, or null if a conflict was detected.
    public Booking? TryAddBooking(Booking booking)
    {
        lock (_lock)
        {
            bool conflict = Bookings.Any(b =>
                b.ResourceId == booking.ResourceId &&
                b.Date == booking.Date &&
                TimeRangesOverlap(b, booking));

            if (conflict) return null;
            Bookings.Add(booking);
            return booking;
        }
    }

    public bool RemoveBooking(string id)
    {
        lock (_lock)
        {
            var booking = Bookings.Find(b => b.Id == id);
            if (booking is null) return false;
            Bookings.Remove(booking);
            return true;
        }
    }

    // Checks whether two bookings overlap on the same day.
    // A null TimeFrom/TimeTo means all-day (Ganztag), which overlaps with anything.
    private static bool TimeRangesOverlap(Booking a, Booking b)
    {
        if (a.TimeFrom is null || b.TimeFrom is null) return true;
        return a.TimeFrom < b.TimeTo && b.TimeFrom < a.TimeTo;
    }
}
