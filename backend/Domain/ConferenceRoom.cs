namespace Calvin.BookingService.Domain;

public record ConferenceRoom(
    string Id,
    string Name,
    int Capacity,
    string[] Equipment,
    bool Occupied,
    string LocationId
);
