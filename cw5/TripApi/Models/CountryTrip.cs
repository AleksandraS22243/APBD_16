namespace TripApi.Models
{
    public class CountryTrip
    {
        public int IdCountry { get; set; }
        public int IdTrip { get; set; }

        public Country Country { get; set; } = new Country();
        public Trip Trip { get; set; } = new Trip();
    }
}