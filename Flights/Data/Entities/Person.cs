using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Flights.Data.Entities.EntitiesEnums;

namespace Flights.Data.Entities
{
    public class Person : IdentityUser
    {
        public enum ECivility : sbyte
        {
            [Display(Name = "Monsieur")]
            SIR = 1,
            [Display(Name = "Madame")]
            MADAM = 2
        }

        [Required(ErrorMessage = "Le nom est requis")]
        [Display(Name ="Nom")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Le prénom est requis")]
        [Display(Name = "Prénom")]
        public string Name { get; set; }

        [Required(ErrorMessage = "L'adresse est requise")]
        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Required(ErrorMessage = "La ville est requise")]
        [Display(Name = "Ville")]
        public string Town { get; set; }

        [Required(ErrorMessage = "Le code postal est requis")]
        [Display(Name = "Code postal")]
        public string ZIPCode { get; set; }

        /// <summary>
        /// Override ici pour ajouter le Required + Display !
        /// </summary>
        [Required(ErrorMessage = "Le n° de téléphone est requis")]
        [Display(Name = "N° de téléphone")]
        public override string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Impossible de déterminer le statut du compte")]
        [Display(Name = "Statut")]
        public EStatus Status { get; set; }

        [Required(ErrorMessage = "La civilité est requise")]
        [Display(Name = "Civilité")]
        public ECivility Civility { get; set; }

        public List<FlightPerson> FlightPersons { get; set; }
    }
}
