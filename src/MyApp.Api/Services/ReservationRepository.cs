using MyApp.Api.Models;

namespace MyApp.Api.Services;

public class ReservationRepository
{
    private readonly List<Reservation> _reservations = new();

    public List<Reservation> GetAll() => _reservations;

    public Reservation? GetById(string id) => _reservations.FirstOrDefault(r => r.Id == id);

    public List<Reservation> GetByTripId(string tripId) => 
        _reservations.Where(r => r.TripId == tripId).ToList();

    public Reservation Add(Reservation reservation)
    {
        _reservations.Add(reservation);
        return reservation;
    }

    public bool Delete(string id)
    {
        var reservation = GetById(id);
        if (reservation == null) return false;
        
        _reservations.Remove(reservation);
        return true;
    }
}
