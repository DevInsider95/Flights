using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Flights.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Le mot de passe est requis")]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Le nouveau mot de passe est requis")]
        [Display(Name = "Nouveau mot de passe")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "La confirmation du nouveau mot de passe est requise")]
        [Display(Name = "Confirmation du nouveau mot de passe")]
        public string ConfirmNewPassword { get; set; }
    }
}
