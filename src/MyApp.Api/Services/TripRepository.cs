using Microsoft.EntityFrameworkCore;
using MyApp.Api.Data;
using MyApp.Api.Models;

namespace MyApp.Api.Services;

public class TripRepository
{
    private readonly AppDbContext _context;

    public TripRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Trip>> GetAllAsync()
    {
        // Sort by departure time to keep listings chronologically ordered
        return await _context.Trips.OrderBy(t => t.DepartureTime).ToListAsync();
    }

    public async Task<Trip?> GetByIdAsync(string id)
    {
        // Allow controller to handle missing trips gracefully
        return await _context.Trips.FindAsync(id);
    }

    public async Task<List<Trip>> SearchAsync(string? departureCity = null, string? arrivalCity = null, DateTime? date = null)
    {
        var query = _context.Trips.AsQueryable();

        if (!string.IsNullOrEmpty(departureCity))
            query = query.Where(t => t.DepartureCity.Contains(departureCity));

        if (!string.IsNullOrEmpty(arrivalCity))
            query = query.Where(t => t.ArrivalCity.Contains(arrivalCity));

        if (date.HasValue)
        {
            // Date'i UTC'ye çevir; böylece tarih karşılaştırmaları tutarlı olur
            var dateUtc = date.Value;
            if (dateUtc.Kind == DateTimeKind.Local)
            {
                dateUtc = dateUtc.ToUniversalTime();
            }
            else if (dateUtc.Kind == DateTimeKind.Unspecified)
            {
                dateUtc = DateTime.SpecifyKind(dateUtc, DateTimeKind.Utc);
            }
            
            var dateOnly = dateUtc.Date;
            query = query.Where(t => t.DepartureTime.Date == dateOnly);
        }

        return await query.OrderBy(t => t.DepartureTime).ToListAsync();
    }

    public async Task<Trip> AddAsync(Trip trip)
    {
        // Generate missing identifier values
        if (string.IsNullOrEmpty(trip.Id))
            trip.Id = Guid.NewGuid().ToString();
        
        // DateTime'ı UTC'ye çevir (PostgreSQL için gerekli ve aramalarla uyumlu)
        if (trip.DepartureTime.Kind == DateTimeKind.Unspecified)
        {
            trip.DepartureTime = DateTime.SpecifyKind(trip.DepartureTime, DateTimeKind.Utc);
        }
        else if (trip.DepartureTime.Kind == DateTimeKind.Local)
        {
            trip.DepartureTime = trip.DepartureTime.ToUniversalTime();
        }
        
        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();
        return trip;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var trip = await GetByIdAsync(id);
        if (trip == null) return false;
        
        _context.Trips.Remove(trip);
        await _context.SaveChangesAsync();
        return true;
    }
}
