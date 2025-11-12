using Microsoft.AspNetCore.Mvc;
using MyApp.Api.Models;
using MyApp.Api.Services;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ReservationRepository _repository;

    public ReservationsController(ReservationRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public ActionResult<List<Reservation>> GetAll()
    {
        return Ok(_repository.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Reservation> GetById(string id)
    {
        var reservation = _repository.GetById(id);
        if (reservation == null)
            return NotFound();

        return Ok(reservation);
    }

    [HttpGet("trip/{tripId}")]
    public ActionResult<List<Reservation>> GetByTripId(string tripId)
    {
        return Ok(_repository.GetByTripId(tripId));
    }

    [HttpPost]
    public ActionResult<Reservation> Create(Reservation reservation)
    {
        // Id ve ReservationCode oluştur
        if (string.IsNullOrEmpty(reservation.Id))
            reservation.Id = Guid.NewGuid().ToString();
        
        if (string.IsNullOrEmpty(reservation.ReservationCode))
            reservation.ReservationCode = Guid.NewGuid().ToString("N")[..8].ToUpper();
        
        reservation.ReservationDate = DateTime.Now;
        
        var created = _repository.Add(reservation);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(string id)
    {
        var success = _repository.Delete(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
