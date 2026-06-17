namespace Calvin.BookingService.Http;

public record BookingResponse(
    string Id,
    string Type,
    string Resource,
    string Location,
    string Date,
    string TimeRange
);
