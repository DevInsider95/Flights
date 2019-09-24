using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flights.Areas.Administration.Models;
using Flights.Attributes;
using Flights.BaseControllers;
using Flights.Business;
using Flights.Data;
using Flights.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flights.Areas.Administration.Controllers
{
    /// <summary>
    /// L'architecture est un peu bancale ici car cela ressemble beaucoup au BaseCRUDController.
    /// J'aurais pu ajouter un autre contrôleur qui prend plusieurs Types en compte 
    /// pour faire les conversions entre ViewModel et Entity mais j'ai manqué de temps...
    /// J'aurais pu également créer une fonction Create au sein de l'objet pour faire les vérifications nécéssaires à la création
    /// </summary>
    [BaseActionFilter]
    [Authorize(Roles = GlobalResources.ROLE_ADMIN)]
    [Area(nameof(Administration))]
    public class FlightsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FlightsLogic _flightLogic;
        public FlightsController(ApplicationDbContext context)
        {
            _context = context;
            _flightLogic = new FlightsLogic(context);
        }

        public IActionResult Index()
        {
            var entities = _context.Flights.Where(s => s.Status == EntitiesEnums.EStatus.ACTIVE)
                .Include(nameof(Flight.Aircraft))
                .Include(nameof(Flight.DepartureAirport))
                .Include(nameof(Flight.DestinationAirport))
                .ToList();
            return View(entities);
        }

        public IActionResult Archives()
        {
            var entities = _context.Flights.Where(s => s.Status == EntitiesEnums.EStatus.ARCHIVED)
                .Include(nameof(Flight.Aircraft))
                .Include(nameof(Flight.DepartureAirport))
                .Include(nameof(Flight.DestinationAirport))
                .ToList();
            return View(entities);
        }

        public IActionResult Creer()
        {
            return View(_flightLogic.FillLists(new Flight()));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Creer(FlightsViewModel entityViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(_flightLogic.FillLists(entityViewModel));
            }

            try
            {
                if(!_flightLogic.TryReserveAircraft(entityViewModel, entityViewModel.AircraftGuid))
                {
                    ModelState.AddModelError(string.Empty, "Cet avion est déjà réservé dans cette plage horaire");
                    return View(_flightLogic.FillLists(entityViewModel));
                }

                Flight entity = entityViewModel;
                entity = _flightLogic.SetPeople(entity, entityViewModel, entityViewModel.AircraftGuid);
                _context.Flights.Add(entity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(_flightLogic.FillLists(entityViewModel));
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Modifier([FromRoute] Guid Id)
        {
            var entity = _context.Flights.Include(nameof(Flight.FlightPersons)).FirstOrDefault(s => s.Id == Id);
            if (entity != null)
                return View(_flightLogic.FillLists(entity));
            else
                return NotFound();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Modifier(FlightsViewModel entityViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(_flightLogic.FillLists(entityViewModel));
            }

            try
            {
                // Récupération de l'entité pour comparer le RowVersion 
                var originalEntity = _context.Flights.AsNoTracking().FirstOrDefault(s => s.Id == entityViewModel.Id);
                if (originalEntity == null)
                    return NotFound();

                if (!originalEntity.RowVersion.Equals(entityViewModel.RowVersion))
                {
                    ModelState.AddModelError(string.Empty, "Veuillez rafraîchir la page car des entrées on été modifiées depuis votre dernière consultation !");
                    return View(_flightLogic.FillLists(entityViewModel));
                }

                if (!_flightLogic.TryReserveAircraft(entityViewModel, entityViewModel.AircraftGuid))
                {
                    ModelState.AddModelError(string.Empty, "Cet avion est déjà réservé dans cette plage horaire");
                    return View(_flightLogic.FillLists(entityViewModel));
                }

                Flight entity = entityViewModel;
                entity = _flightLogic.SetPeople(entity, entityViewModel, entityViewModel.AircraftGuid);
                entity = (Flight)entity.Update(originalEntity);
                _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.Flights.Update(entity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(_flightLogic.FillLists(entityViewModel));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Archiver(Guid Id)
        {
            var entity = _context.Flights.Find(Id);

            if (entity == null)
            {
                return NotFound();
            }

            try
            {
                entity.Status = EntitiesEnums.EStatus.ARCHIVED;
                entity = (Flight)entity.Update(entity);
                _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.Flights.Update(entity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(entity);
            }

            return Json(entity);
        }
    }
}