using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Api.Models
{
    public class Trip
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
}
