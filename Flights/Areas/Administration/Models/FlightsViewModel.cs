using Flights.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
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
        public List<string> PeopleIds { get; set; }
        public Guid DepartureAirportGuid { get; set; }
        public Guid DestinationAirportGuid { get; set; }
        public DateTime DateTimeBegin { get; set; }
        public DateTime DateTimeEnd { get; set; }
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
