using MyApp.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<TripRepository>();
builder.Services.AddSingleton<ReservationRepository>();

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

app.Run();

