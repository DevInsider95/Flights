using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Flights.Data.Entities.EntitiesEnums;

namespace Flights.Data.Entities
{
    public abstract class BaseCRUD
    {
        public Guid Id { get; set; }
        public Guid RowVersion { get; set; }
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
