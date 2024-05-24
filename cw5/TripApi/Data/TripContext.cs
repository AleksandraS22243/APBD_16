using Microsoft.EntityFrameworkCore;
using TripApi.Models;

namespace TripApi.Data
{
    public class TripContext : DbContext
    {
        public TripContext(DbContextOptions<TripContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientTrip> ClientTrips { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CountryTrip> CountryTrips { get; set; }
        public DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientTrip>()
                .HasKey(ct => new { ct.IdClient, ct.IdTrip });

            modelBuilder.Entity<CountryTrip>()
                .HasKey(ct => new { ct.IdCountry, ct.IdTrip });

            modelBuilder.Entity<Client>()
                .HasKey(c => c.IdClient);

            modelBuilder.Entity<Trip>()
                .HasKey(t => t.IdTrip);

            modelBuilder.Entity<Country>()
                .HasKey(c => c.IdCountry);

            modelBuilder.Entity<Trip>()
                .HasMany(t => t.CountryTrips)
                .WithOne(ct => ct.Trip)
                .HasForeignKey(ct => ct.IdTrip);

            modelBuilder.Entity<Trip>()
                .HasMany(t => t.ClientTrips)
                .WithOne(ct => ct.Trip)
                .HasForeignKey(ct => ct.IdTrip);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.ClientTrips)
                .WithOne(ct => ct.Client)
                .HasForeignKey(ct => ct.IdClient);

            modelBuilder.Entity<Country>()
                .HasMany(c => c.CountryTrips)
                .WithOne(ct => ct.Country)
                .HasForeignKey(ct => ct.IdCountry);
        }
    }
}
