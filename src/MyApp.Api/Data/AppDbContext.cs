using Microsoft.EntityFrameworkCore;
using MyApp.Api.Models;

namespace MyApp.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Trip> Trips { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Trip configuration
        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Company).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DepartureCity).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ArrivalCity).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BusType).HasMaxLength(50);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.HasIndex(e => e.DepartureCity);
            entity.HasIndex(e => e.ArrivalCity);
            entity.HasIndex(e => e.DepartureTime);
        });

        // Reservation configuration
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TripId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PassengerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PassengerSurname).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.TCKimlik).HasMaxLength(11);
            entity.Property(e => e.ReservationCode).IsRequired().HasMaxLength(20);
            
            // Persist seat selections as comma-separated strings while keeping EF change tracking accurate
            entity.Property(e => e.Seats).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
            entity.HasIndex(e => e.TripId);
            entity.HasIndex(e => e.ReservationCode);
            
            // Relationship
            entity.HasOne<Trip>()
                .WithMany()
                .HasForeignKey(e => e.TripId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
