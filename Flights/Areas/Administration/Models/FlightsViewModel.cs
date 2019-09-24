using Flights.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Flights.Data.Entities.EntitiesEnums;

namespace Flights.Areas.Administration.Models
{
    public class FlightsViewModel
    {
        public Guid Id { get; set; }
        public Guid RowVersion { get; set; }
        public EStatus Status { get; set; }
        public IEnumerable<SelectListItem> AirportsList { get; set; }
        public IEnumerable<SelectListItem> AircraftList { get; set; }
        public IEnumerable<SelectListItem> IncludedPeople { get; set; }
        public IEnumerable<SelectListItem> ExcludedPeople { get; set; }

        [Display(Name = "Personnes incluses")]
        public List<string> PeopleIds { get; set; }

        [Required(ErrorMessage = "Le départ est requis")]
        [Display(Name = "Aéroport de départ")]
        public Guid DepartureAirportGuid { get; set; }

        [Required(ErrorMessage = "La destination est requise")]
        [Display(Name = "Aéroport d'arrivée")]
        public Guid DestinationAirportGuid { get; set; }

        [Required(ErrorMessage = "La date et l'heure de début sont requises")]
        [Display(Name = "Date et heure de début")]
        public DateTime DateTimeBegin { get; set; }

        [Required(ErrorMessage = "La date et l'heure de fin sont requises")]
        [Display(Name = "Date et heure de fin")]
        public DateTime DateTimeEnd { get; set; }

        [Required(ErrorMessage = "L'avion est requis")]
        [Display(Name = "Avion")]
        public Guid AircraftGuid { get; set; }
        public List<FlightPerson> FlightPersons { get; set; }

        public static implicit operator FlightsViewModel(Flight flight)
        {
            return new FlightsViewModel
            {
                Id = flight.Id,
                RowVersion = flight.RowVersion,
                Status = flight.Status,
                DepartureAirportGuid = flight.DepartureAirportGuid,
                DestinationAirportGuid = flight.DestinationAirportGuid,
                DateTimeBegin = flight.DateTimeBegin,
                DateTimeEnd = flight.DateTimeEnd,
                AircraftGuid = flight.AircraftGuid,
                FlightPersons = flight.FlightPersons,
                PeopleIds = new List<string>()
            };
        }

        public static implicit operator Flight(FlightsViewModel flightViewModel)
        {
            Flight flight = new Flight
            {
                Id = flightViewModel.Id,
                RowVersion = flightViewModel.RowVersion,
                Status = flightViewModel.Status,
                DepartureAirportGuid = flightViewModel.DepartureAirportGuid,
                DestinationAirportGuid = flightViewModel.DestinationAirportGuid,
                DateTimeBegin = flightViewModel.DateTimeBegin,
                DateTimeEnd = flightViewModel.DateTimeEnd,
                //AircraftGuid = flightViewModel.AircraftGuid, Utiliser la méthode TryReserveAircraft à la place
                FlightPersons = new List<FlightPerson>()
            };

            return flight;
        }
    }
}
