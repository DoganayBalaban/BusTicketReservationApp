using System.Net.Http.Json;
using MyApp.Client.Models;

namespace MyApp.Client.Services;

public class ReservationService
{
    private readonly HttpClient _http;

    public ReservationService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ReservationDto>> GetAllReservationsAsync()
    {
        try
        {
            var reservations = await _http.GetFromJsonAsync<List<ReservationDto>>("api/reservations");
            return reservations ?? new List<ReservationDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            return new List<ReservationDto>();
        }
    }

    public async Task<List<ReservationDto>> GetReservationsByTripIdAsync(string tripId)
    {
        try
        {
            var reservations = await _http.GetFromJsonAsync<List<ReservationDto>>($"api/reservations/trip/{tripId}");
            return reservations ?? new List<ReservationDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            return new List<ReservationDto>();
        }
    }

    public async Task<List<string>> GetOccupiedSeatsAsync(string tripId)
    {
        try
        {
            var reservations = await GetReservationsByTripIdAsync(tripId);
            return reservations.SelectMany(r => r.Seats).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            return new List<string>();
        }
    }

    public async Task<ReservationDto?> CreateReservationAsync(ReservationDto reservation)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/reservations", reservation);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ReservationDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteReservationAsync(string id)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/reservations/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
            return false;
        }
    }
}
