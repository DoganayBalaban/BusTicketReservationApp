# Otobüs Rezervasyon Sistemi - Teknik Sunum

## Proje Genel Bakış

**Teknolojiler:**

- Backend: .NET 8 Web API
- Frontend: Blazor WebAssembly
- Veri: JSON dosyası (trips.json)
- Mimari: Repository Pattern, RESTful API

---

## 1. Proje Yapısı

```
MyApp/
├── src/
│   ├── MyApp.Api/              # Backend API
│   │   ├── Controllers/        # API Endpoints
│   │   ├── Models/            # Veri modelleri
│   │   ├── Services/          # Repository servisleri
│   │   └── Data/              # JSON veri dosyaları
│   └── MyApp.Client/          # Frontend Blazor
│       ├── Pages/             # Razor sayfaları
│       ├── Models/            # DTO modelleri
│       └── Services/          # HTTP servisleri
└── MyApp.sln
```

**Kod Konumu:** Tüm proje yapısı
**Dosya:** `MyApp.sln`, `src/` klasörü

---

## 2. Backend API Özellikleri

### 2.1 Sefer Yönetimi (Trips)

**Özellik:** 50 otobüs seferi verisi JSON dosyasından yüklenir

**Kod Konumu:** `src/MyApp.Api/Data/trips.json`

```json
[
  {
    "id": "1",
    "company": "Kamil Koç",
    "departureCity": "İstanbul",
    "arrivalCity": "Ankara",
    "departureTime": "2025-11-13T09:00:00",
    "price": 450,
    "busType": "2+1",
    "seats": 36
  }
]
```

**Özellik:** Trip modeli - Sefer bilgilerini tutar

**Kod Konumu:** `src/MyApp.Api/Models/Trip.cs`

```csharp
public class Trip
{
    public string Id { get; set; }
    public string Company { get; set; }
    public string DepartureCity { get; set; }
    public string ArrivalCity { get; set; }
    public DateTime DepartureTime { get; set; }
    public decimal Price { get; set; }
    public string BusType { get; set; }  // "2+1" veya "2+2"
    public int Seats { get; set; }
}
```

**Özellik:** TripRepository - JSON'dan veri okuma ve arama

**Kod Konumu:** `src/MyApp.Api/Services/TripRepository.cs`

```csharp
public class TripRepository
{
    public TripRepository()
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "trips.json");
        var jsonData = File.ReadAllText(jsonPath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true  // camelCase JSON okuma
        };
        _trips = JsonSerializer.Deserialize<List<Trip>>(jsonData, options);
    }

    public List<Trip> Search(string? departureCity, string? arrivalCity, DateTime? date)
    {
        // Şehir ve tarih bazlı filtreleme
    }
}
```

**Özellik:** Trips API Endpoints

**Kod Konumu:** `src/MyApp.Api/Controllers/TripsController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    [HttpGet]                    // GET /api/trips
    public ActionResult<List<Trip>> GetAll()

    [HttpGet("{id}")]           // GET /api/trips/{id}
    public ActionResult<Trip> GetById(string id)

    [HttpGet("search")]         // GET /api/trips/search?departureCity=...
    public ActionResult<List<Trip>> Search(...)
}
```

---

### 2.2 Rezervasyon Yönetimi (Reservations)

**Özellik:** Reservation modeli - Rezervasyon bilgilerini tutar

**Kod Konumu:** `src/MyApp.Api/Models/Reservation.cs`

```csharp
public class Reservation
{
    public string Id { get; set; }
    public string TripId { get; set; }
    public string PassengerName { get; set; }
    public string PassengerSurname { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string TCKimlik { get; set; }
    public List<string> Seats { get; set; }  // ["1A", "1B"]
    public string ReservationCode { get; set; }
    public DateTime ReservationDate { get; set; }
}
```

**Özellik:** ReservationRepository - In-memory rezervasyon yönetimi

**Kod Konumu:** `src/MyApp.Api/Services/ReservationRepository.cs`

```csharp
public class ReservationRepository
{
    private readonly List<Reservation> _reservations = new();

    public Reservation Add(Reservation reservation)
    public List<Reservation> GetByTripId(string tripId)
    public bool Delete(string id)
}
```

**Özellik:** Reservations API Endpoints

