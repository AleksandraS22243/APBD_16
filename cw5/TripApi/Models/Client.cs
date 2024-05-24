using System.Collections.Generic;

namespace TripApi.Models
{
    public class Client
    {
        public int IdClient { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Pesel { get; set; } = string.Empty;

        public ICollection<ClientTrip> ClientTrips { get; set; } = new List<ClientTrip>();
    }
}
