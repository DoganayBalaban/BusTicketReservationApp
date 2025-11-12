using System.Net.Http.Json;
using MyApp.Client.Models;

namespace MyApp.Client.Services;

public class TripService
{
    private readonly HttpClient _http;

    public TripService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<TripDto>> GetTripsAsync()
    {
        try
        {
            var trips = await _http.GetFromJsonAsync<List<TripDto>>("api/trips");
            return trips ?? new List<TripDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            return new List<TripDto>();
        }
    }

    public async Task<TripDto?> GetTripByIdAsync(string id)
    {
        try
        {
            return await _http.GetFromJsonAsync<TripDto>($"api/trips/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            return null;
        }
    }

    public async Task<List<TripDto>> SearchTripsAsync(string? departureCity, string? arrivalCity, DateTime? date)
    {
        try
        {
            var query = new List<string>();
            if (!string.IsNullOrEmpty(departureCity))
                query.Add($"departureCity={Uri.EscapeDataString(departureCity)}");
            if (!string.IsNullOrEmpty(arrivalCity))
                query.Add($"arrivalCity={Uri.EscapeDataString(arrivalCity)}");
            if (date.HasValue)
                query.Add($"date={date.Value:yyyy-MM-dd}");

            var queryString = query.Count > 0 ? "?" + string.Join("&", query) : "";
            var trips = await _http.GetFromJsonAsync<List<TripDto>>($"api/trips/search{queryString}");
            return trips ?? new List<TripDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            return new List<TripDto>();
        }
    }
}
