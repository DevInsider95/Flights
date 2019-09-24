using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Flights.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Adresse Email")]
        [Required(ErrorMessage = "L'adresse email est requise")]
        public string Email { get; set; }

        [Display(Name ="Mot de passe")]
        [Required(ErrorMessage = "Le mot de passe est requis")]
        public string Password { get; set; }

        [Display(Name ="Se souvenir de moi ?")]
        public bool RememberMe { get; set; }
    }
}
