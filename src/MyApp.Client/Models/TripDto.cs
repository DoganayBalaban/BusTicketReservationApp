namespace MyApp.Client.Models;

public class TripDto
{
    public string Id { get; set; } = "";
    public string Company { get; set; } = "";
    public string DepartureCity { get; set; } = "";
    public string ArrivalCity { get; set; } = "";
    public DateTime DepartureTime { get; set; }
    public decimal Price { get; set; }
    public string BusType { get; set; } = "";
    public int Seats { get; set; }
}