using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Flights.Models;
using Flights.Attributes;
using Microsoft.AspNetCore.Authorization;
using Flights.Data.Entities;

namespace Flights.Controllers
{
    [Authorize]
    [BaseActionFilter]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole(GlobalResources.ROLE_ADMIN))
                return Redirect("/Administration");

            // Par manque de temps, je n'ai pas pu faire la Zone : Client
            // Mais elle devrait s'appuyer de près à ce qui a été fait dans la Zone : Administration
            else if (User.IsInRole(GlobalResources.ROLE_CLIENT))
                return Redirect("/Client");

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
