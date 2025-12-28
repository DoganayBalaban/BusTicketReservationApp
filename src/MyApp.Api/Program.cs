using Microsoft.EntityFrameworkCore;
using MyApp.Api.Data;
using MyApp.Api.Models;
using MyApp.Api.Services;
using DotNetEnv;
using System.Text.Json;

// .env dosyasını yükle - önce API dizininde, sonra root dizinde dene
var envPath = Path.Combine(AppContext.BaseDirectory, ".env");
if (!File.Exists(envPath))
{
    // API projesinin root dizininde dene
    envPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".env");
    if (!File.Exists(envPath))
    {
        // Mevcut dizinde dene
        envPath = ".env";
    }
}
Env.Load(envPath);

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL connection string'i .env'den al
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(databaseUrl))
{
    throw new InvalidOperationException("DATABASE_URL environment variable is not set");
}

// PostgreSQL connection string formatını parse et
// Format: postgresql://user:password@host:port/database
var connectionString = ParseConnectionString(databaseUrl);

// DbContext'i ekle
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<TripRepository>();
builder.Services.AddScoped<ReservationRepository>();

const string allowClient = "_allowClient";
builder.Services.AddCors(o =>
{
    o.AddPolicy(allowClient, p =>
        p.WithOrigins("http://localhost:5085", "https://localhost:7223", "http://localhost:5000", "https://localhost:5000") // Blazor portları
         .AllowAnyHeader()
         .AllowAnyMethod());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(allowClient);

// Ana route için bilgi sayfası
app.MapGet("/", () => Results.Json(new
{
    message = "Otobüs Bileti Rezervasyon API'si",
    version = "1.0",
    endpoints = new
    {
        swagger = "/swagger",
        trips = "/api/trips",
        reservations = "/api/reservations"
    }
}));

app.MapControllers();

// Database migration'ını otomatik uygula ve seed data ekle (development için)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.Migrate();
        
        // Seed data: Eğer database boşsa trips.json'dan verileri yükle
        var tripCount = dbContext.Trips.Count();
        if (tripCount == 0)
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "trips.json");
            if (!File.Exists(jsonPath))
            {
                // Bir üst dizinde de dene
                jsonPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Data", "trips.json");
            }
            
            if (File.Exists(jsonPath))
            {
                var jsonData = File.ReadAllText(jsonPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var trips = JsonSerializer.Deserialize<List<Trip>>(jsonData, options);
                
                if (trips != null && trips.Any())
                {
                    // ChangeTracker'ı temizle (önceki track edilmiş entity'ler varsa)
                    dbContext.ChangeTracker.Clear();
                    
                    // Mevcut ID'leri al (idempotent seed için)
                    var existingIds = dbContext.Trips.Select(t => t.Id).ToHashSet();
                    var tripsToAdd = new List<Trip>();
                    var seenIds = new HashSet<string>();
                    
                    foreach (var trip in trips)
                    {
                        // JSON içinde duplicate ID kontrolü
                        if (seenIds.Contains(trip.Id))
                        {
                            Console.WriteLine($"Uyarı: JSON'da duplicate ID bulundu: {trip.Id}, atlanıyor.");
                            continue;
                        }
                        seenIds.Add(trip.Id);
                        
                        // Database'de zaten varsa atla (idempotent seed)
                        if (existingIds.Contains(trip.Id))
                        {
                            continue;
                        }
                        
                        // DateTime'ı UTC'ye çevir (PostgreSQL için gerekli)
                        if (trip.DepartureTime.Kind == DateTimeKind.Unspecified)
                        {
                            // Unspecified ise UTC olarak kabul et
                            trip.DepartureTime = DateTime.SpecifyKind(trip.DepartureTime, DateTimeKind.Utc);
                        }
                        else if (trip.DepartureTime.Kind == DateTimeKind.Local)
                        {
                            trip.DepartureTime = trip.DepartureTime.ToUniversalTime();
                        }
                        // Zaten UTC ise olduğu gibi bırak
                        
                        tripsToAdd.Add(trip);
                    }
                    
                    if (tripsToAdd.Any())
                    {
                        dbContext.Trips.AddRange(tripsToAdd);
                        dbContext.SaveChanges();
                        Console.WriteLine($"{tripsToAdd.Count} trip verisi database'e eklendi.");
                    }
                    else
                    {
                        Console.WriteLine("Tüm trip verileri zaten database'de mevcut.");
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        // Migration zaten uygulanmış olabilir veya database yoksa oluşturulabilir
        Console.WriteLine($"Migration hatası (normal olabilir): {ex.Message}");
        try
        {
            dbContext.Database.EnsureCreated();
            
            // Seed data
            var tripCountFallback = dbContext.Trips.Count();
            if (tripCountFallback == 0)
            {
                var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "trips.json");
                if (!File.Exists(jsonPath))
                {
                    jsonPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Data", "trips.json");
                }
                
                if (File.Exists(jsonPath))
                {
                    var jsonData = File.ReadAllText(jsonPath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var trips = JsonSerializer.Deserialize<List<Trip>>(jsonData, options);
                    
                    if (trips != null && trips.Any())
                    {
                        // ChangeTracker'ı temizle
                        dbContext.ChangeTracker.Clear();
                        
                        // Mevcut ID'leri al
                        var existingIds = dbContext.Trips.Select(t => t.Id).ToHashSet();
                        var tripsToAdd = new List<Trip>();
                        var seenIds = new HashSet<string>();
                        
                        foreach (var trip in trips)
                        {
                            // JSON içinde duplicate ID kontrolü
                            if (seenIds.Contains(trip.Id))
                            {
                                Console.WriteLine($"Uyarı: JSON'da duplicate ID bulundu: {trip.Id}, atlanıyor.");
                                continue;
                            }
                            seenIds.Add(trip.Id);
                            
                            // Database'de zaten varsa atla
                            if (existingIds.Contains(trip.Id))
                            {
                                continue;
                            }
                            
                            // DateTime'ı UTC'ye çevir
                            if (trip.DepartureTime.Kind == DateTimeKind.Unspecified)
                            {
                                trip.DepartureTime = DateTime.SpecifyKind(trip.DepartureTime, DateTimeKind.Utc);
                            }
                            else if (trip.DepartureTime.Kind == DateTimeKind.Local)
                            {
                                trip.DepartureTime = trip.DepartureTime.ToUniversalTime();
                            }
                            
                            tripsToAdd.Add(trip);
                        }
                        
                        if (tripsToAdd.Any())
                        {
                            dbContext.Trips.AddRange(tripsToAdd);
                            dbContext.SaveChanges();
                            Console.WriteLine($"{tripsToAdd.Count} trip verisi database'e eklendi.");
                        }
                    }
                }
            }
        }
        catch (Exception ex2)
        {
            Console.WriteLine($"Database oluşturma hatası: {ex2.Message}");
        }
    }
}

app.Run();

// PostgreSQL connection string parser
static string ParseConnectionString(string databaseUrl)
{
    // postgresql://user:password@host:port/database formatını parse et
    if (databaseUrl.StartsWith("postgresql://"))
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');
        
        return $"Host={host};Port={port};Database={database};Username={username};Password={Uri.UnescapeDataString(password)}";
    }
    
    // Zaten standard connection string formatındaysa olduğu gibi döndür
    return databaseUrl;
}

