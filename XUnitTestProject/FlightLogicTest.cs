using Flights.Areas.Administration.Models;
using Flights.Business;
using Flights.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Xunit;

namespace XUnitTestProject
{
    public class FlightLogicTest : UnitTestBase
    {
        public static TheoryData<DateTime, DateTime, DateTime, DateTime, bool> ReservationDateTimes = new TheoryData<DateTime, DateTime, DateTime, DateTime, bool>
        {
            // Dates de réservation en 2 vols pour 1 même avion dans avec le résultat attendu !
            // Réservations pendant que l'avion est déjà réservé
            { new DateTime(2019, 09, 23, 12, 0, 0),  new DateTime(2019, 09, 23, 14, 0, 0), new DateTime(2019, 09, 23, 13, 0, 0), new DateTime(2019, 09, 23, 15, 0, 0), false },
            { new DateTime(2019, 09, 23, 12, 0, 0),  new DateTime(2019, 09, 23, 14, 0, 0), new DateTime(2019, 09, 23, 11, 0, 0), new DateTime(2019, 09, 23, 13, 0, 0), false },
            { new DateTime(2019, 09, 23, 12, 0, 0),  new DateTime(2019, 09, 23, 14, 0, 0), new DateTime(2019, 09, 23, 11, 0, 0), new DateTime(2019, 09, 23, 15, 0, 0), false },
            { new DateTime(2019, 09, 23, 12, 0, 0),  new DateTime(2019, 09, 23, 14, 0, 0), new DateTime(2019, 09, 23, 13, 0, 0), new DateTime(2019, 09, 23, 13, 30, 0), false },

            // Réservations avant et après
            { new DateTime(2019, 09, 23, 12, 0, 0),  new DateTime(2019, 09, 23, 14, 0, 0), new DateTime(2019, 09, 23, 07, 0, 0), new DateTime(2019, 09, 23, 09, 0, 0), true },
            { new DateTime(2019, 09, 23, 12, 0, 0),  new DateTime(2019, 09, 23, 14, 0, 0), new DateTime(2019, 09, 23, 16, 0, 0), new DateTime(2019, 09, 23, 18, 0, 0), true },
        };

        [Fact]
        public void AircraftWith1MaximumPassenger_ShouldntSetMoreThan1Person()
        {
            // Création de deux comptes
            Person person_1 = CreatePerson("test", "test", "testaccount@mail.fr");
            Person person_2 = CreatePerson("test", "test", "testaccount2@mail.fr");

            // Capacité maximale à 1 personne
            Aircraft aircraft = new Aircraft("test", 500, 35, 100, new TimeSpan(2, 0, 0), 1);
            Airport airport_1 = new Airport("test", "France", 1.0, 1.0);
            Airport airport_2 = new Airport("test_2", "France", 2.0, 2.0);

            Flight flight = new Flight(airport_1, airport_2);

            // On contourne la réservation de l'avion pour ce test
            flight.Aircraft = aircraft;
            flight.AddPerson(person_1.Id);
            Assert.False(flight.AddPerson(person_2.Id));
        }

        [Theory]
        [ClassData(typeof(TheoryData<DateTime, DateTime, DateTime, DateTime, bool>))]
        [MemberData(nameof(FlightLogicTest.ReservationDateTimes))]
        public void Aircraft_ShouldntBeReservedIfAlreadyReserved(DateTime dateTimeBegin, DateTime dateTimeEnd, DateTime dateTimeBeginSecond, DateTime dateTimeEndSecond, bool expectedResult)
        {
            // Transaction pour rollback
            var transaction = _context.Database.BeginTransaction();

            // Création d'un vol
            Aircraft aircraft = new Aircraft("test", 500, 35, 100, new TimeSpan(2, 0, 0), 1);
            Airport airport_1 = new Airport("test", "France", 1.0, 1.0);
            Airport airport_2 = new Airport("test_2", "France", 2.0, 2.0);

            // Ajout des entités
            _context.Aircrafts.Add(aircraft);
            _context.Airports.Add(airport_1);
            _context.Airports.Add(airport_2);
            _context.SaveChanges();

            Flight flight = new Flight(airport_1, airport_2);
            flight.DateTimeBegin = dateTimeBegin;
            flight.DateTimeEnd = dateTimeEnd;

            // Première réservation
            if(_flightsLogic.TryReserveAircraft(flight, aircraft.Id))
            {
                // Ajout du vol et sauvegarde des changements
                _context.Flights.Add(flight);
                _context.SaveChanges();

                // Création d'un second vol
                flight = new Flight(airport_1, airport_2);
                flight.DateTimeBegin = dateTimeBeginSecond;
                flight.DateTimeEnd = dateTimeEndSecond;

                bool result = _flightsLogic.TryReserveAircraft(flight, aircraft.Id);
                transaction.Rollback();

                Assert.Equal(expectedResult, result);
            }
            else
            {
                transaction.Rollback();
                throw new Exception("First reservation failed !");
            }
        }

