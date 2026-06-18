namespace Calvin.BookingService.Domain;

// Occupied is a baseline availability flag used when no specific date is queried.
// When a date (and optionally a time range) is supplied, occupancy is computed
// from the active bookings instead.
public class ConferenceRoom
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int Capacity { get; set; }
    public List<string> Equipment { get; set; } = new();
    public bool Occupied { get; set; }
    public string LocationId { get; set; } = "";
}
