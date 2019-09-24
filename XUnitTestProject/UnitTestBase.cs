using Flights.Business;
using Flights.Data;
using Flights.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XUnitTestProject
{
    public class UnitTestBase
    {
        // Base de données de test
        private const string connectionString = "server=localhost;database=FlightsTest;port=3306;uid=root;password=root";
        protected readonly ApplicationDbContext _context;
        protected readonly FlightsLogic _flightsLogic;
       
        public UnitTestBase()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(connectionString).Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            _flightsLogic = new FlightsLogic(_context);
        }

        protected Person CreatePerson(string surname, string name, string email)
        {
            return new Person
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                Surname = surname,
                Name = name,
                UserName = email,
                Address = string.Empty,
                Town = string.Empty,
                ZIPCode = string.Empty,
                PhoneNumber = string.Empty,
                Status = EntitiesEnums.EStatus.ACTIVE,
                Civility = Person.ECivility.SIR
            };
        }
    }
}
