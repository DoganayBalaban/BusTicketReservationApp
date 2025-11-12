using System.Text.Json;
using MyApp.Api.Models;

namespace MyApp.Api.Services;

public class TripRepository
{
    private readonly List<Trip> _trips;

    public TripRepository()
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "trips.json");
        var jsonData = File.ReadAllText(jsonPath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _trips = JsonSerializer.Deserialize<List<Trip>>(jsonData, options) ?? new List<Trip>();
    }

    public List<Trip> GetAll() => _trips;

    public Trip? GetById(string id) => _trips.FirstOrDefault(t => t.Id == id);

    public List<Trip> Search(string? departureCity = null, string? arrivalCity = null, DateTime? date = null)
    {
        var query = _trips.AsQueryable();

        if (!string.IsNullOrEmpty(departureCity))
            query = query.Where(t => t.DepartureCity.Contains(departureCity, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(arrivalCity))
            query = query.Where(t => t.ArrivalCity.Contains(arrivalCity, StringComparison.OrdinalIgnoreCase));

        if (date.HasValue)
            query = query.Where(t => t.DepartureTime.Date == date.Value.Date);

        return query.OrderBy(t => t.DepartureTime).ToList();
    }
}