**Kod Konumu:** `src/MyApp.Api/Controllers/ReservationsController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    [HttpPost]                           // POST /api/reservations
    public ActionResult<Reservation> Create(Reservation reservation)
    {
        // Otomatik Id ve ReservationCode oluşturma
        if (string.IsNullOrEmpty(reservation.Id))
            reservation.Id = Guid.NewGuid().ToString();

        if (string.IsNullOrEmpty(reservation.ReservationCode))
            reservation.ReservationCode = Guid.NewGuid().ToString("N")[..8].ToUpper();

        reservation.ReservationDate = DateTime.Now;
    }

    [HttpGet("trip/{tripId}")]          // GET /api/reservations/trip/{tripId}
    public ActionResult<List<Reservation>> GetByTripId(string tripId)
}
```

---

### 2.3 CORS Yapılandırması

**Özellik:** Blazor frontend'in API'ye erişimi için CORS

**Kod Konumu:** `src/MyApp.Api/Program.cs`

```csharp
builder.Services.AddCors(o =>
{
    o.AddPolicy(allowClient, p =>
        p.WithOrigins("http://localhost:5085", "http://localhost:5000")
         .AllowAnyHeader()
         .AllowAnyMethod());
});

app.UseCors(allowClient);
```

---

## 3. Frontend Blazor Özellikleri

### 3.1 Sefer Listesi ve Arama

**Özellik:** Ana sayfa - Sefer listesi ve filtreleme

**Kod Konumu:** `src/MyApp.Client/Pages/Trips.razor`

```razor
@page "/"
@page "/trips"

<div class="filters">
    <input type="text" @bind="departureCity" placeholder="Kalkış Şehri" />
    <input type="text" @bind="arrivalCity" placeholder="Varış Şehri" />
    <input type="date" @bind="travelDate" />
    <button @onclick="Search">Ara</button>
</div>

@foreach (var trip in filtered)
{
    <div class="trip-card">
        <h4>@trip.Company</h4>
        <span class="price">@trip.Price.ToString("N2") ₺</span>
        <div class="route">
            <span>@trip.DepartureCity</span> → <span>@trip.ArrivalCity</span>
        </div>
        <button @onclick="@(() => NavigateToSeats(trip.Id))">Koltuk Seç</button>
    </div>
}
```

**Özellik:** Client-side filtreleme

**Kod Konumu:** `src/MyApp.Client/Pages/Trips.razor` (code section)

```csharp
private void Search()
{
    filtered = all
        .Where(t =>
            (string.IsNullOrWhiteSpace(departureCity) ||
             t.DepartureCity.Contains(departureCity, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(arrivalCity) ||
             t.ArrivalCity.Contains(arrivalCity, StringComparison.OrdinalIgnoreCase)) &&
            (!travelDate.HasValue || t.DepartureTime.Date == travelDate.Value.Date))
        .ToList();
}
```

---

### 3.2 HTTP Servisleri

**Özellik:** TripService - API'den sefer verilerini çeker

**Kod Konumu:** `src/MyApp.Client/Services/TripService.cs`

```csharp
public class TripService
{
    private readonly HttpClient _http;

    public async Task<List<TripDto>> GetTripsAsync()
    {
        var trips = await _http.GetFromJsonAsync<List<TripDto>>("api/trips");
        return trips ?? new List<TripDto>();
    }

    public async Task<TripDto?> GetTripByIdAsync(string id)
    {
        return await _http.GetFromJsonAsync<TripDto>($"api/trips/{id}");
    }
}
```

**Özellik:** ReservationService - Rezervasyon işlemleri

**Kod Konumu:** `src/MyApp.Client/Services/ReservationService.cs`

```csharp
public class ReservationService
{
    public async Task<List<string>> GetOccupiedSeatsAsync(string tripId)
    {
        // Dolu koltukları getir
        var reservations = await GetReservationsByTripIdAsync(tripId);
        return reservations.SelectMany(r => r.Seats).ToList();
    }

    public async Task<ReservationDto?> CreateReservationAsync(ReservationDto reservation)
    {
        var response = await _http.PostAsJsonAsync("api/reservations", reservation);
        return await response.Content.ReadFromJsonAsync<ReservationDto>();
    }
}
```

**Özellik:** LocalStorageService - Tarayıcı localStorage kullanımı

**Kod Konumu:** `src/MyApp.Client/Services/LocalStorageService.cs`

```csharp
public class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public async Task SetItemAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        return JsonSerializer.Deserialize<T>(json);
    }
}
```

---

### 3.3 Koltuk Seçimi

**Özellik:** Seats sayfası - Koltuk haritası ve seçim

**Kod Konumu:** `src/MyApp.Client/Pages/Seats.razor`

