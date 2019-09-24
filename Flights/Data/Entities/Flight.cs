using GeoCoordinatePortable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static Flights.Data.Entities.EntitiesEnums;

namespace Flights.Data.Entities
{
    public class Flight : BaseCRUD
    {
        [Required]
        public Guid DepartureAirportGuid { get; set; }

        [Required]
        public Guid DestinationAirportGuid { get; set; }

        public Airport DepartureAirport { get; set; }
        public Airport DestinationAirport { get; set; }

        [Required]
        public DateTime DateTimeBegin { get; set; }

        [Required]
        public DateTime DateTimeEnd { get; set; }

        [Required]
        public Guid AircraftGuid { get; set; }
        public Aircraft Aircraft { get; set; }
        public List<FlightPerson> FlightPersons { get; set; }

        /// <summary>
        /// Constructeur sans paramètres
        /// </summary>
        public Flight() : base()
        {
            this.DateTimeBegin = DateTime.Now;
            this.DateTimeEnd = DateTime.Now;
        }

        public Flight(Airport departure, Airport destination)
        {
            this.Id = Guid.NewGuid();
            this.DepartureAirport = departure;
            this.DestinationAirport = destination;
            this.Status = EStatus.ACTIVE;
        }

        public bool AddPerson(string Id)
        {
            if (FlightPersons == null || Aircraft == null)
                FlightPersons = new List<FlightPerson>();

            if(FlightPersons.Count < Aircraft.TotalCapacity)
            {
                FlightPerson flightPerson = new FlightPerson(this.Id, Id);
                FlightPersons.Add(flightPerson);
                return true;
            }
            else
            {
                // Capacity excedeed !
                return false;
            }
        }

        public bool TryReserveAircraft(Aircraft aircraft, IEnumerable<Flight> flights)
        {
            // Ajout du temps de préparation de l'avion !
            DateTime dateTimeBegin = DateTimeBegin.Subtract(aircraft.PreparationTimeSpan);

            if(aircraft.CanBeReserved(dateTimeBegin, DateTimeEnd, flights))
            {
                this.AircraftGuid = aircraft.Id;
                this.Aircraft = aircraft;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Distance en KM
        /// </summary>
        public double CalculateDistance()
        {
            var departureCoords = new GeoCoordinate(DepartureAirport.Latitude, DepartureAirport.Longitude);
            var destinationCoords = new GeoCoordinate(DestinationAirport.Latitude, DestinationAirport.Longitude);

            return departureCoords.GetDistanceTo(destinationCoords) / 1000;
        }

        /// <summary>
        /// Calcul de la consommation de carburant requise pour le vol
        /// </summary>
        /// <returns></returns>
        public double CalculateRequiredFuel()
        {
            // La consommation est exprimée en : Consommation / (KM/h)
            var fuelConsumption_Per_KM_Per_Hour = Aircraft.FuelConsumption_Per_KM_Per_Hour;
            var distance = CalculateDistance();
            TimeSpan duration = DateTimeEnd.Subtract(DateTimeBegin);

            return (fuelConsumption_Per_KM_Per_Hour * (distance / duration.TotalHours)) + Aircraft.TakeOffEffort;
        }

        /// <summary>
        /// Mise à jour du modèle
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override BaseCRUD Update(BaseCRUD originalEntity)
        {
            if (originalEntity is Flight flight)
            {
                // Il n'est pas possible de changer d'avion car cela complexifie trop l'exercice à cause du nombre de passagers à gérer
                this.AircraftGuid = flight.AircraftGuid;
                this.RowVersion = Guid.NewGuid();
            }

            return this;
        }
    }
}
