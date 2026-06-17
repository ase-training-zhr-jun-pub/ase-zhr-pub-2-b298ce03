namespace Calvin.BookingService.Domain;

public record Workplace(
    string Id,
    string Name,
    string Floor,
    string[] Equipment,
    bool Occupied,
    string LocationId
);
