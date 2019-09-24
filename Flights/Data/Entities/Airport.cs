using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Flights.Data.Entities.EntitiesEnums;

namespace Flights.Data.Entities
{
    public class Airport : BaseCRUD
    {
        [Required(ErrorMessage = "Le nom de l'aéroport est requis")]
        [Display(Name = "Aéroport")]
        public string AirportName { get; set; }

        [Required(ErrorMessage = "Le pays de l'aéroport est requis")]
        [Display(Name = "Pays")]
        public string AirportCountry { get; set; }

        [Required(ErrorMessage = "La latitude est requise")]
        [Display(Name = "Latitude")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "La longitude est requise")]
        [Display(Name = "Longitude")]
        public double Longitude { get; set; }
        public List<Flight> FlightsDeparture { get; set; }
        public List<Flight> FlightsDestination { get; set; }

        public override string ToString()
        {
            return $"{AirportName},{AirportCountry}";
        }

        /// <summary>
        /// Constructeur sans paramètres
        /// </summary>
        public Airport() : base()
        {
        }

        public Airport(string airportName, string airportCountry, double latitude, double longitude)
        {
            this.Id = Guid.NewGuid();
            this.AirportName = airportName;
            this.AirportCountry = airportCountry;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Status = EStatus.ACTIVE;
        }

        /// <summary>
        /// Mise à jour du modèle
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override BaseCRUD Update(BaseCRUD originalEntity)
        {
            if(originalEntity is Airport airport)
            {
                this.FlightsDeparture = airport.FlightsDeparture;
                this.FlightsDestination = airport.FlightsDestination;
                this.RowVersion = Guid.NewGuid();
            }

            return this;
        }
    }
}
