using Microsoft.EntityFrameworkCore;

namespace Calvin.BookingService.Data;

public static class DbSeeder
{
    // Creates the SQLite database file if it does not exist and seeds the
    // master data + demo bookings on first run. State persists across restarts.
    public static void EnsureSeeded(CalvinDbContext db)
    {
        db.Database.EnsureCreated();

        if (!db.Locations.Any())
            db.Locations.AddRange(SeedData.Locations());
        if (!db.ConferenceRooms.Any())
            db.ConferenceRooms.AddRange(SeedData.ConferenceRooms());
        if (!db.Workplaces.Any())
            db.Workplaces.AddRange(SeedData.Workplaces());
        if (!db.Users.Any())
            db.Users.AddRange(SeedData.Users());
        if (!db.Bookings.Any())
            db.Bookings.AddRange(SeedData.Bookings());

        db.SaveChanges();
    }
}
