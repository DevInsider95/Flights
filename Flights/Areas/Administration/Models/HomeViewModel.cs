using Flights.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flights.Areas.Administration.Models
{
    public class HomeViewModel
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructeur
        /// </summary>
        public HomeViewModel(ApplicationDbContext context)
        {
            this._context = context;
            ClientsCount = GlobalResources.CountInRole(_context, GlobalResources.ROLE_CLIENT);
            AirportsCount = _context.Airports.LongCount();
            FlightsCount = _context.Flights.LongCount();
            AircraftCount = _context.Aircrafts.LongCount();
        }

        /// <summary>
        /// Nombre de clients
        /// </summary>
        public long ClientsCount { get; set; }

        /// <summary>
        /// Nombre d'aéroports
        /// </summary>
        public long AirportsCount { get; set; }

        /// <summary>
        /// Nombre de vols
        /// </summary>
        public long FlightsCount { get; set; }

        /// <summary>
        /// Nombre d'avions
        /// </summary>
        public long AircraftCount { get; set; }
    }
}
