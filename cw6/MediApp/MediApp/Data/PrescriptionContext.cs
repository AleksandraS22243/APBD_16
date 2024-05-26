using MediApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MediApp.Data
{
    public class PrescriptionContext : DbContext
    {
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Prescription_Medicament> Prescription_Medicaments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }

        public PrescriptionContext(DbContextOptions<PrescriptionContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Prescription_Medicament>()
                .HasKey(pm => new { pm.IdMedicament, pm.IdPrescription });

            modelBuilder.Entity<Prescription_Medicament>()
                .HasOne(pm => pm.Medicament)
                .WithMany(m => m.Prescription_Medicaments)
                .HasForeignKey(pm => pm.IdMedicament);

            modelBuilder.Entity<Prescription_Medicament>()
                .HasOne(pm => pm.Prescription)
                .WithMany(p => p.Prescription_Medicaments)
                .HasForeignKey(pm => pm.IdPrescription);

            modelBuilder.Entity<Doctor>()
                .HasKey(d => d.IdDoctor);

            modelBuilder.Entity<Medicament>()
                .HasKey(m => m.IdMedicament);

            modelBuilder.Entity<Patient>()
                .HasKey(p => p.IdPatient);

            modelBuilder.Entity<Prescription>()
                .HasKey(pr => pr.IdPrescription);
        }
    }
}