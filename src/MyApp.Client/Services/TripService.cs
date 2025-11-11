using System.Net.Http.Json;
using MyApp.Client.Models;

namespace MyApp.Client.Services;

public class TripService
{
    private readonly HttpClient _http;
    public TripService(HttpClient http) => _http = http;

    public async Task<List<TripDto>> GetTripsAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<TripDto>>("api/trips") ?? new();
        }
        catch
        {
            return new List<TripDto>();
        }
    }

    public async Task<TripDto?> GetTripByIdAsync(string id)
    {
        try
        {
            return await _http.GetFromJsonAsync<TripDto>($"api/trips/{id}");
        }
        catch
        {
            return null;
        }
    }
}
