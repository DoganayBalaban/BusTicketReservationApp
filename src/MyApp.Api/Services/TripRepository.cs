using System.Text.Json;
using MyApp.Api.Models;

namespace MyApp.Api.Services;

public class TripRepository
{
    private readonly IWebHostEnvironment _env;
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public TripRepository(IWebHostEnvironment env) => _env = env;

    public async Task<List<Trip>> GetAllAsync()
    {
        var path = Path.Combine(_env.ContentRootPath, "Data", "trips.json");
        if (!File.Exists(path)) return new();

        await using var fs = File.OpenRead(path);
        var data = await JsonSerializer.DeserializeAsync<List<Trip>>(fs, _json);
        return data ?? new();
    }

    public async Task<Trip?> GetByIdAsync(string id)
        => (await GetAllAsync()).FirstOrDefault(t => t.Id == id);
}
