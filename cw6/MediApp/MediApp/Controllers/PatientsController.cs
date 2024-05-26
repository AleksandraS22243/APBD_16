using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using MediApp.Data;
using MediApp.Models;

namespace MediApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly PrescriptionContext _context;

        public PatientsController(PrescriptionContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientData(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Prescription_Medicaments)
                        .ThenInclude(pm => pm.Medicament)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Doctor)
                .FirstOrDefaultAsync(p => p.IdPatient == id);

            if (patient == null)
                return NotFound();

            var result = new
            {
                patient.IdPatient,
                patient.FirstName,
                patient.LastName,
                patient.Birthdate,
                Prescriptions = patient.Prescriptions.OrderBy(pr => pr.DueDate).Select(pr => new
                {
                    pr.IdPrescription,
                    pr.Date,
                    pr.DueDate,
                    Doctor = new
                    {
                        pr.Doctor.IdDoctor,
                        pr.Doctor.FirstName,
                        pr.Doctor.LastName,
                        pr.Doctor.Email
                    },
                    Medicaments = pr.Prescription_Medicaments.Select(pm => new
                    {
                        pm.IdMedicament,
                        pm.Medicament.Name,
                        pm.Medicament.Description,
                        pm.Medicament.Type,
                        pm.Dose,
                        pm.Details
                    })
                })
            };

            return Ok(result);
        }
    }
}
