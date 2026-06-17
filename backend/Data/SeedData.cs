using Calvin.BookingService.Domain;

namespace Calvin.BookingService.Data;

public static class SeedData
{
    public static List<Location> Locations() =>
    [
        new("koeln",     "Köln"),
        new("berlin",    "Berlin"),
        new("hamburg",   "Hamburg"),
        new("monheim",   "Monheim"),
        new("muenchen",  "München"),
        new("offenbach", "Offenbach"),
        new("zuerich",   "Zürich"),
        new("baar",      "Baar"),
    ];

    public static List<ConferenceRoom> ConferenceRooms() =>
    [
        new("KR-RHEINBLICK", "Konferenzraum Rheinblick", 12,
            ["Videokonferenz", "Whiteboard", "Beamer"], false, "koeln"),
        new("KR-DOM", "Konferenzraum Dom", 6,
            ["Whiteboard"], false, "koeln"),
        new("KR-HEUMARKT", "Konferenzraum Heumarkt", 8,
            ["Videokonferenz", "Bildschirm"], false, "koeln"),
        new("KR-BERLIN-1", "Konferenzraum Spree", 10,
            ["Videokonferenz", "Whiteboard"], false, "berlin"),
        new("KR-HH-1", "Konferenzraum Alster", 8,
            ["Beamer", "Whiteboard"], false, "hamburg"),
        new("KR-MUC-1", "Konferenzraum Isar", 12,
            ["Videokonferenz", "Beamer"], false, "muenchen"),
    ];

    public static List<Workplace> Workplaces() =>
    [
        new("AP-01", "Fensterplatz Nord", "Etage 1",
            ["Höhenverstellbarer Tisch", "Fensterplatz"], false, "koeln"),
        new("AP-02", "Teambereich", "Etage 1",
            ["Dockingstation"], true, "koeln"),
        new("AP-03", "Ruhezone", "Etage 2",
            ["2 Monitore", "Höhenverstellbarer Tisch"], false, "koeln"),
        new("AP-04", "Fokusplatz", "Etage 2",
            ["2 Monitore", "Dockingstation"], false, "koeln"),
        new("AP-05", "Gemeinschaftstisch", "Etage 2",
            ["Großer Bildschirm"], true, "koeln"),
        new("AP-06", "Einzelplatz Süd", "Etage 3",
            ["Höhenverstellbarer Tisch", "Dockingstation"], false, "koeln"),
    ];

    public static List<Booking> Bookings() =>
    [
        new("CLVN-B-1001", "Workplace", "AP-03", "koeln", "alice",
            new DateOnly(2026, 6, 18), null, null),
        new("CLVN-B-1002", "ConferenceRoom", "KR-RHEINBLICK", "koeln", "alice",
            new DateOnly(2026, 6, 19),
            new TimeOnly(10, 0), new TimeOnly(11, 30)),
    ];

    public static List<User> Users() =>
    [
        new("alice",   "Alice Müller"),
        new("bob",     "Bob Schmidt"),
        new("charlie", "Charlie Weber"),
    ];
}
