using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flights.Areas.Administration.Models;
using Flights.Attributes;
using Flights.Data;
using Flights.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flights.Areas.Administration.Controllers
{
    [BaseActionFilter]
    [Authorize(Roles = GlobalResources.ROLE_ADMIN)]
    [Area(nameof(Administration))]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new HomeViewModel(_context));
        }
    }
}