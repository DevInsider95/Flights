using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Flights.Attributes;
using Flights.Data;
using Flights.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Flights.Areas.Administration.Controllers
{
    [BaseActionFilter]
    [Authorize(Roles = GlobalResources.ROLE_ADMIN)]
    [Area(nameof(Administration))]
    public class ClientsController : Controller
    {
        private readonly CUserManager<Person> _userManager;
        private readonly IEmailSender _emailSender;

        public ClientsController(CUserManager<Person> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Person> Clients = await _userManager.GetUsersInRoleAsync(GlobalResources.ROLE_CLIENT);
            Clients = Clients.Where(s => s.Status == EntitiesEnums.EStatus.ACTIVE).ToList();
            return View(Clients);
        }

        public async Task<IActionResult> Archives()
        {
            IEnumerable<Person> Clients = await _userManager.GetUsersInRoleAsync(GlobalResources.ROLE_CLIENT);
            Clients = Clients.Where(s => s.Status == EntitiesEnums.EStatus.ARCHIVED).ToList();
            return View(Clients);
        }

        public IActionResult Creer()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Creer(Person client)
        {
            if (!ModelState.IsValid)
                return View(client);

            // Création d'un compte client
            var result = await _userManager.CreateAsync(client);

            if (result.Succeeded)
            {
                // Ajout au rôle de client
                result = await _userManager.AddToRoleAsync(client, GlobalResources.ROLE_CLIENT);

                if (result.Succeeded)
                {
                    // Génération du token pour la génération du mot de passe
                    var Token = await _userManager.GeneratePasswordResetTokenAsync(client);

                    // Envoi du mail pour la création du compte
                    var callbackUrl = Url.Action("ResetPassword", "Login", new { area = "", userId = client.Id, code = Token }, Request.Scheme);

                    await _emailSender.SendEmailAsync(client.Email, "Veuillez définir votre mot de passe",
                        $"Veuillez définir votre mot de passe <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>En cliquant ici</a>.");
                }
                else
                {
                    // Retour des erreurs
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(client);
                }
            }
            else
            {
                // Retour des erreurs
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(client);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Modifier(string Id)
        {
            var User = await _userManager.FindByIdAsync(Id);
            return View(User);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Modifier([FromRoute] string Id, string editBtn, Person person)
        {
            if (!ModelState.IsValid)
                return View(User);

            // Récupération de l'utilisateur
            var OriginalUser = await _userManager.FindByIdAsync(person.Id);

            // Mise à jour de l'utilisateur
            OriginalUser.Address = person.Address;
            OriginalUser.ZIPCode = person.ZIPCode;
            OriginalUser.Email = person.Email;
            OriginalUser.Surname = person.Surname;
            OriginalUser.PhoneNumber = person.PhoneNumber;
            OriginalUser.Name = person.Name;
            OriginalUser.Town = person.Town;
            OriginalUser.Status = person.Status;
            OriginalUser.Civility = person.Civility;

            var result = await _userManager.UpdateAsync(OriginalUser);
            if (!result.Succeeded)
            {
                // Retour des erreurs
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(User);
            }

            if (editBtn == "edit_resend")
            {
                // Génération du token pour la génération du mot de passe
                var Token = await _userManager.GeneratePasswordResetTokenAsync(OriginalUser);

                // Envoi du mail pour la création du compte
                var callbackUrl = Url.Action("ResetPassword", "Login", new { area = "", userId = OriginalUser.Id, code = Token }, Request.Scheme);

                await _emailSender.SendEmailAsync(OriginalUser.Email, "Veuillez définir votre mot de passe",
                    $"Veuillez définir votre mot de passe <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>En cliquant ici</a>.");
            }

            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Archivage d'un utilisateur
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Archiver(string Id)
        {
            var person = await _userManager.FindByIdAsync(Id);

            if (person == null)
            {
                return BadRequest();
            }

            // Suppression de l'utilisateur
            IdentityResult IResult = await _userManager.DeleteAsync(person);

            // Gestion du retour des erreurs
            if (!IResult.Succeeded)
            {
                foreach (IdentityError IdentityError in IResult.Errors)
                {
                    ModelState.AddModelError(IdentityError.Code, IdentityError.Description);
                }

                return BadRequest(ModelState);
            }

            return Json(person);
        }
    }
}