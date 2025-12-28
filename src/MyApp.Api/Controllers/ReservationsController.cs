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
    public async Task<ActionResult<List<Reservation>>> GetAll()
    {
        // Return latest reservations so the newest bookings are shown first
        var reservations = await _repository.GetAllAsync();
        return Ok(reservations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Reservation>> GetById(string id)
    {
        // Short-circuit with 404 if the reservation does not exist
        var reservation = await _repository.GetByIdAsync(id);
        if (reservation == null)
            return NotFound();

        return Ok(reservation);
    }

    [HttpGet("trip/{tripId}")]
    public async Task<ActionResult<List<Reservation>>> GetByTripId(string tripId)
    {
        // Limit results to a specific trip to simplify seat management
        var reservations = await _repository.GetByTripIdAsync(tripId);
        return Ok(reservations);
    }

    [HttpPost]
    public async Task<ActionResult<Reservation>> Create(Reservation reservation)
    {
        // Repository handles id/code generation and UTC normalization
        var created = await _repository.AddAsync(reservation);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        // Avoid silent failures; respond with 404 when nothing is removed
        var success = await _repository.DeleteAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
