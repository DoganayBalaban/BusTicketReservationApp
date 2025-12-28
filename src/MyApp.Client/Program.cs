using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyApp.Client;                 // App.razor için
using MyApp.Client.Services;        // TripService için

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// API base URL'ini configuration'dan al (varsayılan olarak localhost:5001)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? 
                 Environment.GetEnvironmentVariable("API_BASE_URL") ?? 
                 "http://localhost:5001/";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Servislerini ekle
builder.Services.AddScoped<TripService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<LocalStorageService>();

await builder.Build().RunAsync();
