namespace Calvin.BookingService.Domain;

// A booking of a conference room or a workplace by a user for a given date and
// (optional) time range. A null TimeFrom/TimeTo means all-day ("Ganztägig").
// Status is "Active" or "Cancelled" (soft cancellation keeps the history; see CLVN-026/027).
public class Booking
{
    public string Id { get; set; } = "";
    public string Type { get; set; } = "";          // "ConferenceRoom" | "Workplace"
    public string ResourceId { get; set; } = "";
    public string LocationId { get; set; } = "";
    public string UserId { get; set; } = "";
    public DateOnly Date { get; set; }
    public TimeOnly? TimeFrom { get; set; }
    public TimeOnly? TimeTo { get; set; }
    public string Title { get; set; } = "";          // Meetingtitel (CLVN-018)
    public string? Notes { get; set; }               // Buchungsnotiz (CLVN-017)
    public string Status { get; set; } = BookingStatus.Active;
}

public static class BookingStatus
{
    public const string Active = "Active";
    public const string Cancelled = "Cancelled";
}

public static class BookingType
{
    public const string ConferenceRoom = "ConferenceRoom";
    public const string Workplace = "Workplace";
}
