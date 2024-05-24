using System;
using System.Collections.Generic;

namespace TripApi.Dtos
{
    public class TripDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int MaxPeople { get; set; }
        public IEnumerable<CountryDto> Countries { get; set; } = new List<CountryDto>();
        public IEnumerable<ClientDto> Clients { get; set; } = new List<ClientDto>();
    }

    public class CountryDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class ClientDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class ClientTripDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Pesel { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; }
    }
}
