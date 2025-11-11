using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using MyApp.Api.Models;

namespace MyApp.Api.Services;

public class ReservationRepository
{
    private readonly ConcurrentDictionary<string, List<Reservation>> _reservations = new();
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public ReservationRepository()
    {
        // Örnek dolu koltuklar (test için)
        _reservations.TryAdd("1", new List<Reservation>
        {
            new() { Id = "r1", TripId = "1", Seats = new List<string> { "1A", "1B" }, ReservationCode = "OBR-2025-001" },
            new() { Id = "r2", TripId = "1", Seats = new List<string> { "5C" }, ReservationCode = "OBR-2025-002" }
        });
        _reservations.TryAdd("2", new List<Reservation>
        {
            new() { Id = "r3", TripId = "2", Seats = new List<string> { "2A", "2B", "2C" }, ReservationCode = "OBR-2025-003" }
        });
    }

    public List<string> GetOccupiedSeats(string tripId)
    {
        if (!_reservations.TryGetValue(tripId, out var reservations))
            return new List<string>();

        return reservations.SelectMany(r => r.Seats).ToList();
    }

    public Reservation CreateReservation(Reservation reservation)
    {
        reservation.Id = Guid.NewGuid().ToString();
        reservation.ReservationDate = DateTime.UtcNow;
        reservation.ReservationCode = $"OBR-{DateTime.UtcNow.Year}-{Random.Shared.Next(100, 999)}";

        _reservations.AddOrUpdate(
            reservation.TripId,
            new List<Reservation> { reservation },
            (key, existing) =>
            {
                existing.Add(reservation);
                return existing;
            });

        return reservation;
    }

    public Reservation? GetReservationByCode(string code)
    {
        return _reservations.Values
            .SelectMany(r => r)
            .FirstOrDefault(r => r.ReservationCode == code);
    }
}

