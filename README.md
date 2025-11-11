# MyApp - .NET Backend + Blazor Frontend

.NET 8 Web API backend ve Blazor WebAssembly frontend ile oluşturulmuş temel proje yapısı.

## Proje Yapısı

```
MyApp/
├── src/
│   ├── MyApp.Api/          # .NET Web API Backend
│   └── MyApp.Client/       # Blazor WebAssembly Frontend
└── MyApp.sln               # Solution dosyası
```

## Çalıştırma

### Backend (API)

```bash
cd src/MyApp.Api
dotnet run
```

API: http://localhost:5001

### Frontend (Blazor)

```bash
cd src/MyApp.Client
dotnet run
```

Blazor: http://localhost:5000

## Özellikler

- ✅ .NET 8 Web API
- ✅ Blazor WebAssembly
- ✅ CORS yapılandırması
- ✅ Swagger/OpenAPI
- ✅ Örnek WeatherForecast endpoint

## Geliştirme

Her iki projeyi ayrı terminallerde çalıştırın. API varsayılan olarak 5001 portunda, Blazor uygulaması 5000 portunda çalışır.
