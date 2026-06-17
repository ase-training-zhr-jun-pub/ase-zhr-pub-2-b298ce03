namespace Calvin.BookingService.Http;

public record BookingRequest(
    string Type,
    string ResourceId,
    string LocationId,
    string UserId,
    string Date,
    string? TimeFrom,
    string? TimeTo
);
