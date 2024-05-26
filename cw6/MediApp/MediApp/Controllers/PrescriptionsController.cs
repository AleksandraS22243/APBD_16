using Microsoft.AspNetCore.Mvc;
using MediApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using MediApp.Data;


namespace MediApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly PrescriptionContext _context;

        public PrescriptionsController(PrescriptionContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescription([FromBody] PrescriptionRequest request)
        {
            if (request.Medicaments.Count > 10)
                return BadRequest("A prescription can include a maximum of 10 medicaments.");

            if (request.DueDate < request.Date)
                return BadRequest("DueDate must be greater than or equal to Date.");

            var patient = await _context.Patients.FindAsync(request.Patient.IdPatient);
            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = request.Patient.FirstName,
                    LastName = request.Patient.LastName,
                    Birthdate = request.Patient.Birthdate
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            foreach (var med in request.Medicaments)
            {
                var medicament = await _context.Medicaments.FindAsync(med.IdMedicament);
                if (medicament == null)
                    return BadRequest($"Medicament with Id {med.IdMedicament} does not exist.");
            }

            var prescription = new Prescription
            {
                Date = request.Date,
                DueDate = request.DueDate,
                IdPatient = patient.IdPatient,
                IdDoctor = request.IdDoctor
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            foreach (var med in request.Medicaments)
            {
                _context.Prescription_Medicaments.Add(new Prescription_Medicament
                {
                    IdMedicament = med.IdMedicament,
                    IdPrescription = prescription.IdPrescription,
                    Dose = med.Dose,
                    Details = med.Details
                });
            }

            await _context.SaveChangesAsync();
            return Ok(prescription);
        }
    }

    public class PrescriptionRequest
    {
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public PatientRequest Patient { get; set; }
        public int IdDoctor { get; set; }
        public List<MedicamentRequest> Medicaments { get; set; }
    }

    public class PatientRequest
    {
        public int IdPatient { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
    }

    public class MedicamentRequest
    {
        public int IdMedicament { get; set; }
        public int Dose { get; set; }
        public string Details { get; set; }
    }
}
