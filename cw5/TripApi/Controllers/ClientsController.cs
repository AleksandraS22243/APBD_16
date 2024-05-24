using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TripApi.Data;
using TripApi.Dtos;
using TripApi.Models;

namespace TripApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly TripContext _context;

        public ClientsController(TripContext context)
        {
            _context = context;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            bool hasTrips = _context.ClientTrips.Any(ct => ct.IdClient == id);
            if (hasTrips)
            {
                return BadRequest("Client has assigned trips.");
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{idTrip}/clients")]
        public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] ClientTripDto clientTripDto)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == clientTripDto.Pesel);
            if (client == null)
            {
                client = new Client
                {
                    FirstName = clientTripDto.FirstName,
                    LastName = clientTripDto.LastName,
                    Email = clientTripDto.Email,
                    Telephone = clientTripDto.Telephone,
                    Pesel = clientTripDto.Pesel
                };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
            }

            var trip = await _context.Trips.FindAsync(idTrip);
            if (trip == null)
            {
                return NotFound("Trip not found.");
            }

            bool alreadyAssigned = await _context.ClientTrips.AnyAsync(ct => ct.IdClient == client.IdClient && ct.IdTrip == idTrip);
            if (alreadyAssigned)
            {
                return BadRequest("Client is already assigned to this trip.");
            }

            var clientTrip = new ClientTrip
            {
                IdClient = client.IdClient,
                IdTrip = idTrip,
                RegisteredAt = DateTime.Now,
                PaymentDate = clientTripDto.PaymentDate
            };

            _context.ClientTrips.Add(clientTrip);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
