using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Flights.Models
{
    public class ResetPasswordViewModel
    {
        public string userId { get; set; }
        public string code { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Required(ErrorMessage = "La confirmation du mot de passe est requise")]
        [Display(Name = "Confirmation du mot de passe")]
        public string ConfirmPassword { get; set; }
    }
}
