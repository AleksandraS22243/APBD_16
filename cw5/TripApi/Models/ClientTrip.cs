using System;

namespace TripApi.Models
{
    public class ClientTrip
    {
        public int IdClient { get; set; }
        public int IdTrip { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime? PaymentDate { get; set; }

        public Client Client { get; set; } = new Client();
        public Trip Trip { get; set; } = new Trip();
    }
}
