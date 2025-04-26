using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ServiceStatusDTO
    {
        public int ID { get; set; }
        public string Status { get; set; }

        public ServiceStatusDTO() { }

        public ServiceStatusDTO(Domain.Entities.ServiceStatus entity)
        {
            ID = entity.ID;
            Status = entity.Status;
        }
    }
}

