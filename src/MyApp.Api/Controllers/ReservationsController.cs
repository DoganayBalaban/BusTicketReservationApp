using Microsoft.AspNetCore.Mvc;
using MyApp.Api.Models;
using MyApp.Api.Services;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ReservationRepository _repo;
    public ReservationsController(ReservationRepository repo) => _repo = repo;

    [HttpGet("trip/{tripId}/occupied-seats")]
    public ActionResult<List<string>> GetOccupiedSeats(string tripId)
        => _repo.GetOccupiedSeats(tripId);

    [HttpPost]
    public ActionResult<Reservation> CreateReservation([FromBody] Reservation reservation)
    {
        // Koltukların dolu olup olmadığını kontrol et
        var occupiedSeats = _repo.GetOccupiedSeats(reservation.TripId);
        var conflictingSeats = reservation.Seats.Where(s => occupiedSeats.Contains(s)).ToList();
        
        if (conflictingSeats.Any())
        {
            return BadRequest(new { message = $"Bu koltuklar zaten rezerve edilmiş: {string.Join(", ", conflictingSeats)}" });
        }

        var created = _repo.CreateReservation(reservation);
        return CreatedAtAction(nameof(GetByCode), new { code = created.ReservationCode }, created);
    }

    [HttpGet("code/{code}")]
    public ActionResult<Reservation> GetByCode(string code)
    {
        var reservation = _repo.GetReservationByCode(code);
        return reservation is null ? NotFound() : reservation;
    }
}

