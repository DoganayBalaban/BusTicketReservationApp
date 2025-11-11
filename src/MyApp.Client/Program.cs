using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyApp.Client;                 // App.razor için
using MyApp.Client.Services;        // TripService için

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// API tabanı (API'nin gerçek HTTP portunu yaz)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5001/") });

// Servislerini ekle
builder.Services.AddScoped<TripService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<LocalStorageService>();

await builder.Build().RunAsync();
