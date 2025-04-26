using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ReservationStatusDTO
    {
        public int ID { get; set; }
        public string Status { get; set; }

        public ReservationStatusDTO() { }

        public ReservationStatusDTO(Domain.Entities.ReservationStatus entity)
        {
            ID = entity.ID;
            Status = entity.Status;
        }
    }
}
