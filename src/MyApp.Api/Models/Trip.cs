namespace MyApp.Api.Models;

public class Trip
{
    public string Id { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string DepartureCity { get; set; } = string.Empty;
    public string ArrivalCity { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public decimal Price { get; set; }
    public string BusType { get; set; } = string.Empty;
    public int Seats { get; set; }
}
