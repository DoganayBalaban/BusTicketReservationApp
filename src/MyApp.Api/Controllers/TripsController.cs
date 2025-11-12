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
    public ActionResult<List<Trip>> GetAll()
    {
        return Ok(_repository.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Trip> GetById(string id)
    {
        var trip = _repository.GetById(id);
        if (trip == null)
            return NotFound();

        return Ok(trip);
    }

    [HttpGet("search")]
    public ActionResult<List<Trip>> Search(
        [FromQuery] string? departureCity,
        [FromQuery] string? arrivalCity,
        [FromQuery] DateTime? date)
    {
        var results = _repository.Search(departureCity, arrivalCity, date);
        return Ok(results);
    }
}