```razor
@page "/trips/{id}/seats"

<div class="trip-info">
    <h3>@trip.Company — @trip.DepartureCity → @trip.ArrivalCity</h3>
    <p>Otobüs Tipi: @trip.BusType | Toplam Koltuk: @trip.Seats</p>
</div>

<SeatMap
    Rows="@rows"
    Cols="@cols"
    TotalSeats="@trip.Seats"
    OccupiedSeats="@occupiedSeats"
    SelectedSeats="@selectedSeats"
    OnSeatsChanged="OnSeatsChanged" />

<button @onclick="GoCheckout">
    Devam Et (@selectedSeats.Count koltuk)
</button>
```

**Özellik:** Otobüs tipine göre koltuk düzeni

**Kod Konumu:** `src/MyApp.Client/Pages/Seats.razor` (code section)

```csharp
protected override async Task OnParametersSetAsync()
{
    trip = await TripService.GetTripByIdAsync(Id);

    // Otobüs tipine göre sütun sayısını belirle
    if (trip.BusType == "2+1")
    {
        cols = 3;
        rows = (int)Math.Ceiling(trip.Seats / 3.0);
    }
    else  // "2+2"
    {
        cols = 4;
        rows = (int)Math.Ceiling(trip.Seats / 4.0);
    }

    // Dolu koltukları yükle
    occupiedSeats = await ReservationService.GetOccupiedSeatsAsync(Id);
}
```

---

### 3.4 Rezervasyon ve Ödeme

**Özellik:** Checkout sayfası - Yolcu bilgileri ve rezervasyon

**Kod Konumu:** `src/MyApp.Client/Pages/Checkout.razor`

```razor
@page "/checkout"

<div class="trip-summary">
    <h4>Sefer Bilgileri</h4>
    <p><strong>Firma:</strong> @trip.Company</p>
    <p><strong>Güzergah:</strong> @trip.DepartureCity → @trip.ArrivalCity</p>
    <p><strong>Seçilen Koltuklar:</strong> @string.Join(", ", selectedSeats)</p>
    <p><strong>Toplam Tutar:</strong> @((selectedSeats.Count * trip.Price).ToString("N2")) ₺</p>
</div>

<PassengerForm OnSubmitValid="OnSubmitValid" />

@if (!string.IsNullOrEmpty(successMessage))
{
    <div class="alert alert-success">
        ✓ @successMessage
    </div>
}

@if (completedReservation is not null)
{
    <div class="reservation-summary">
        <p><strong>Rezervasyon Kodu:</strong> @completedReservation.ReservationCode</p>
        <p><strong>Yolcu:</strong> @completedReservation.PassengerName @completedReservation.PassengerSurname</p>
    </div>
}
```

**Özellik:** Rezervasyon oluşturma ve localStorage'a kaydetme

**Kod Konumu:** `src/MyApp.Client/Pages/Checkout.razor` (code section)

```csharp
private async Task OnSubmitValid(PassengerForm.Passenger passenger)
{
    var reservation = new ReservationDto
    {
        TripId = trip.Id,
        Seats = selectedSeats,  // ["1A", "1B"]
        PassengerName = passenger.FirstName,
        PassengerSurname = passenger.LastName,
        Email = passenger.Email,
        TCKimlik = passenger.TCKimlik,
        Phone = passenger.Phone
    };

    var createdReservation = await ReservationService.CreateReservationAsync(reservation);

    // localStorage'a kaydet (sayfa yenilendiğinde göster)
    await LocalStorage.SetItemAsync("lastReservation", createdReservation);

    completedReservation = createdReservation;
    successMessage = $"Rezervasyonunuz başarıyla oluşturuldu. Rezervasyon kodunuz: {createdReservation.ReservationCode}";
}
```

**Özellik:** Sayfa yenilendiğinde rezervasyonu gösterme

**Kod Konumu:** `src/MyApp.Client/Pages/Checkout.razor` (code section)

```csharp
private async Task LoadStoredReservationIfExists()
{
    var storedReservation = await LocalStorage.GetItemAsync<ReservationDto>("lastReservation");

    // Aynı sefer ve koltuklar için rezervasyon varsa göster
    if (storedReservation?.TripId == trip.Id &&
        selectedSeats.SequenceEqual(storedReservation.Seats))
    {
        completedReservation = storedReservation;
        successMessage = $"Rezervasyonunuz hazır. Rezervasyon kodunuz: {storedReservation.ReservationCode}";
    }
}
```

---

## 4. Dependency Injection

**Özellik:** Backend servislerin DI ile kaydı

