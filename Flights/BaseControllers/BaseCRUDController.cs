using Flights.Data;
using Flights.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flights.BaseControllers
{
    public class BaseCRUDController<TEntity> : Controller where TEntity : BaseCRUD, new()
    {
        protected readonly ApplicationDbContext _context;

        public BaseCRUDController(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual IActionResult Index()
        {
            var entities = _context.Set<TEntity>().Where(s => s.Status == EntitiesEnums.EStatus.ACTIVE).ToList();
            return View(entities);
        }

        public virtual IActionResult Archives()
        {
            var entities = _context.Set<TEntity>().Where(s => s.Status == EntitiesEnums.EStatus.ARCHIVED).ToList();
            return View(entities);
        }

        public virtual IActionResult Creer()
        {
            return View(new TEntity());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual IActionResult Creer(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return View(entity);
            }

            try
            {
                _context.Set<TEntity>().Add(entity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(entity);
            }

            return RedirectToAction(nameof(Index));
        }

        public virtual IActionResult Modifier([FromRoute] Guid Id)
        {
            var entity = _context.Set<TEntity>().Find(Id);
            if (entity != null)
                return View(entity);
            else
                return NotFound();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual IActionResult Modifier(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return View(entity);
            }

            try
            {
                // Récupération de l'entité pour comparer le RowVersion 
                // Et utiliser la méthode Update(TEntity entity); de l'entité afin de ne pas mettre à jour tous les champs
                // Par exemple : les listes seront en général envoyées (null) et nous ne voulons pas supprimer les listes en mettant à jour des informations
                var originalEntity = _context.Set<TEntity>().AsNoTracking().FirstOrDefault(s => s.Id == entity.Id);
                if (originalEntity == null)
                    return NotFound();

                if(!originalEntity.RowVersion.Equals(entity.RowVersion))
                {
                    ModelState.AddModelError(string.Empty, "Veuillez rafraîchir la page car des entrées on été modifiées depuis votre dernière consultation !");
                    return View(entity);
                }

                entity = (TEntity)entity.Update(originalEntity);
                _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.Set<TEntity>().Update(entity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(entity);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public virtual IActionResult Archiver(Guid Id)
        {
            var entity = _context.Set<TEntity>().Find(Id);

            if (entity == null)
            {
                return NotFound();
            }

            try
            {
                entity.Status = EntitiesEnums.EStatus.ARCHIVED;
                entity = (TEntity)entity.Update(entity);
                _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.Set<TEntity>().Update(entity);
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
