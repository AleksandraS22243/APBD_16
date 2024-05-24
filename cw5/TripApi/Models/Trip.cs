using System;
using System.Collections.Generic;

namespace TripApi.Models
{
    public class Trip
    {
        public int IdTrip { get; set; }
        public string Name { get; set; } = string.Empty; // Initialize to empty string
        public string Description { get; set; } = string.Empty; // Initialize to empty string
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int MaxPeople { get; set; }

        public ICollection<CountryTrip> CountryTrips { get; set; } = new List<CountryTrip>();
        public ICollection<ClientTrip> ClientTrips { get; set; } = new List<ClientTrip>();
    }
}