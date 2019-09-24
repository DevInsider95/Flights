using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Flights.Data.Entities.EntitiesEnums;

namespace Flights.Data.Entities
{
    public abstract class BaseCRUD
    {
        public Guid Id { get; set; }
        public Guid RowVersion { get; set; }

        [Required(ErrorMessage = "Le statut est requis")]
        [Display(Name = "Statut")]
        public EStatus Status { get; set; }

        public abstract BaseCRUD Update(BaseCRUD originalEntity);

        public BaseCRUD()
        {
            this.Id = Guid.NewGuid();
            this.RowVersion = Guid.NewGuid();
            this.Status = EStatus.ACTIVE;
        }
    }
}
