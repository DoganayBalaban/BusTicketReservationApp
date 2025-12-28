using Microsoft.EntityFrameworkCore;
using MyApp.Api.Data;
using MyApp.Api.Models;

namespace MyApp.Api.Services;

public class ReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Reservation>> GetAllAsync()
    {
        // Keep latest reservations first to make recent activity visible
        return await _context.Reservations
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync();
    }

    public async Task<Reservation?> GetByIdAsync(string id)
    {
        // Single-key lookup; returns null when not found to let controller handle 404
        return await _context.Reservations.FindAsync(id);
    }

    public async Task<List<Reservation>> GetByTripIdAsync(string tripId)
    {
        // Useful for seat availability checks on a specific trip
        return await _context.Reservations
            .Where(r => r.TripId == tripId)
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync();
    }

    public async Task<Reservation> AddAsync(Reservation reservation)
    {
        // Generate identifiers when caller did not supply them
        if (string.IsNullOrEmpty(reservation.Id))
            reservation.Id = Guid.NewGuid().ToString();
        
        if (string.IsNullOrEmpty(reservation.ReservationCode))
            reservation.ReservationCode = Guid.NewGuid().ToString("N")[..8].ToUpper();
        
        // Normalize timestamps to UTC so PostgreSQL time zone handling stays consistent
        if (reservation.ReservationDate == default)
        {
            reservation.ReservationDate = DateTime.UtcNow;
        }
        else
        {
            // DateTime'ı UTC'ye çevir (PostgreSQL için gerekli)
            if (reservation.ReservationDate.Kind == DateTimeKind.Unspecified)
            {
                reservation.ReservationDate = DateTime.SpecifyKind(reservation.ReservationDate, DateTimeKind.Utc);
            }
            else if (reservation.ReservationDate.Kind == DateTimeKind.Local)
            {
                reservation.ReservationDate = reservation.ReservationDate.ToUniversalTime();
            }
        }

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
        return reservation;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var reservation = await GetByIdAsync(id);
        if (reservation == null) return false;
        
        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return true;
    }
}
