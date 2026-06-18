using Calvin.BookingService.Domain;
using Microsoft.EntityFrameworkCore;

namespace Calvin.BookingService.Data;

// EF Core context backed by a file-based SQLite database (calvin.db).
// String collections (Equipment) are mapped as EF Core primitive collections
// (stored as JSON in a single column), so no explicit value converter is needed.
public class CalvinDbContext(DbContextOptions<CalvinDbContext> options) : DbContext(options)
{
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<ConferenceRoom> ConferenceRooms => Set<ConferenceRoom>();
    public DbSet<Workplace> Workplaces => Set<Workplace>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Equipment (List<string>) is mapped by convention as an EF Core primitive
        // collection (stored as JSON in a single column) — no converter required.
        modelBuilder.Entity<Location>().HasKey(l => l.Id);
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<ConferenceRoom>().HasKey(r => r.Id);
        modelBuilder.Entity<Workplace>().HasKey(w => w.Id);

        modelBuilder.Entity<Booking>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.Title).HasMaxLength(100);
            e.Property(b => b.Notes).HasMaxLength(500);
            e.Property(b => b.Status).HasMaxLength(20);
        });
    }
}
