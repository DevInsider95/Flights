using System;
using System.Collections.Generic;
using System.Text;
using Flights.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Flights.Data
{
    public class ApplicationDbContext : IdentityDbContext<Person, IdentityRole, string>
    {
        public DbSet<Person> Persons { get; set; }

        public DbSet<Flight> Flights { get; set; }

        public DbSet<FlightPerson> FlightsPersons { get; set; }

        public DbSet<Airport> Airports { get; set; }

        public DbSet<Aircraft> Aircrafts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Airport>().HasKey(e => e.Id);
            modelBuilder.Entity<Aircraft>().HasKey(e => e.Id);

            modelBuilder.Entity<Flight>().HasKey(e => e.Id);
            modelBuilder.Entity<Flight>().Property(e => e.DateTimeBegin).IsRequired();
            modelBuilder.Entity<Flight>().Property(e => e.DateTimeEnd).IsRequired();

            modelBuilder.Entity<Flight>()
                .HasOne(e => e.Aircraft)
                .WithMany(e => e.Flights)
                .HasForeignKey(e => e.AircraftGuid)
                .IsRequired();

            modelBuilder.Entity<Flight>()
                 .HasOne(e => e.DepartureAirport)
                 .WithMany(e => e.FlightsDeparture)
                 .HasForeignKey(e => e.DepartureAirportGuid)
                 .OnDelete(DeleteBehavior.Restrict)
                 .IsRequired();

            modelBuilder.Entity<Flight>()
                .HasOne(e => e.DestinationAirport)
                .WithMany(e => e.FlightsDestination)
                .HasForeignKey(e => e.DestinationAirportGuid)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<FlightPerson>().HasKey(e => new { e.FlightId, e.PersonId });
            modelBuilder.Entity<FlightPerson>()
                .HasOne(e => e.Flight)
                .WithMany(e => e.FlightPersons)
                .HasForeignKey(e => e.FlightId);

            modelBuilder.Entity<FlightPerson>()
                .HasOne(e => e.Person)
                .WithMany(e => e.FlightPersons)
                .HasForeignKey(e => e.PersonId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
