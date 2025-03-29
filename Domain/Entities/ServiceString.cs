using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ServiceString
    {
        [Key]
        public int ID { get; set; }

        public int Count { get; set; }

        public int AdditionalServiceID { get; set; }

        public int ReservationID { get; set; }

        public decimal Price { get; set; }
        public int ServiceStatusID { get; set; }

        public virtual AdditionalService AdditionalService { get; set; }

        public virtual Reservation Reservation { get; set; }
        public virtual ServiceStatus ServiceStatus { get; set; }
    }
}