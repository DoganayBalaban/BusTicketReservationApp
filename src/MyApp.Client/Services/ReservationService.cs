using System.Net.Http.Json;
using MyApp.Client.Models;

namespace MyApp.Client.Services;

public class ReservationService
{
    private readonly HttpClient _http;

    public ReservationService(HttpClient http) => _http = http;

    public async Task<List<string>> GetOccupiedSeatsAsync(string tripId)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<List<string>>($"api/reservations/trip/{tripId}/occupied-seats");
            return result ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public async Task<ReservationDto?> CreateReservationAsync(ReservationDto reservation)
    {
        var response = await _http.PostAsJsonAsync("api/reservations", reservation);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ReservationDto>();
        }
        return null;
    }

    public async Task<ReservationDto?> GetReservationByCodeAsync(string code)
    {
        try
        {
            return await _http.GetFromJsonAsync<ReservationDto>($"api/reservations/code/{code}");
        }
        catch
        {
            return null;
        }
    }
}

