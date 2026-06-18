namespace Calvin.BookingService.Http;

// Create a booking (CLVN-016..020). Title/Notes are optional on the wire so the
// existing frontend keeps working; when supplied they are length-validated
// (Title <= 100, Notes <= 500, see CLVN-017/018).
public record BookingRequest(
    string Type,
    string ResourceId,
    string LocationId,
    string UserId,
    string Date,
    string? TimeFrom,
    string? TimeTo,
    string? Title = null,
    string? Notes = null
);

// Modify / reschedule an existing booking (CLVN-027). UserId and Status are not
// changeable here; availability is re-checked excluding the booking itself.
public record BookingUpdateRequest(
    string Type,
    string ResourceId,
    string LocationId,
    string Date,
    string? TimeFrom,
    string? TimeTo,
    string? Title = null,
    string? Notes = null
);
