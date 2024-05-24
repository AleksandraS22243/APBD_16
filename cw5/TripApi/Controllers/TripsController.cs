using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TripApi.Data;
using TripApi.Dtos;

namespace TripApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly TripContext _context;

        public TripsController(TripContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TripDto>>> GetTrips()
        {
            var trips = await _context.Trips
                .OrderByDescending(t => t.DateFrom)
                .Select(t => new TripDto
                {
                    Name = t.Name,
                    Description = t.Description,
                    DateFrom = t.DateFrom,
                    DateTo = t.DateTo,
                    MaxPeople = t.MaxPeople,
                    Countries = t.CountryTrips.Select(ct => new CountryDto { Name = ct.Country.Name }),
                    Clients = t.ClientTrips.Select(ct => new ClientDto { FirstName = ct.Client.FirstName, LastName = ct.Client.LastName })
                })
                .ToListAsync();

            return Ok(trips);
        }
    }
}
