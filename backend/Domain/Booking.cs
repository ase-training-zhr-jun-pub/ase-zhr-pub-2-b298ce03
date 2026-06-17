namespace Calvin.BookingService.Domain;

public record Booking(
    string Id,
    string Type,
    string ResourceId,
    string LocationId,
    string UserId,
    DateOnly Date,
    TimeOnly? TimeFrom,
    TimeOnly? TimeTo
);
