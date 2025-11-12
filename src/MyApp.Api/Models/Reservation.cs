namespace MyApp.Api.Models;

public class Reservation
{
    public string Id { get; set; } = string.Empty;
    public string TripId { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string PassengerSurname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string TCKimlik { get; set; } = string.Empty;
    public List<string> Seats { get; set; } = new();
    public string ReservationCode { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; }
}
