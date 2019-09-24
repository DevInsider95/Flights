using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Flights.Data.Entities
{
    public static class EntitiesEnums
    {
        /// <summary>
        /// Statut de l'entité
        /// </summary>
        public enum EStatus : sbyte
        {
            [Display(Name = "Actif")]
            ACTIVE,
            [Display(Name = "Archivé")]
            ARCHIVED
        }
    }
}