        [Fact]
        public void AirportList_ShouldReturnActiveEntites()
        {
            // Transaction pour rollback
            var transaction = _context.Database.BeginTransaction();

            Airport airport = new Airport("test_1", "France", 1.0, 1.0);
            Airport airport_inactive = new Airport("test_2", "France", 1.0, 1.0);
            airport_inactive.Status = EntitiesEnums.EStatus.ARCHIVED;

            // Ajout des entités au contexte
            _context.Airports.Add(airport);
            _context.Airports.Add(airport_inactive);
            _context.SaveChanges();

            var airportList = _flightsLogic.GetAirportsList().ToList();
            transaction.Rollback();

            Assert.Single(airportList);
        }

        [Fact]
        public void AircraftList_ShouldReturnActiveEntites()
        {
            // Transaction pour rollback
            var transaction = _context.Database.BeginTransaction();

            Aircraft aircraft = new Aircraft("test_1", 500, 35, 100, new TimeSpan(2, 0, 0), 1);
            Aircraft aircraft_inactive = new Aircraft("test_2", 500, 35, 100, new TimeSpan(2, 0, 0), 1);
            aircraft_inactive.Status = EntitiesEnums.EStatus.ARCHIVED;

            // Ajout des entités au contexte
            _context.Aircrafts.Add(aircraft);
            _context.Aircrafts.Add(aircraft_inactive);
            _context.SaveChanges();

            var aircraftList = _flightsLogic.GetAircraftsList().ToList();
            transaction.Rollback();

            Assert.Single(aircraftList);
        }

        [Fact]
        public void ListOfPeopleInsideAFlight_MustBePersisted()
        {
            // Transaction pour rollback
            var transaction = _context.Database.BeginTransaction();

            Airport airport = new Airport("test_1", "France", 1.0, 1.0);
            Airport airport_inactive = new Airport("test_2", "France", 1.0, 1.0);
            airport_inactive.Status = EntitiesEnums.EStatus.ARCHIVED;

            // Ajout des entités au contexte
            _context.Airports.Add(airport);
            _context.Airports.Add(airport_inactive);
            _context.SaveChanges();

            var airportList = _flightsLogic.GetAirportsList().ToList();
            transaction.Rollback();

            Assert.Single(airportList);
        }

        [Fact]
        public void FlightViewModel_ShouldAddPeopleToFlightEntity_UsingFlightLogic()
        {
            // Transaction pour rollback
            var transaction = _context.Database.BeginTransaction();

            // Création de deux comptes
            Person person_1 = CreatePerson("test", "test", "testaccount@mail.fr");
            Person person_2 = CreatePerson("test", "test", "testaccount2@mail.fr");

            // Capacité maximale à 5 personne
            Aircraft aircraft = new Aircraft("test", 500, 35, 100, new TimeSpan(2, 0, 0), 5);
            Airport airport_1 = new Airport("test", "France", 1.0, 1.0);
            Airport airport_2 = new Airport("test_2", "France", 2.0, 2.0);

            Flight flight = new Flight(airport_1, airport_2);

            // On contourne la réservation de l'avion pour ce test
            flight.Aircraft = aircraft;

            // Ajout des entités au contexte
            _context.Aircrafts.Add(aircraft);
            _context.Airports.Add(airport_1);
            _context.Airports.Add(airport_2);
            _context.SaveChanges();

            // Ajout des deux personnes à la vue
            FlightsViewModel flightsViewModel = flight;
            flightsViewModel.PeopleIds.Add(person_1.Id);
            flightsViewModel.PeopleIds.Add(person_2.Id);

            flight = _flightsLogic.SetPeople(flight, flightsViewModel, aircraft.Id);
            int count = flight.FlightPersons.Count;
            transaction.Rollback();

            Assert.Equal(2, count);
        }

