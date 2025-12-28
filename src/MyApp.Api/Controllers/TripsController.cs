using Microsoft.AspNetCore.Mvc;
using MyApp.Api.Models;
using MyApp.Api.Services;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly TripRepository _repository;

    public TripsController(TripRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Trip>>> GetAll()
    {
        // Present trips ordered by departure time for predictable UI listing
        var trips = await _repository.GetAllAsync();
        return Ok(trips);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Trip>> GetById(string id)
    {
        // Early return 404 to avoid null payloads
        var trip = await _repository.GetByIdAsync(id);
        if (trip == null)
            return NotFound();

        return Ok(trip);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<Trip>>> Search(
        [FromQuery] string? departureCity,
        [FromQuery] string? arrivalCity,
        [FromQuery] DateTime? date)
    {
        // Search with optional filters; repository normalizes date to UTC
        var results = await _repository.SearchAsync(departureCity, arrivalCity, date);
        return Ok(results);
    }
}
