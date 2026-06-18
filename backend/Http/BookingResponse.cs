namespace Calvin.BookingService.Http;

// The first six fields are the original, display-oriented contract consumed by
// the frontend (id, type, resource, location, date, timeRange). The remaining
// fields are additive (ignored by older clients) and power the detail view,
// booking confirmation, editing and .ics export (CLVN-020/024/025/027).
public record BookingResponse(
    string Id,
    string Type,
    string Resource,
    string Location,
    string Date,        // display: dd.MM.yyyy
    string TimeRange,   // display: "Ganztägig" | "10:00 – 11:30"
    string ResourceId,
    string LocationId,
    string UserId,
    string DateIso,     // yyyy-MM-dd
    string? TimeFrom,   // HH:mm | null (all-day)
    string? TimeTo,     // HH:mm | null (all-day)
    string Title,
    string? Notes,
    string Status,      // "Active" | "Cancelled"
    bool IsPast,
    string[] Equipment
);
