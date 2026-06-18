namespace Calvin.BookingService.Http;

// Response shapes mirror the frontend types in frontend/src/lib/api.ts.
// `occupied` is computed per requested date/time when those query params are
// present, otherwise it falls back to the resource's baseline flag.

public record ConferenceRoomResponse(
    string Id,
    string Name,
    int Capacity,
    string[] Equipment,
    bool Occupied,
    string LocationId
);

public record WorkplaceResponse(
    string Id,
    string Name,
    string Floor,
    string[] Equipment,
    bool Occupied,
    string LocationId
);

// Availability check for a room/date/time (CLVN-010..012).
public record AvailabilityResponse(
    bool Available,
    BookingResponse[] Conflicts,
    TimeSlotResponse[] Alternatives
);

public record TimeSlotResponse(string TimeFrom, string TimeTo);
