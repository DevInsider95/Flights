 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Flights.Data;
using Flights.Data.Entities;
using Flights.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Flights.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {
        private readonly CSignInManager<Person> _signInManager;
        private readonly CUserManager<Person> _userManager;
        private readonly IEmailSender _emailSender;

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public LoginController(CSignInManager<Person> signInManager, CUserManager<Person> userManager, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(LoginViewModel LoginViewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(LoginViewModel);
            }

            // Connexion par adresse email ou nom d'utilisateur
            Person UserToLog;

            // Si une adresse email est détectée
            if (IsValidEmail(LoginViewModel.Email))
            {
                // Récupération de l'utilisateur par adresse email
                UserToLog = await _userManager.FindByEmailAsync(LoginViewModel.Email);
            }
            else
            {
                // Récupération de l'utilisateur par nom d'utilisateur
                UserToLog = await _userManager.FindByNameAsync(LoginViewModel.Email);
            }

            // Connexion de l'utilisateur
            var TaskResult = await _signInManager.PasswordSignInAsync(UserToLog, LoginViewModel.Password, LoginViewModel.RememberMe, true);

            if ( TaskResult.Succeeded )
            {
                // Redirection vers l'action Home par défaut
                return Redirect("/");
            }
            else if ( TaskResult.IsDisabled )
            {
                ModelState.AddModelError(string.Empty, "Ce compte a été désactivé");
                return View(LoginViewModel);
            }
            else if ( TaskResult.IsLockedOut )
            {
                ModelState.AddModelError(string.Empty, "Ce compte a temporairement été bloqué");
                return View(LoginViewModel);
            }
            else if ( TaskResult.DoesNotExists)
            {
                ModelState.AddModelError(string.Empty, "Ce compte n'existe pas");
                return View(LoginViewModel);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Vos identifiants sont incorrects");
                return View(LoginViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Disconnect()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        public IActionResult ForbiddenAccess()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Forgot()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Forgot(string Email)
        {
            if (IsValidEmail(Email))
            {
                var User = await _userManager.FindByEmailAsync(Email);

                if ( User != null )
                {
                    // Génération du token pour la génération du mot de passe
                    var Token = await _userManager.GeneratePasswordResetTokenAsync(User);

                    // Envoi du mail pour la création du compte
                    var callbackUrl = Url.Action("ResetPassword", "Login", new { area = "", userId = User.Id, code = Token }, Request.Scheme);

                    await _emailSender.SendEmailAsync(Email, "Veuillez redéfinir votre mot de passe",
                        $"Veuillez redéfinir votre mot de passe <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>En cliquant ici</a>.");
                }
            }

            return Redirect("/");
        }


        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string code)
        {
            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel
            {
                userId = userId,
                code = code
            };

            return View(resetPasswordViewModel);
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(resetPasswordViewModel);
            }

            if (resetPasswordViewModel.Password != resetPasswordViewModel.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Les mots de passes ne correspondent pas");
                return View(resetPasswordViewModel);
            }

            var User = await _userManager.FindByIdAsync(resetPasswordViewModel.userId);
            
            if ( User == null )
            {
                ModelState.AddModelError(string.Empty, "L'utilisateur n'existe pas !");
                return View(resetPasswordViewModel);
            }

            var result = await _userManager.ResetPasswordAsync(User, resetPasswordViewModel.code, resetPasswordViewModel.Password);

            if (!result.Succeeded)
            {
                // Retour des erreurs
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(resetPasswordViewModel);
            }

            return Redirect("/");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (!ModelState.IsValid)
                return View(changePasswordViewModel);

            if (changePasswordViewModel.NewPassword != changePasswordViewModel.ConfirmNewPassword)
            {
                ModelState.AddModelError(string.Empty, "Le nouveau mot de passe ne correspond pas à la confirmation");
                return View(changePasswordViewModel);
            }

            // Récupération de l'utilisateur
            Person User = await _userManager.GetUserAsync(HttpContext.User);
            var result = await _userManager.ChangePasswordAsync(User, changePasswordViewModel.Password, changePasswordViewModel.NewPassword);

            if (!result.Succeeded)
            {
                // Retour des erreurs
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(changePasswordViewModel);
            }

            // Déconnexion
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }
    }
}