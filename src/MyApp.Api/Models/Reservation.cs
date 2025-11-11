namespace MyApp.Api.Models;

public class Reservation
{
    public string Id { get; set; } = "";
    public string TripId { get; set; } = "";
    public List<string> Seats { get; set; } = new();
    public string PassengerName { get; set; } = "";
    public string PassengerSurname { get; set; } = "";
    public string Email { get; set; } = "";
    public string TCKimlik { get; set; } = "";
    public string Phone { get; set; } = "";
    public DateTime ReservationDate { get; set; }
    public string ReservationCode { get; set; } = "";
}

