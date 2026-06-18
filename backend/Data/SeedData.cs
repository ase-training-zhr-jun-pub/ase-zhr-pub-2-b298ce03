using Calvin.BookingService.Domain;

namespace Calvin.BookingService.Data;

// Realistic INNOQ master data. Kept consistent with the frontend mock data
// (frontend/src/lib/mock-data.ts) so the wired app shows the same resources.
public static class SeedData
{
    public static List<Location> Locations() =>
    [
        new() { Id = "koeln",     Name = "Köln" },
        new() { Id = "berlin",    Name = "Berlin" },
        new() { Id = "hamburg",   Name = "Hamburg" },
        new() { Id = "monheim",   Name = "Monheim" },
        new() { Id = "muenchen",  Name = "München" },
        new() { Id = "offenbach", Name = "Offenbach" },
        new() { Id = "zuerich",   Name = "Zürich" },
        new() { Id = "baar",      Name = "Baar" },
    ];

    public static List<ConferenceRoom> ConferenceRooms() =>
    [
        new() { Id = "KR-KOE-01", Name = "Rheinblick",    Capacity = 8,
                Equipment = ["Bildschirm", "Whiteboard", "Videokonferenz"], Occupied = false, LocationId = "koeln" },
        new() { Id = "KR-KOE-02", Name = "Stadtgarten",   Capacity = 4,
                Equipment = ["Bildschirm", "Flipchart"], Occupied = true, LocationId = "koeln" },
        new() { Id = "KR-BER-01", Name = "Spreebogen",    Capacity = 12,
                Equipment = ["Bildschirm", "Whiteboard", "Videokonferenz"], Occupied = false, LocationId = "berlin" },
        new() { Id = "KR-BER-02", Name = "Brandenburger", Capacity = 6,
                Equipment = ["Bildschirm", "Flipchart", "Whiteboard"], Occupied = false, LocationId = "berlin" },
        new() { Id = "KR-HAM-01", Name = "Speicherstadt", Capacity = 10,
                Equipment = ["Bildschirm", "Videokonferenz"], Occupied = true, LocationId = "hamburg" },
        new() { Id = "KR-MUC-01", Name = "Isar",          Capacity = 8,
                Equipment = ["Bildschirm", "Whiteboard"], Occupied = false, LocationId = "muenchen" },
    ];

    public static List<Workplace> Workplaces() =>
    [
        new() { Id = "AP-01", Name = "Fensterplatz Nord",  Floor = "Etage 1",
                Equipment = ["Höhenverstellbarer Tisch", "Fensterplatz"], Occupied = false, LocationId = "koeln" },
        new() { Id = "AP-02", Name = "Teambereich",        Floor = "Etage 1",
                Equipment = ["Dockingstation"], Occupied = true, LocationId = "koeln" },
        new() { Id = "AP-03", Name = "Ruhezone",           Floor = "Etage 2",
                Equipment = ["2 Monitore", "Höhenverstellbarer Tisch"], Occupied = false, LocationId = "koeln" },
        new() { Id = "AP-04", Name = "Fokusplatz",         Floor = "Etage 2",
                Equipment = ["2 Monitore", "Dockingstation"], Occupied = false, LocationId = "koeln" },
        new() { Id = "AP-05", Name = "Gemeinschaftstisch", Floor = "Etage 2",
                Equipment = ["Großer Bildschirm"], Occupied = true, LocationId = "koeln" },
        new() { Id = "AP-06", Name = "Einzelplatz Süd",    Floor = "Etage 3",
                Equipment = ["Höhenverstellbarer Tisch", "Dockingstation"], Occupied = false, LocationId = "koeln" },
    ];

    public static List<Booking> Bookings() =>
    [
        new()
        {
            Id = "CLVN-B-1001", Type = BookingType.Workplace, ResourceId = "AP-03",
            LocationId = "koeln", UserId = "alice", Date = new DateOnly(2026, 6, 18),
            TimeFrom = null, TimeTo = null, Title = "Bürotag Köln", Status = BookingStatus.Active,
        },
        new()
        {
            Id = "CLVN-B-1002", Type = BookingType.ConferenceRoom, ResourceId = "KR-KOE-01",
            LocationId = "koeln", UserId = "alice", Date = new DateOnly(2026, 6, 19),
            TimeFrom = new TimeOnly(10, 0), TimeTo = new TimeOnly(11, 30),
            Title = "Team-Sync", Notes = "Bitte Whiteboard freihalten.", Status = BookingStatus.Active,
        },
    ];

    public static List<User> Users() =>
    [
        new() { Id = "alice",   Name = "Alice Müller" },
        new() { Id = "bob",     Name = "Bob Schmidt" },
        new() { Id = "charlie", Name = "Charlie Weber" },
    ];
}
