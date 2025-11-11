
using Microsoft.AspNetCore.Mvc;
using MyApp.Api.Models;
using MyApp.Api.Services;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly TripRepository _repo;
    public TripsController(TripRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<List<Trip>>> Get() => await _repo.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Trip>> GetById(string id)
    {
        var trip = await _repo.GetByIdAsync(id);
        return trip is null ? NotFound() : trip;
    }
}
