using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flights.Data.Entities
{
    public class FlightPerson
    {
        public Guid FlightId { get; set; }
        public Flight Flight { get; set; }

        public string PersonId { get; set; }
        public Person Person { get; set; }

        /// <summary>
        /// Constructeur sans paramètres
        /// </summary>
        public FlightPerson()
        {

        }

        public FlightPerson(Flight flight, Person person)
        {
            this.FlightId = flight.Id;
            this.PersonId = person.Id;
        }

        public FlightPerson(Guid flightId, string personId)
        {
            this.FlightId = flightId;
            this.PersonId = personId;
        }
    }
}
