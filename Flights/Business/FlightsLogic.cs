using Flights.Areas.Administration.Models;
using Flights.Data;
using Flights.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flights.Business
{
    public class FlightsLogic
    {
        private readonly ApplicationDbContext _context;

        public FlightsLogic(ApplicationDbContext context)
        {
            this._context = context;
        }

        public Flight SetPeople(Flight flight, FlightsViewModel flightsViewModel, Guid aircraftGuid)
        {
            flight.Aircraft = _context.Aircrafts.FirstOrDefault(s => s.Id == aircraftGuid);
            if (flightsViewModel.PeopleIds != null)
                flightsViewModel.PeopleIds.ForEach(s => {
                    if (!flight.AddPerson(s)) throw new Exception("Limit excedeed !");
                });

            return flight;
        }

        public bool TryReserveAircraft(Flight flight, Guid aircraftGuid)
        {
            return flight.TryReserveAircraft(
                _context.Aircrafts.FirstOrDefault(s => s.Id == aircraftGuid),
                _context.Flights.AsNoTracking()
                    .Where(s => 
                    s.AircraftGuid == aircraftGuid && 
                    s.Id != flight.Id && 
                    s.Status == EntitiesEnums.EStatus.ACTIVE
                ));
        }

        public FlightsViewModel FillLists(FlightsViewModel flightViewModel)
        {
            flightViewModel.AircraftList = GetAircraftsList();
            flightViewModel.AirportsList = GetAirportsList();

            if (flightViewModel.FlightPersons == null)
                flightViewModel.FlightPersons = new List<FlightPerson>();

            if (flightViewModel.PeopleIds == null)
                flightViewModel.PeopleIds = new List<string>();

            flightViewModel.FlightPersons.ForEach(s => flightViewModel.PeopleIds.Add(s.PersonId));
            flightViewModel.IncludedPeople = GetListPeopleFromIncludedIds(flightViewModel.PeopleIds);
            flightViewModel.ExcludedPeople = GetListPeopleFromExcludedIds(flightViewModel.PeopleIds);
            return flightViewModel;
        }

        public IEnumerable<SelectListItem> GetListPeopleFromExcludedIds(List<string> Ids)
        {
            return _context.Users
                 .Where(s => !Ids.Contains(s.Id))
                 .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.UserName });
        }

        public IEnumerable<SelectListItem> GetListPeopleFromIncludedIds(List<string> Ids)
        {
            return _context.Users
                .Where(s => Ids.Contains(s.Id))
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.UserName });
        }

        public IEnumerable<SelectListItem> GetAirportsList()
        {
            return 
                _context.Airports
                .Where(s => s.Status == EntitiesEnums.EStatus.ACTIVE)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.AirportName });
        }

        public IEnumerable<SelectListItem> GetAircraftsList()
        {
            return
                _context.Aircrafts
                .Where(s => s.Status == EntitiesEnums.EStatus.ACTIVE)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.AircraftName });
        }
    }
}