**Kod Konumu:** `src/MyApp.Api/Program.cs`

```csharp
builder.Services.AddSingleton<TripRepository>();
builder.Services.AddSingleton<ReservationRepository>();
```

**Özellik:** Frontend servislerin DI ile kaydı

**Kod Konumu:** `src/MyApp.Client/Program.cs`

```csharp
// API için HttpClient
builder.Services.AddScoped(sp => new HttpClient {
    BaseAddress = new Uri("http://localhost:5001/")
});

// Servisler
builder.Services.AddScoped<TripService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<LocalStorageService>();
```

---

## 5. Veri Modelleri

**Özellik:** DTO modelleri - Frontend ve Backend arasında veri transferi

**Kod Konumu:** `src/MyApp.Client/Models/TripDto.cs`

```csharp
public class TripDto
{
    public string Id { get; set; }
    public string Company { get; set; }
    public string DepartureCity { get; set; }
    public string ArrivalCity { get; set; }
    public DateTime DepartureTime { get; set; }
    public decimal Price { get; set; }
    public string BusType { get; set; }
    public int Seats { get; set; }
}
```

**Kod Konumu:** `src/MyApp.Client/Models/ReservationDto.cs`

```csharp
public class ReservationDto
{
    public string Id { get; set; }
    public string TripId { get; set; }
    public string PassengerName { get; set; }
    public string PassengerSurname { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string TCKimlik { get; set; }
    public List<string> Seats { get; set; }  // Koltuk numaraları
    public string ReservationCode { get; set; }
    public DateTime ReservationDate { get; set; }
}
```

---

## 6. Önemli Teknik Detaylar

### 6.1 JSON Deserializasyon

**Sorun:** JSON'da camelCase, C#'ta PascalCase
**Çözüm:** PropertyNameCaseInsensitive = true

**Kod Konumu:** `src/MyApp.Api/Services/TripRepository.cs`

```csharp
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};
_trips = JsonSerializer.Deserialize<List<Trip>>(jsonData, options);
```

### 6.2 Koltuk Numaraları

**Tasarım Kararı:** Koltuklar string olarak saklanır ("1A", "1B", "2C")
**Neden:** Otobüslerde koltuklar harf-sayı kombinasyonu kullanır

**Kod Konumu:** `src/MyApp.Api/Models/Reservation.cs`

```csharp
public List<string> Seats { get; set; } = new();  // ["1A", "1B"]
```

### 6.3 Otomatik ID Oluşturma

**Özellik:** Rezervasyon oluşturulurken otomatik ID ve kod

**Kod Konumu:** `src/MyApp.Api/Controllers/ReservationsController.cs`

```csharp
if (string.IsNullOrEmpty(reservation.Id))
    reservation.Id = Guid.NewGuid().ToString();

if (string.IsNullOrEmpty(reservation.ReservationCode))
    reservation.ReservationCode = Guid.NewGuid().ToString("N")[..8].ToUpper();
```

---

## 7. Çalıştırma

**Backend API:**

```bash
cd src/MyApp.Api
dotnet run
```

URL: http://localhost:5001
Swagger: http://localhost:5001/swagger

**Frontend Blazor:**

```bash
cd src/MyApp.Client
dotnet run
```

URL: http://localhost:5000

---

## 8. Kullanılan Teknolojiler ve Kütüphaneler

- **.NET 8 SDK**
- **ASP.NET Core Web API** - RESTful API
- **Blazor WebAssembly** - SPA framework
- **System.Text.Json** - JSON serialization
- **Swagger/OpenAPI** - API dokümantasyonu
- **HttpClient** - HTTP istekleri
- **IJSRuntime** - JavaScript interop (localStorage)

---

## 9. Mimari Kararlar

1. **Repository Pattern:** Veri erişim katmanı soyutlaması
2. **DTO Pattern:** Frontend-Backend veri transferi
3. **Dependency Injection:** Loose coupling
4. **RESTful API:** Standart HTTP metodları
5. **SPA (Single Page Application):** Blazor WebAssembly
6. **In-Memory Storage:** Rezervasyonlar için (demo amaçlı)
7. **JSON File Storage:** Sefer verileri için

---

## 10. Geliştirilebilir Özellikler

- Database entegrasyonu (SQL Server, PostgreSQL)
- Authentication/Authorization (JWT)
- Payment gateway entegrasyonu
- Email bildirimleri
- Admin paneli
- Gerçek zamanlı koltuk güncellemeleri (SignalR)
- Unit testler
- Docker containerization