        [Fact]
        public void AddingTwiceTheSamePersonToAFlight_MustReturnAnError()
        {
            // Transaction pour rollback
            var transaction = _context.Database.BeginTransaction();

            // Création d'un compte
            Person person_1 = CreatePerson("test", "test", "testaccount@mail.fr");

            // Capacité maximale à 5 personne
            Aircraft aircraft = new Aircraft("test", 500, 35, 100, new TimeSpan(2, 0, 0), 5);
            Airport airport_1 = new Airport("test", "France", 1.0, 1.0);
            Airport airport_2 = new Airport("test_2", "France", 2.0, 2.0);

            Flight flight = new Flight(airport_1, airport_2);

            // On contourne la réservation de l'avion pour ce test
            flight.Aircraft = aircraft;

            // Ajout des entités au contexte
            _context.Aircrafts.Add(aircraft);
            _context.Airports.Add(airport_1);
            _context.Airports.Add(airport_2);
            _context.SaveChanges();

            // Ajout des deux personnes à la vue
            FlightsViewModel flightsViewModel = flight;
            flightsViewModel.PeopleIds.Add(person_1.Id);
            flightsViewModel.PeopleIds.Add(person_1.Id);

            flight = _flightsLogic.SetPeople(flight, flightsViewModel, aircraft.Id);
            Assert.Throws<InvalidOperationException>(() => _context.Flights.Add(flight));
        }

        [Fact]
        public void GettingListOfPeopleWithIncludedIds_MustReturnTheMatchingList()
        {
            // Transaction pour rollback
            var transaction = _context.Database.BeginTransaction();

            // Création de comptes
            Person person_1 = CreatePerson("test", "test", "testaccount@mail.fr");
            Person person_2 = CreatePerson("test", "test", "testaccount@mail.fr");

            // Ajout des comptes au contexte
            _context.Users.Add(person_1);
            _context.Users.Add(person_2);
            _context.SaveChanges();

            Assert.DoesNotContain(person_2.Id, _flightsLogic.GetListPeopleFromIncludedIds(new List<string> { person_1.Id }).Select(s => s.Value));
        }

        [Fact]
        public void GettingListOfPeopleWithExcludedIds_MustReturnTheMatchingList()
        {
            // Transaction pour rollback
            var transaction = _context.Database.BeginTransaction();

            // Création de comptes
            Person person_1 = CreatePerson("test", "test", "testaccount@mail.fr");
            Person person_2 = CreatePerson("test", "test", "testaccount@mail.fr");

            // Ajout des comptes au contexte
            _context.Users.Add(person_1);
            _context.Users.Add(person_2);
            _context.SaveChanges();

            Assert.DoesNotContain(person_1.Id, _flightsLogic.GetListPeopleFromExcludedIds(new List<string> { person_1.Id }).Select(s => s.Value));
        }

        [Fact]
        public void FillList_MustReturnCorrectData()
        {
            // Transaction pour rollback
            var transaction = _context.Database.BeginTransaction();

            // Création de comptes
            Person person_1 = CreatePerson("test", "test", "testaccount@mail.fr");
            Person person_2 = CreatePerson("test", "test", "testaccount@mail.fr");

            Aircraft aircraft = new Aircraft("test", 500, 35, 100, new TimeSpan(2, 0, 0), 5);
            Airport airport = new Airport("test", "France", 1.0, 1.0);

            FlightsViewModel flightsViewModel = new FlightsViewModel();
            flightsViewModel.FlightPersons = new List<FlightPerson>();
            flightsViewModel.FlightPersons.Add(new FlightPerson(flightsViewModel.Id, person_1.Id));
            flightsViewModel.FlightPersons.Add(new FlightPerson(flightsViewModel.Id, person_2.Id));

            // Ajout des entités au contexte
            _context.Users.Add(person_1);
            _context.Users.Add(person_2);
            _context.Aircrafts.Add(aircraft);
            _context.Airports.Add(airport);
            _context.SaveChanges();

            flightsViewModel = _flightsLogic.FillLists(flightsViewModel);

            // Contient les deux personnes dans la liste où les personnes sont inclues
            Assert.Contains(person_1.Id, flightsViewModel.IncludedPeople.Select(s => s.Value));
            Assert.Contains(person_2.Id, flightsViewModel.IncludedPeople.Select(s => s.Value));

            // Ne contient pas les deux personnes dans la liste exclue
            Assert.DoesNotContain(person_1.Id, flightsViewModel.ExcludedPeople.Select(s => s.Value));
            Assert.DoesNotContain(person_2.Id, flightsViewModel.ExcludedPeople.Select(s => s.Value));

            // Contient l'aéroport
            Assert.Contains(airport.Id.ToString(), flightsViewModel.AirportsList.Select(s => s.Value));

            // Contient l'avion
            Assert.Contains(aircraft.Id.ToString(), flightsViewModel.AircraftList.Select(s => s.Value));

            // Contient les Ids des personnes
            Assert.Contains(person_1.Id, flightsViewModel.PeopleIds);
            Assert.Contains(person_2.Id, flightsViewModel.PeopleIds);
        }
    }
}
